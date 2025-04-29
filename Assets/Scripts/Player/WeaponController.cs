using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    //Components
    [Header("Components")]
    [SerializeField] private Transform weaponTransform;
    [SerializeField] private Transform firePoint;
    [SerializeField] private InputReader input;
    [SerializeField] private TrajectoryVisuals trajectoryVisuals;
    private PlayerStats playerStats;
    private PlayerController playerController;

    //Arrow
    [Header("Arrow Settings")]
    //Current arrow
    [SerializeField] private List<ArrowEntry> arrowEntries = new();
    public Dictionary<ArrowType, Arrow> allWeapons = new();
    public ArrowType currentArrowType = ArrowType.Basic;
    public ArrowheadType currentArrowheadIndex = ArrowheadType.Piercing;
    //Launch settings
    private float currentLaunchVelocity;
    private float launchVelocityTimer;
    private Vector3 shootDirection;
    //State control
    private bool isDrawing = false;
    private bool isShooting = false;
    private bool isDrawPaused = false;
    private bool isAimLocked = false;

    //Arrow nocking
    private Arrow currentArrowInstance;
    [SerializeField] private Vector3 arrowRestPosition = new(2.5f, 0, 0);
    [SerializeField] private Vector3 arrowPulledPosition = new(1f, 0, 0);

    private void Awake()
    {
        //Get references
        playerStats = GetComponent<PlayerStats>();
        playerController = GetComponent<PlayerController>();
        trajectoryVisuals = GetComponent<TrajectoryVisuals>();

        //Debugging in case reference is missing
        if (playerStats == null) Debug.LogWarning("WeaponController is missing a PlayerStats reference");
        if (playerController == null) Debug.LogWarning("WeaponController is missing a PlayerController reference");
        if (trajectoryVisuals == null) Debug.LogWarning("WeaponController is missing a TrajectoryVisuals reference");

        // Subscribe to events
        input.DrawBowStarted += StartDraw;
        input.DrawBowReleased += ReleaseArrow;
        input.PauseDraw += PauseDraw;
        input.CancelDraw += CancelDraw;
        input.ScrollArrow += ScrollArrow;
        input.SelectArrow += SelectArrow;
        input.SelectArrowhead += SelectArrowhead;
        playerController.DodgePerformed += CancelDraw;
    }

    private void Start()
    {
        allWeapons = new Dictionary<ArrowType, Arrow>();

        foreach (var entry in arrowEntries)
        {
            if (!allWeapons.ContainsKey(entry.arrowType))
            {
                allWeapons.Add(entry.arrowType, entry.arrowPrefab);
            }
            else
            {
                Debug.LogWarning($"Duplicate ArrowType {entry.arrowType} found in WeaponController!");
            }
        }
    }

    private void OnDestroy()
    {
        //Unsubscribe from events
        input.DrawBowStarted -= StartDraw;
        input.DrawBowReleased -= ReleaseArrow;
        input.PauseDraw -= PauseDraw;
        input.CancelDraw -= CancelDraw;
        input.ScrollArrow -= ScrollArrow;
        input.SelectArrow -= SelectArrow;
        input.SelectArrowhead -= SelectArrowhead;
        playerController.DodgePerformed -= CancelDraw;
    }

    private void Update()
    {
        if (isDrawing)
        {
            AimWeaponTowardMouse();

            //Increase draw strength when not paused and below the max draw
            if (!isDrawPaused && currentLaunchVelocity < playerStats.MaxLaunchVelocity)
            {
                launchVelocityTimer += Time.deltaTime * playerStats.DrawSpeed;
                currentLaunchVelocity = Mathf.Lerp(playerStats.MinLaunchVelocity, playerStats.MaxLaunchVelocity, launchVelocityTimer);
            }

            //Update shoot direction and trajectory visuals
            shootDirection = weaponTransform.right * currentLaunchVelocity;
            Vector3 startingPosition = currentArrowInstance != null ? currentArrowInstance.transform.position : firePoint.position;
            trajectoryVisuals.UpdateTrajectory(startingPosition, shootDirection);
        }

        //Slowly pull the arrow back from rest position to pulled position
        if (currentArrowInstance != null)
        {
            float drawPercent = Mathf.InverseLerp(playerStats.MinLaunchVelocity, playerStats.MaxLaunchVelocity, currentLaunchVelocity);
            currentArrowInstance.transform.localPosition = Vector3.Lerp(arrowRestPosition, arrowPulledPosition, drawPercent);
        }
    }

    private void StartDraw()
    {
        //Makes sure to wait the shooting delay
        if (isShooting) return;

        //Initialize drawing
        isDrawing = true;
        launchVelocityTimer = 0;
        currentLaunchVelocity = playerStats.MinLaunchVelocity;

        //Instantiate and nock the arrow
        currentArrowInstance = ObjectPooler.Instance.SpawnFromPool(currentArrowType, firePoint.position, weaponTransform.rotation * Quaternion.Euler(0, 90, 0)).GetComponent<Arrow>();
        currentArrowInstance.ResetArrow();
        currentArrowInstance.transform.SetParent(weaponTransform);
        currentArrowInstance.transform.localPosition = arrowRestPosition;

        //Change arrowhead
        currentArrowInstance.Arrowhead = currentArrowheadIndex;
        //Disable arrow collision
        currentArrowInstance.GetComponent<Collider>().enabled = false;
        //Disable physics so it follows the bow
        Rigidbody arrowRb = currentArrowInstance.GetComponent<Rigidbody>();
        arrowRb.isKinematic = true;
        //Activate trajectory visuals
        trajectoryVisuals.Show();
    }

    private void AimWeaponTowardMouse()
    {
        //Don't aim if locked
        if (isAimLocked) return;

        //Get the mouse position and aim towards it
        Vector3 mousePos = Input.mousePosition;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, weaponTransform.position.z - Camera.main.transform.position.z));
        Vector3 aimDirection = (mouseWorldPos - weaponTransform.position).normalized;
        //Lock the angle to 2 dimensions
        float targetAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        // Clamp the angle between -135 and -45 degrees
        if (targetAngle > -135f && targetAngle < -45f)
        {
            // Snap to closest valid edge
            if (Mathf.Abs(targetAngle + 45f) < Mathf.Abs(targetAngle + 135f))
                targetAngle = -45f;
            else
                targetAngle = -135f;
        }

        //Rotate the bow towards target rotation at the speed of AimSpeed
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        weaponTransform.rotation = Quaternion.Lerp(weaponTransform.rotation, targetRotation, playerStats.AimSpeed * Time.deltaTime);

        ////Rotate the players facing direction
        //float currentZAngle = weaponTransform.rotation.eulerAngles.z;
        //if (currentZAngle > 180) currentZAngle -= 360;

        //if (currentZAngle > 90 || currentZAngle < -90)
        //{
        //    playerController.transform.localRotation = Quaternion.Euler(0, 180, 0);
        //}
        //else
        //{
        //    playerController.transform.localRotation = Quaternion.Euler(0, 0, 0);
        //}
    }

    private void ReleaseArrow()
    {
        if (!isDrawing || isShooting) return;

        isDrawing = false;
        isShooting = true;

        //trajectoryVisuals.Hide();

        StartCoroutine(ShootCoroutine());
    }

    private IEnumerator ShootCoroutine()
    {
        if (currentArrowInstance == null) yield break;

        // Detach and fire the nocked arrow
        FireArrow(currentArrowInstance, shootDirection);

        // Check if the arrow is a repeating type
        currentArrowInstance.TryGetComponent(out RepeatingArrow repeatingArrow);

        // Reset the current nocked arrow reference
        currentArrowInstance = null;

        // If the arrow is repeating, fire additional arrows
        if (repeatingArrow != null)
        {
            // Cache shoot data for consistency across repeated shots
            Vector3 cachedShootDirection = shootDirection;
            Quaternion cachedRotation = weaponTransform.rotation;
            float cachedDrawPercent = Mathf.InverseLerp(playerStats.MinLaunchVelocity, playerStats.MaxLaunchVelocity, currentLaunchVelocity);

            // Lock aim while repeating arrows are fired
            isAimLocked = true;
            yield return StartCoroutine(RepeatArrowsCoroutine(repeatingArrow, cachedShootDirection, cachedRotation, cachedDrawPercent));
            isAimLocked = false;
        }

        // Wait the firing delay before allowing next action
        isShooting = false;
        yield return new WaitForSeconds(playerStats.DelayBetweenShots);
    }

    private IEnumerator RepeatArrowsCoroutine(RepeatingArrow repeatingArrow, Vector3 cachedShootDirection, Quaternion cachedRotation, float cachedDrawPercent)
    {
        yield return new WaitForSeconds(repeatingArrow.DelayBetweenShots);

        int shotsLeft = repeatingArrow.AdditionalShots;

        while (shotsLeft > 0)
        {
            // Nock new arrow
            Arrow newArrow = ObjectPooler.Instance.SpawnFromPool(currentArrowType, firePoint.position, cachedRotation * Quaternion.Euler(0, 90, 0)).GetComponent<Arrow>();
            SetupNockedArrow(newArrow);

            // Animate pullback toward cached draw percent
            yield return AnimateDraw(newArrow, cachedDrawPercent, repeatingArrow.DelayBetweenShots);

            // Fire the arrow
            FireArrow(newArrow, cachedShootDirection);

            shotsLeft--;

            // Wait for next shot if needed
            if (shotsLeft > 0)
            {
                yield return new WaitForSeconds(repeatingArrow.DelayBetweenShots);
            }
        }
    }

    // Sets up a freshly nocked arrow before firing.
    private void SetupNockedArrow(Arrow arrow)
    {
        arrow.ResetArrow();
        arrow.Arrowhead = currentArrowheadIndex;
        arrow.transform.SetParent(weaponTransform);
        arrow.transform.localPosition = arrowRestPosition;
        arrow.GetComponent<Rigidbody>().isKinematic = true;
        arrow.GetComponent<Collider>().enabled = false;
    }

    // Animates the arrow pulling back towards a certain draw percent.
    private IEnumerator AnimateDraw(Arrow arrow, float targetDrawPercent, float duration)
    {
        float elapsedTime = 0f;
        Vector3 targetPosition = Vector3.Lerp(arrowRestPosition, arrowPulledPosition, targetDrawPercent);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            arrow.transform.localPosition = Vector3.Lerp(arrowRestPosition, targetPosition, t);
            yield return null;
        }
    }

    // Fires the arrow by detaching, enabling physics, and adding force.
    private void FireArrow(Arrow arrow, Vector3 direction)
    {
        arrow.transform.parent = null;
        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        Collider col = arrow.GetComponent<Collider>();

        rb.isKinematic = false;
        col.enabled = true;
        rb.AddForce(direction, ForceMode.VelocityChange);
    }


    private void CancelDraw()
    {
        if (!isDrawing) return;

        isDrawing = false;
        launchVelocityTimer = 0;
        currentLaunchVelocity = playerStats.MinLaunchVelocity;

        //Destroy nocked arrow
        if (currentArrowInstance != null)
        {
            Destroy(currentArrowInstance.gameObject);
            currentArrowInstance = null;
        }

        trajectoryVisuals.Hide();
    }

    private void PauseDraw(bool isPaused)
    {
        isDrawPaused = isPaused;
    }

    private void ScrollArrow(float direction)
    {
        if (isDrawing || isShooting) return;

        int arrowTypeCount = allWeapons.Count;
        int currentIndex = (int)currentArrowType;
        currentIndex = (currentIndex + (direction > 0 ? 1 : -1) + arrowTypeCount) % arrowTypeCount;
        currentArrowType = (ArrowType)currentIndex;
    }


    private void SelectArrow(int index)
    {
        if (isDrawing || isShooting) return;

        int arrowTypeCount = System.Enum.GetValues(typeof(ArrowType)).Length;
        //Index -1 so that if you press 1 numkey you get the basic arrow which is index 0
        currentArrowType = (ArrowType)Mathf.Clamp(index - 1, 0, arrowTypeCount - 1);
    }


    private void SelectArrowhead(int index)
    {
        if (isDrawing || isShooting) return;
        currentArrowheadIndex = (ArrowheadType)Mathf.Clamp(index, 0, 2);
    }

    public bool IsDrawing => isDrawing;
    public bool IsShooting => isShooting;
}