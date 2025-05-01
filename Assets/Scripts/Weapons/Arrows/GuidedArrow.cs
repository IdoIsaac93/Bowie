using UnityEngine;

public class GuidedArrow : Arrow
{
    [Header("Guidance Settings")]
    [SerializeField] private float guideForce = 10f;
    [SerializeField] private float manergyCostPerSecond = 5f;
    [SerializeField] private InputReader inputReader;

    [SerializeField] private bool isGuiding = false;

    public float GuideForce { get => guideForce; set => guideForce = value; }

    private void OnEnable()
    {
        if (inputReader != null)
            inputReader.TriggerArrowEffect += ToggleGuiding;
    }

    private void OnDisable()
    {
        if (inputReader != null)
            inputReader.TriggerArrowEffect -= ToggleGuiding;
    }

    private void OnDestroy()
    {
        if (inputReader != null)
        {
            inputReader.TriggerArrowEffect -= ToggleGuiding;
        }
    }

    protected override void Update()
    {
        base.Update();

        if (isFlying && isGuiding)
        {
            GuideTowardsMouse();
        }
    }

    private void ToggleGuiding()
    {
        Debug.Log("Guiding toggled");
        if (!isFlying || PlayerStats.Instance.CurrentManergy < manergyCostPerSecond) return;

        isGuiding = !isGuiding;
        // You may want to set a timer to stop this if needed.
    }

    private void GuideTowardsMouse()
    {
        // Deduct manergy per second
        float manergyCostThisFrame = manergyCostPerSecond * Time.deltaTime;

        if (PlayerStats.Instance.CurrentManergy < manergyCostThisFrame)
        {
            isGuiding = false;
            return;
        }

        PlayerStats.Instance.CurrentManergy -= manergyCostThisFrame;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = transform.position.z - Camera.main.transform.position.z;
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3 direction = (worldMousePos - transform.position).normalized;

        rb.AddForce(direction * guideForce, ForceMode.Force);
    }

    public override void ResetArrow()
    {
        base.ResetArrow();
        isGuiding = false;
    }
}
