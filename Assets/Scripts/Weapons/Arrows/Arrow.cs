using UnityEngine;

public class Arrow : MonoBehaviour
{
    //References
    protected Rigidbody rb;
    protected Collider col;

    //Flight
    [Header("Flight")]
    protected bool isFlying = true;
    [SerializeField] protected float timeUntilDestroy = 8f;

    //Damage
    [Header("Damage Settings")]
    [SerializeField] protected ArrowheadType arrowhead;
    [SerializeField] protected int minDamage = 1;
    [SerializeField] protected int maxDamage = 3;
    protected float damage;

    //Cooldown
    [Header("Cooldown Settings")]
    [SerializeField] private float cooldown = 3f;

    //Manergy cost
    [Header("Manergy Cost")]
    [SerializeField] private int manergyCost = 0;

    protected bool isShield;

    //Properties
    public ArrowheadType Arrowhead { get => arrowhead; set => arrowhead = value; }
    public int MinDamage { get => minDamage; set => minDamage = value; }
    public int MaxDamage { get => maxDamage; set => maxDamage = value; }
    public float Cooldown { get => cooldown; set => cooldown = value; }
    public int ManergyCost { get => manergyCost; set => manergyCost = value; }

    protected virtual void Awake()
    {
        //Get components
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        //Debugging in case reference is missing
        if (rb == null) { Debug.LogWarning("Arrow is missing Rigidbody reference"); }
        if (col == null) { Debug.LogWarning("Arrow is missing Collider reference"); }
    }

    protected virtual void Update()
    {
        HandleFlight();
    }

    protected void HandleFlight()
    {
        if (isFlying && rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(rb.linearVelocity.normalized);
        }
    }

    protected void OnTriggerEnter(Collider collider)
    {
        HandleCollision(collider);
    }

    protected virtual void HandleCollision(Collider collider)
    {
        if (!isFlying) return;

        DamageInfo calculatedDamage = CalculateDamage(collider);

        ApplyDamage(collider, calculatedDamage);

        StickArrow(collider);
    }

    protected DamageInfo CalculateDamage(Collider collider)
    {
        float baseDamage = Random.Range(minDamage, maxDamage + 1);
        bool isCritical = Random.value < PlayerStats.Instance.CriticalChance;
        bool isWeakspot = collider.CompareTag("WeakSpot");
        isShield = collider.CompareTag("EnemyShield");

        if (isWeakspot)
        {
            baseDamage *= PlayerStats.Instance.WeakSpotDamageMod;
        }

        if (isCritical)
        {
            baseDamage *= PlayerStats.Instance.CriticalMultiplier;
        }

        _ = Mathf.Round(baseDamage);

        return new DamageInfo(baseDamage, isCritical, isWeakspot, isShield, arrowhead);
    }

    protected void ApplyDamage(Collider collider, DamageInfo damage)
    {
        if (damage.IsShieldHit)
        {
            ShieldController shield = collider.GetComponentInChildren<ShieldController>();
            if (shield != null)
                shield.TakeDamage(damage);
        }
        else if (collider.CompareTag("WeakSpot") || collider.CompareTag("Enemy"))
        {
            EnemyHealth enemy = collider.GetComponentInParent<EnemyHealth>();
            if (enemy != null)
                enemy.TakeDamage(damage);
        }

        Debug.Log($"Arrow hit! Damage: {damage.Amount}, Crit: {damage.IsCritical}, Weakspot: {damage.IsWeakspot}, Shield: {damage.IsShieldHit}");

    }

    protected void StickArrow(Collider collider)
    {
        isFlying = false;
        rb.isKinematic = true;
        col.enabled = false;

        transform.SetParent(collider.transform, true);

        Invoke(nameof(Deactivate), timeUntilDestroy);
    }

    protected void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public virtual void ResetArrow()
    {
        // Reset movement
        if (rb == null) rb = GetComponent<Rigidbody>(); // Just in case
        //Turn on physics so arrow velocity can be reset
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        //Turn off physics so arrow doesn't fall while nocked
        rb.isKinematic = true;

        // Reset collision
        if (col == null) col = GetComponent<Collider>();
        col.enabled = false;

        // Reset flying state
        isFlying = true;

        // Unparent (optional - you might reparent manually later anyway)
        transform.parent = null;
    }
}
