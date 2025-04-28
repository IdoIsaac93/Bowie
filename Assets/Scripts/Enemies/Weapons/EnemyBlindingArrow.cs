using UnityEngine;

public class EnemyBlindingArrow : MonoBehaviour
{
    Rigidbody rb;
    Collider col;
    bool isFlying = true;
    float timeUntillDestroy = 8f;

    //Damage
    public float damage;
    public int minDamage = 1;
    public int maxDamage = 3;
    public float blindDuration = 3;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }
    void Update()
    {
        //Rotates the arrow according to flight arc
        if (isFlying && rb.linearVelocity.magnitude > 0)
        {
            Quaternion arcRotation = Quaternion.LookRotation(rb.linearVelocity.normalized);
            transform.rotation = arcRotation;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStats playerHealth = collision.gameObject.GetComponentInParent<PlayerStats>();

            //Randomize damage and multiply if critical hit
            damage = Random.Range(minDamage, maxDamage + 1);

            //Round and deal damage
            damage = Mathf.Round(damage);
            playerHealth.TakeDamage(damage);

            //Blind player
            playerHealth.StartBlind(blindDuration);
        }

        //Stick projectile to whatever it hit, disable its physics and collision
        isFlying = false;
        rb.isKinematic = true;
        col.enabled = false;
        transform.parent = collision.transform;


        //Destroy projectile a few seconds after it collided
        Destroy(gameObject, timeUntillDestroy);
    }
}
