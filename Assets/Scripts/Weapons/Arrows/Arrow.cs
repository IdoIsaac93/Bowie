using UnityEngine;

public class Arrow : MonoBehaviour
{
    protected Rigidbody rb;
    protected Collider col;
    protected PlayerStats stats;
    protected WeaponController weapon;

    //Flight
    [Header("Flight")]
    protected bool isFlying = true;
    [SerializeField] protected float timeUntilDestroy = 8f;

    //Damage
    [Header("Damage Settings")]
    [SerializeField] protected ArrowheadType arrowhead;
    protected float damage;
    [SerializeField] protected int minDamage = 1;
    [SerializeField] protected int maxDamage = 3;

    //Cooldown
    [Header("Cooldown Settings")]
    [SerializeField] private float cooldown = 3f;

    //Manergy cost
    [Header("Manergy Cost")]
    [SerializeField] private int manergyCost = 0;

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
        GameObject player = GameObject.FindWithTag("Player");
        stats = player.GetComponent<PlayerStats>();
        weapon = player.GetComponent<WeaponController>();

        //Debugging in case reference is missing
        if (rb == null) { Debug.LogWarning("Arrow is missing Rigidbody reference"); }
        if (col == null) { Debug.LogWarning("Arrow is missing Collider reference"); }
        if (player == null) { Debug.LogWarning("Arrow is missing Player reference"); }
        if (stats == null) { Debug.LogWarning("Arrow is missing PlayerStats reference"); }
        if (weapon == null) { Debug.LogWarning("Arrow is missing WeaponController reference"); }
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

        float calculatedDamage = CalculateDamage(collider);

        ApplyDamage(collider, calculatedDamage);

        StickArrow(collider);
    }

    protected float CalculateDamage(Collider collider)
    {
        float baseDamage = Random.Range(minDamage, maxDamage + 1);

        if (collider.CompareTag("WeakSpot"))
        {
            baseDamage *= stats.WeakSpotDamageMod;
        }

        return Mathf.Round(baseDamage);
    }

    protected void ApplyDamage(Collider collider, float damageAmount)
    {
        if (collider.CompareTag("EnemyShield"))
        {
            ShieldController shield = collider.GetComponentInChildren<ShieldController>();
            if (shield != null)
                shield.TakeDamage(damageAmount, arrowhead);
        }
        else if (collider.CompareTag("WeakSpot") || collider.CompareTag("Enemy"))
        {
            EnemyHealth enemy = collider.GetComponentInParent<EnemyHealth>();
            if (enemy != null)
                enemy.TakeDamage(damageAmount, arrowhead);
        }
    }

    protected void StickArrow(Collider collider)
    {
        isFlying = false;
        rb.isKinematic = true;
        col.enabled = false;
        transform.parent = collider.transform;

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
