using UnityEngine;

public class SplittingArrow : Arrow
{
    [Header("Splitting Settings")]
    [SerializeField] private int splitCount = 3;
    [SerializeField] private float spreadAngle = 60f;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private InputReader inputReader;
    [SerializeField] private bool hasSplit = false;

    private void Start()
    {
        //Debuggers in case references are missing
        if(inputReader == null) { Debug.LogWarning("SplittingArrow is missing InputReader reference"); }
        if(arrowPrefab == null) { Debug.LogWarning("SplittingArrow is missing SplitShardPrefab"); }
    }

    private void OnEnable()
    {
        // Subscribe to event
        inputReader.TriggerArrowEffect += SplitArrow;
    }

    private void OnDisable()
    {
        // Unsubscribe from event
        inputReader.TriggerArrowEffect -= SplitArrow;
    }

    private void SplitArrow()
    {
        if (!isFlying || hasSplit) return;

        hasSplit = true;
        float angleStep = spreadAngle / (splitCount - 1);
        float startAngle = -spreadAngle / 2f;

        for (int i = 0; i < splitCount; i++)
        {
            float angle = startAngle + i * angleStep;
            Vector3 direction = Quaternion.AngleAxis(angle, Vector3.forward) * rb.linearVelocity.normalized;

            GameObject newArrow = ObjectPooler.Instance.SpawnFromPool(ArrowType.Splitting, transform.position, Quaternion.LookRotation(direction, Vector3.up));
            Rigidbody newRb = newArrow.GetComponent<Rigidbody>();
            if (newRb != null)
                newRb.isKinematic = false;
                newRb.linearVelocity = direction * rb.linearVelocity.magnitude;
            
            // Configure the new arrow
            if (newArrow.TryGetComponent(out SplittingArrow newSplitArrow))
            {
                // prevent further splitting
                newSplitArrow.hasSplit = true;
                // Inherit properties
                newSplitArrow.Arrowhead = this.Arrowhead;
                newSplitArrow.MinDamage = this.MinDamage;
                newSplitArrow.MaxDamage = this.MaxDamage;
                newSplitArrow.isFlying = true;
                
            }
        }

        // Deactivate the original arrow
        Deactivate();
    }

    public override void ResetArrow()
    {
        base.ResetArrow();
        hasSplit = false;
    }
}
