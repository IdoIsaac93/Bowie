using UnityEngine;

public class BombArrow : Arrow
{
    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionForce = 2500f;
    [SerializeField] private LayerMask damageableLayers;
    [SerializeField] private InputReader inputReader;

    private bool hasExploded = false;

    //Properties
    public float ExplosionRadius { get => explosionRadius; set => explosionRadius = value; }
    public float ExplosionForce { get => explosionForce; set => explosionForce = value; }
    protected override void Awake()
    {
        base.Awake();
        if (inputReader != null)
        {
            inputReader.TriggerArrowEffect += Explode;
        }
        else
        {
            Debug.LogWarning("BombArrow is missing InputReader reference!");
        }
    }

    private void OnDestroy()
    {
        if (inputReader != null)
        {
            inputReader.TriggerArrowEffect -= Explode;
        }
    }

    protected override void HandleCollision(Collider collider)
    {
        if (!isFlying) return;

        Explode();
    }


    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        float explosionDamage = Random.Range(minDamage, maxDamage + 1);
        DamageInfo damageInfo = new DamageInfo(explosionDamage, false, false, false, arrowhead);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, damageableLayers);

        foreach (Collider hit in colliders)
        {
            IDamageable damageable = hit.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damageInfo);
            }

            Rigidbody rb = hit.GetComponentInParent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        Deactivate();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    public override void ResetArrow()
    {
        base.ResetArrow();
        hasExploded = false;
    }
}