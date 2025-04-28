using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemyController : EnemyController
{
    //Enemy
    NavMeshAgent navAgent;
    Collider weaponCollider;
    
    //Player
    GameObject player;
    Transform playerPosition;
    Collider playerCollider;
    PlayerStats playerStats;
    Rigidbody playerRigidbody;

    //Attack
    public int damage = 2;
    public float attackSpeed = 2;
    public float attackRange = 2f;
    public float knockbackForce = 10f;
    float attackTimer = 0;

    void Start()
    {
        //Enemy
        navAgent = GetComponent<NavMeshAgent>();
        weaponCollider = transform.Find("EnemyWeapon").GetComponent<Collider>();

        //Player
        player = GameObject.FindWithTag("Player");
        playerPosition = player.transform;
        playerCollider = player.GetComponent<Collider>();
        playerStats = player.GetComponent<PlayerStats>();
        playerRigidbody = player.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (player != null)
        {
            navAgent.SetDestination(playerPosition.position);

            //Attacks when in range
            if (Vector3.Distance(transform.position, playerPosition.position) <= attackRange)
            {
                Attack();
            }
            else
            {
                weaponCollider.enabled = false;
            }
        }

        //Attack speed
        if (attackTimer <= attackSpeed)
        {
            attackTimer += Time.deltaTime;
        }
    }

    void Attack()
    {
        weaponCollider.enabled = true;
        //Deals damage when weapon collides with player
        if (weaponCollider.bounds.Intersects(playerCollider.bounds) && attackTimer > attackSpeed)
        {
            playerStats.TakeDamage(damage);
            playerRigidbody.AddForce((playerPosition.position - transform.position).normalized * knockbackForce, ForceMode.Impulse);
            attackTimer = 0;
        }
    }
}
