using System.Diagnostics.Contracts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class FlyingMeleeEnemyController : EnemyController
{
    //Enemy
    Collider weaponCollider;
    Rigidbody rb;

    //Movement
    public int maxRange = 20;
    public int minRange = 10;
    public int maxHeight = 20;
    public int minHeight = 10;
    public float currentSpeed;
    public float moveSpeed = 1f;
    public float retreatSpeed = 5f;
    public float diveSpeed = 15f;
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
    public float attackSpeed = 15;
    public float attackRange = 20f;
    public float knockbackForce = 30f;
    float attackTimer = 0;
    bool isAttacking = false;

    void Start()
    {
        //Enemy
        rb = GetComponent<Rigidbody>();
        weaponCollider = transform.Find("EnemyWeapon").GetComponent<Collider>();
        targetPosition = transform.position;

        //Player
        player = GameObject.FindWithTag("Player");
        playerCollider = player.GetComponent<Collider>();
        playerStats = player.GetComponent<PlayerStats>();
        playerRigidbody = player.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (player != null)
        {
            playerPosition = player.transform.position;

            //Melee Attack
            if (Vector3.Distance(transform.position, playerPosition) <= attackRange && attackTimer > attackSpeed)
            {
                if (!isAttacking)
                {
                    //Locks the player position to where it was when the attack started
                    targetPosition = playerPosition;
                    isAttacking = true;
                }
                Attack();
            }
            else
            {
                Movement();
                moveTimer += Time.deltaTime;

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
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        rb.AddForce(moveDirection * diveSpeed, ForceMode.Force);

        weaponCollider.enabled = true;

        //Deals damage when weapon collides with player
        if (weaponCollider.bounds.Intersects(playerCollider.bounds))
        {
            playerStats.TakeDamage(damage);

            //Push player and enemy back
            playerRigidbody.AddForce((playerPosition - transform.position).normalized * knockbackForce, ForceMode.Impulse);
            rb.AddForce((transform.position - playerPosition).normalized * knockbackForce * 5, ForceMode.Impulse);

            EndAttack();
        }

        if (Vector3.Distance(transform.position, targetPosition) <= 1)
        {
            EndAttack();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isAttacking)
        {
            EndAttack();
        }
    }

    void EndAttack()
    {
        isAttacking = false;
        attackTimer = 0;
    }

    void Movement()
    {
        //First find the distance to player and to ground
        LayerMask ignoreLayer = LayerMask.GetMask("Enemy") | LayerMask.GetMask("Projectiles") | LayerMask.GetMask("Player");
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, ~ignoreLayer);
        float distanceToGround = hit.distance;
        float distanceToPlayer = Vector3.Distance(transform.position, playerPosition);

        //Move towards player when above max range
        if (distanceToPlayer >= maxRange)
        {
            targetPosition = playerPosition;
            targetPosition.z = 0;
            currentSpeed = moveSpeed;
        }

        //Move away from player when below min range
        else if (distanceToPlayer <= minRange)
        {
            targetPosition = transform.position - (playerPosition - transform.position).normalized * maxRange;
            targetPosition.z = 0;
            currentSpeed = retreatSpeed;
        }

        //When within range, move randomly
        else if (moveTimer > moveInterval)
        {
            //Move randomly within the given constraints
            bool flag = true;
            while (flag)
            {
                //Get a random position within max range of player
                Vector3 randomDirection = Random.insideUnitSphere * maxRange;
                randomDirection += transform.position;
                randomDirection.z = 0;

                //Check that new random position is'nt too close
                if (Vector3.Distance(transform.position, randomDirection) < 3)
                {
                    return;
                }
                //Move to new random position
                else
                {
                    targetPosition = randomDirection;
                    currentSpeed = moveSpeed;
                    moveTimer = 0;
                    flag = false;
                }
            }
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

        //And finally, move to target position
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        rb.AddForce(moveDirection * currentSpeed, ForceMode.Force);
    }
}
