using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[System.Serializable]
public class ArrowEntry
{
    public ArrowType arrowType;
    public Arrow arrowPrefab;
}

public class WeaponController : MonoBehaviour
{
    //Components
    [Header("Components")]
    [SerializeField] private Transform weaponTransform;
    [SerializeField] private Transform firePoint;
    [SerializeField] private InputReader input;
    [SerializeField] private LineRenderer trajectoryLineRenderer;
    [SerializeField] private GameObject trajectoryPointPrefab;
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
    //Arrow nocking
    private Arrow currentArrowInstance;
    [SerializeField] private Vector3 arrowRestPosition = new Vector3(2.5f, 0, 0);
    [SerializeField] private Vector3 arrowPulledPosition = new Vector3(1f, 0, 0);

    //Trajectory Visuals
    [Header("Trajectory Visuals")]
    [SerializeField] private int numberOfPoints = 30;
    [SerializeField] private float distanceBetweenPoints = 0.2f;
    [SerializeField] private GameObject[] trajectoryVisual;

    private void Awake()
    {
        //Get references
        playerStats = GetComponent<PlayerStats>();
        playerController = GetComponent<PlayerController>();

        //Debugging in case reference is missing
        if (playerStats == null) Debug.LogWarning("WeaponController is missing a PlayerStats reference");
        if (playerController == null) Debug.LogWarning("WeaponController is missing a PlayerController reference");

        //Initialize the trajectory visuals
        trajectoryVisual = new GameObject[numberOfPoints];
        for (int i = 0; i < numberOfPoints; i++)
        {
            trajectoryVisual[i] = Instantiate(trajectoryPointPrefab, firePoint.position, Quaternion.identity, transform.Find("Weapon/TrajectoryVisuals"));
            trajectoryVisual[i].SetActive(false);
        }
        trajectoryLineRenderer.positionCount = numberOfPoints;

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
                //Increase draw
                launchVelocityTimer += Time.deltaTime * playerStats.DrawSpeed;
                currentLaunchVelocity = Mathf.Lerp(playerStats.MinLaunchVelocity, playerStats.MaxLaunchVelocity, launchVelocityTimer);
                
                //Slowly pull the arrow back from rest position to pulled position
                if (currentArrowInstance != null)
                {
                    float drawPercent = Mathf.InverseLerp(playerStats.MinLaunchVelocity, playerStats.MaxLaunchVelocity, currentLaunchVelocity);
                    currentArrowInstance.transform.localPosition = Vector3.Lerp(arrowRestPosition, arrowPulledPosition, drawPercent);
                }
            }

            //Update the shoot direction and the trajectory visuals
            shootDirection = weaponTransform.right * currentLaunchVelocity;
            UpdateTrajectory();
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
        Arrow arrowPrefab = allWeapons[currentArrowType];

        //currentArrowInstance = Instantiate(arrowPrefab, weaponTransform);
        ////Change arrow position
        //currentArrowInstance.transform.localPosition = arrowRestPosition;
        ////Change arrow rotation
        //currentArrowInstance.transform.localRotation = Quaternion.Euler(0, 90, 0);

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
        foreach (var point in trajectoryVisual)
            point.SetActive(true);
        trajectoryLineRenderer.enabled = true;
    }

    private void AimWeaponTowardMouse()
    {
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

        StartCoroutine(ShootCoroutine());
    }

    private IEnumerator ShootCoroutine()
    {
        if (currentArrowInstance == null) yield break;

        //Disable trajectory visuals
        foreach (var point in trajectoryVisual)
            point.SetActive(false);
        trajectoryLineRenderer.enabled = false;

        // Detach and fire the nocked arrow
        currentArrowInstance.transform.parent = null;
        //Enable physics
        Rigidbody rb = currentArrowInstance.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        //Add velocity
        rb.AddForce(shootDirection, ForceMode.VelocityChange);
        //Enable collision
        currentArrowInstance.GetComponent<Collider>().enabled = true;

        //Reset the current nocked arrow
        currentArrowInstance = null;

        //Wait the firing delay
        yield return new WaitForSeconds(playerStats.DelayBetweenShots);
        isShooting = false;
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

        foreach (var point in trajectoryVisual)
            point.SetActive(false);

        trajectoryLineRenderer.enabled = false;
    }

    private void PauseDraw(bool isPaused)
    {
        isDrawPaused = isPaused;
    }

    private void UpdateTrajectory()
    {
        for (int i = 0; i < numberOfPoints; i++)
        {
            Vector3 pos = firePoint.position + shootDirection * (i * distanceBetweenPoints) + 0.5f * Physics.gravity * Mathf.Pow(i * distanceBetweenPoints, 2);
            trajectoryVisual[i].transform.position = pos;
            trajectoryLineRenderer.SetPosition(i, pos);
        }
    }

    private void ScrollArrow(float direction)
    {
        if (isDrawing) return;

        int arrowTypeCount = allWeapons.Count;
        int currentIndex = (int)currentArrowType;
        currentIndex = (currentIndex + (direction > 0 ? 1 : -1) + arrowTypeCount) % arrowTypeCount;
        currentArrowType = (ArrowType)currentIndex;
    }


    private void SelectArrow(int index)
    {
        if (isDrawing) return;

        int arrowTypeCount = System.Enum.GetValues(typeof(ArrowType)).Length;
        //Index -1 so that if you press 1 numkey you get the basic arrow which is index 0
        currentArrowType = (ArrowType)Mathf.Clamp(index -1 , 0, arrowTypeCount - 1);
    }


    private void SelectArrowhead(int index)
    {
        if (isDrawing) return;
        currentArrowheadIndex = (ArrowheadType)Mathf.Clamp(index, 0, 2);
    }

    public bool IsDrawing => isDrawing;
    public bool IsShooting => isShooting;
}