using System.Collections;
using UnityEngine;

public class RepeatingArrow : Arrow
{
    [Header("Repeating Settings")]
    [SerializeField] private int additionalShots = 2;
    [SerializeField] private float delayBetweenShots = 0.2f;

    //Properties
    public int AdditionalShots { get => additionalShots; set => additionalShots = value; }

    private void Start()
    {
        if (additionalShots > 0)
        {
            StartCoroutine(RepeatFire());
        }
    }

    private IEnumerator RepeatFire()
    {
        // Save the original force
        Vector3 originalForce = rb.linearVelocity;

        for (int i = 0; i < additionalShots; i++)
        {
            yield return new WaitForSeconds(delayBetweenShots);

            // Spawn another arrow
            GameObject repeatArrowObj = ObjectPooler.Instance.SpawnFromPool(ArrowType.Repeating, transform.position, transform.rotation);
            if (repeatArrowObj != null && repeatArrowObj.TryGetComponent(out RepeatingArrow repeatArrow))
            {
                repeatArrow.ResetArrow();
                repeatArrow.Arrowhead = this.Arrowhead;
                repeatArrow.MinDamage = this.MinDamage;
                repeatArrow.MaxDamage = this.MaxDamage;

                Rigidbody repeatRb = repeatArrow.GetComponent<Rigidbody>();
                Collider repeatCol = repeatArrow.GetComponent<Collider>();

                repeatRb.isKinematic = false;
                repeatCol.enabled = true;

                repeatRb.AddForce(originalForce, ForceMode.VelocityChange);
            }
        }
    }
}