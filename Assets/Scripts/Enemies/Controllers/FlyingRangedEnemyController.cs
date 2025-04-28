using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class FlyingRangedEnemyController : EnemyController
{
    //Enemy
    Collider weaponCollider;
    Rigidbody rb;

    //Movement
    public int maxRange = 20;
    public int minRange = 10;
    public int maxHeight = 20;
    public int minHeight = 10;
    Vector3 targetPosition;
    float moveTimer = 0;
    public float moveInterval = 5f;

    //Player
    GameObject player;
    Vector3 playerPosition;
    Collider playerCollider;
    PlayerStats playerStats;
    Rigidbody playerRigidbody;

    //Attack
    public int damage = 2;
    public float attackSpeed = 2;
    public float attackRange = 20f;
    public float knockbackForce = 30f;
    float attackTimer = 0;

    void Start()
    {
        //Enemy
        rb = GetComponent<Rigidbody>();
        weaponCollider = transform.Find("EnemyWeapon").GetComponent<Collider>();
        targetPosition = transform.position;

        //Player
        player = GameObject.FindWithTag("Player");
        playerPosition = player.transform.position;
        playerCollider = player.GetComponent<Collider>();
        playerStats = player.GetComponent<PlayerStats>();
        playerRigidbody = player.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (player != null)
        {
            Movement();
            moveTimer += Time.deltaTime;

            //Melee Attack
            if (Vector3.Distance(transform.position, playerPosition) <= 1)
            {
                Attack();
            }
            else
            {
                weaponCollider.enabled = false;
            }

            //Attack speed
            if (attackTimer <= attackSpeed)
            {
                attackTimer += Time.deltaTime;
            }

            //Rotate to face player
            transform.LookAt(playerPosition);
        }
    }

    void Attack()
    {
        weaponCollider.enabled = true;
        //Deals damage when weapon collides with player
        if (weaponCollider.bounds.Intersects(playerCollider.bounds) && attackTimer > attackSpeed)
        {
            playerStats.TakeDamage(damage);

            //Push player and enemy back
            playerRigidbody.AddForce((playerPosition - transform.position).normalized * knockbackForce, ForceMode.Impulse);
            rb.AddForce((transform.position - playerPosition).normalized * knockbackForce * 5, ForceMode.Impulse);
            attackTimer = 0;
        }
    }

    void Movement()
    {
        //First find the distance to player and to ground
        LayerMask enemyLayer = LayerMask.GetMask("Enemy") | LayerMask.GetMask("Projectiles");
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, ~enemyLayer);
        float distanceToGround = hit.distance;
        float distanceToPlayer = Vector3.Distance(transform.position, playerPosition);

        //Move towards or away from player
        if (distanceToPlayer >= maxRange)
        {
            targetPosition = playerPosition;
            targetPosition.z = transform.position.z;
        }
        else if (distanceToPlayer <= minRange)
        {
            targetPosition = transform.position - (playerPosition - transform.position).normalized * maxRange;
            targetPosition.z = transform.position.z;
        }

        //When within range, move randomly
        else if (moveTimer > moveInterval)
        {
            //Move randomly within the given constraints
            Vector3 randomDirection = Random.insideUnitSphere * maxRange;
            randomDirection += transform.position;
            randomDirection.z = transform.position.z;
            targetPosition = randomDirection;
            moveTimer = 0;
        }

        //Adjust height
        if (distanceToGround < minHeight)
        {
            targetPosition += Vector3.up * (minHeight - distanceToGround);
        }
        if (distanceToGround >= maxHeight)
        {
            targetPosition += Vector3.down * (distanceToGround - maxHeight);
        }

        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        rb.AddForce(moveDirection * movementSpeed, ForceMode.Force);
    }
}
