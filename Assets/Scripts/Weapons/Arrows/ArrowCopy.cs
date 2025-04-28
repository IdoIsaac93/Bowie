using UnityEngine;
using UnityEngine.UI;

public class ArrowCopy : MonoBehaviour
{
    //Components
    protected Rigidbody rb;
    protected Collider col;
    protected PlayerStats stats;
    protected WeaponController weapon;

    //Flight
    protected bool isFlying = true;
    [SerializeField] protected float timeUntillDestroy = 8f;

    //Damage
    [SerializeField] protected ArrowheadType arrowhead;
    protected string damageType;
    protected float damage;
    [SerializeField] protected int minDamage = 1;
    [SerializeField] protected int maxDamage = 3;
    [SerializeField] protected int numberOfShots = 1;
    public ArrowheadType Arrowhead { get { return arrowhead; } set { arrowhead = value; } }
    public int MinDamage { get { return minDamage; } set { minDamage = value; } }
    public int MaxDamage { get { return maxDamage; } set { maxDamage = value; } }
    public int NumberOfShots { get { return numberOfShots; } set { numberOfShots = value; } }



    //Cooldown
    [SerializeField] private float shotDelay = 1f;
    [SerializeField] private float cooldown = 3;
    public float ShotDelay { get { return shotDelay; } set { shotDelay = value; } }
    public float Cooldown { get { return cooldown; } set { cooldown = value; } }

    //Icon
    private Sprite icon;

    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        GameObject player = GameObject.FindWithTag("Player");
        stats = player.GetComponent<PlayerStats>();
        weapon = player.GetComponent<WeaponController>();
    }
    public void Update()
    {
        //Rotates the arrow according to flight arc
        if (isFlying && rb.linearVelocity.magnitude > 0)
        {
            transform.rotation = Quaternion.LookRotation(rb.linearVelocity.normalized);
        }

        //Changes damage type to match arrowhead
        switch (arrowhead)
        {
            case ArrowheadType.Negative:
                damageType = "steel";
                break;
            case ArrowheadType.Piercing:
                damageType = "fire";
                break;
            case ArrowheadType.Positive:
                damageType = "ice";
                break;
        }
    }

    public void OnTriggerEnter(Collider collider)
    {
        //Randomize damage
        damage = Random.Range(minDamage, maxDamage + 1);

        //Shield hit
        if (collider.gameObject.CompareTag("EnemyShield"))
        {
            ShieldController enemyShield = collider.gameObject.GetComponentInChildren<ShieldController>();

            //Round and deal damage to shield
            damage = Mathf.Round(damage);
            enemyShield.TakeDamage(damage, arrowhead);
        }
        //Critical hit
        else if (collider.gameObject.CompareTag("WeakSpot"))
        {
            EnemyHealth enemyHealth = collider.gameObject.GetComponentInParent<EnemyHealth>();
            damage *= stats.CriticalMultiplier;

            //Round and deal damage
            damage = Mathf.Round(damage);
            enemyHealth.TakeDamage(damage, arrowhead);
        }
        //Enemy hit
        else if (collider.gameObject.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = collider.gameObject.GetComponentInParent<EnemyHealth>();

            //Round and deal damage
            damage = Mathf.Round(damage);
            enemyHealth.TakeDamage(damage, arrowhead);
        }


        //Stick projectile to whatever it hit, disable its physics and collision
        isFlying = false;
        rb.isKinematic = true;
        col.enabled = false;
        transform.parent = collider.gameObject.transform;

        //Destroy projectile a few seconds after it collided
        Destroy(gameObject, timeUntillDestroy);
    }
}
