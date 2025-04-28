using UnityEngine;
using UnityEngine.AI;

public class RangedEnemyController : EnemyController
{
    //Enemy
    NavMeshAgent navAgent;
    Collider weaponCollider;
    bool isDrawing = false;
    //bool isShooting = false;

    //Player
    GameObject player;
    Transform playerPosition;
    Collider playerCollider;
    PlayerStats playerStats;
    Rigidbody playerRigidbody;

    //Attack
    public int damage = 2;
    public float attackSpeed = 5;
    public float meleeAttackRange = 2f;
    public float attackRange = 30f;
    public float knockbackForce = 10f;
    float attackTimer = 0;
    public int numberOfShots = 1;
    public GameObject enemyArrow;

    //Weapon Rotation
    GameObject enemyWeapon;
    private Quaternion targetRotation;
    public float aimSpeed = 5f;
    public float angle = 135f;
    private Quaternion idlePosition;
    Vector3 shootDirection;
    float centerOffset;

    //Velocity
    public float minLaunchVelocity = 1f;
    public float currentLaunchVelocity = 1f;
    public float maxLaunchVelocity = 50f;
    public float launchVelocityTimer = 0;
    public float drawSpeed = 0.1f;

    //Trajectory
    public GameObject[] trajectoryVisual;
    public GameObject trajectoryVisualObject;
    public int numberOfPoints;
    public float distanceBetweenPoints = 0.2f;

    //LINE RENDERER NOT NEEDED
    public LineRenderer trajectoryLineRenderer;

    void Start()
    {
        //Enemy
        navAgent = GetComponent<NavMeshAgent>();
        weaponCollider = transform.Find("EnemyWeapon").GetComponent<Collider>();
        enemyWeapon = transform.Find("EnemyWeapon").gameObject;

        //Find arrow prefab
        //enemyArrow = Resources.Load<GameObject>("EnemyWeapons/EnemyBasicArrow");

        //Player
        player = GameObject.FindWithTag("Player");
        playerPosition = player.transform;
        playerCollider = player.GetComponent<Collider>();
        playerStats = player.GetComponent<PlayerStats>();
        playerRigidbody = player.GetComponent<Rigidbody>();

        //Find idle position
        idlePosition = enemyWeapon.transform.rotation;
        targetRotation = idlePosition;

        //Initialize trajectory
        trajectoryVisualObject = Resources.Load<GameObject>("TrajectoryVisualisationPoint");
        trajectoryVisual = new GameObject[numberOfPoints];
        for (int i = 0; i < numberOfPoints; i++)
        {
            trajectoryVisual[i] = Instantiate(trajectoryVisualObject, enemyWeapon.transform.position, Quaternion.identity, transform.Find("TrajectoryVisuals"));
        }

        //LINE RENDERER NOT NEEDED!
        trajectoryLineRenderer.positionCount = numberOfPoints;
    }

    void Update()
    {
        if (player != null)
        {
            //Attack speed
            if (attackTimer <= attackSpeed)
            {
                attackTimer += Time.deltaTime;
                navAgent.SetDestination(playerPosition.position);
                ResetDraw();
            }
            else
            {
                //Attacks when in melee range
                if (Vector3.Distance(transform.position, playerPosition.position) <= meleeAttackRange)
                {
                    navAgent.ResetPath();
                    Attack();
                }
                else
                {
                    //Disables melee weapon
                    weaponCollider.enabled = false;

                    //Shoots when in range
                    if (Vector3.Distance(transform.position, playerPosition.position) <= attackRange)
                    {
                        navAgent.ResetPath();
                        DrawWeapon();
                    }
                    else
                    {
                        navAgent.SetDestination(playerPosition.position);
                        ResetDraw();
                    }
                }
            }
        }
        if (!isDrawing)
        {
            centerOffset = Random.Range(0f, 1.5f);
        }

        //Rotates weapon to desired rotation
        enemyWeapon.transform.rotation = Quaternion.Slerp(enemyWeapon.transform.rotation, targetRotation, aimSpeed * Time.deltaTime);
    }

    //Melee attack
    void Attack()
    {
        weaponCollider.enabled = true;
        if (weaponCollider.bounds.Intersects(playerCollider.bounds) && attackTimer > attackSpeed)
        {
            playerStats.TakeDamage(damage);
            playerRigidbody.AddForce((playerPosition.position - transform.position).normalized * knockbackForce, ForceMode.Impulse);
            attackTimer = 0;
        }
    }

    //Ranged attack
    void DrawWeapon()
    {
        isDrawing = true;

        //LINE RENDERER NOT NEEDED!
        trajectoryLineRenderer.enabled = true;

        if (currentLaunchVelocity < maxLaunchVelocity)
        {
            launchVelocityTimer += Time.deltaTime * drawSpeed;
            currentLaunchVelocity = Mathf.Lerp(minLaunchVelocity, maxLaunchVelocity, launchVelocityTimer);
        }

        //Aim weapon
        shootDirection = enemyWeapon.transform.right * currentLaunchVelocity;
        targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));

        //Trajectory
        //bool hitSomething = false;
        //while (!hitSomething)
        //{
            for (int i = 0; i < trajectoryVisual.Length - 1; i++)
            {
                trajectoryVisual[i].SetActive(true);
                trajectoryVisual[i].transform.position = TrajectoryVisualization(i * distanceBetweenPoints);

                //LINE RENDERER NOT NEEDED!
                trajectoryLineRenderer.enabled = true;
                trajectoryLineRenderer.SetPosition(i, trajectoryVisual[i].transform.position);

                //Find position of trajectory point, and the one after it, calculates the direction between them
                Vector3 startRay = trajectoryVisual[i].transform.position;
                Vector3 endRay = trajectoryVisual[i + 1].transform.position;
                Vector3 rayDirection = (endRay - startRay).normalized;

                //Ray cast between the trajectory points and looks for player collision
                if (Physics.Raycast(startRay, rayDirection, out RaycastHit hit, Vector3.Distance(startRay, endRay)))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        //Compares raycast hit position to player center + the random offset
                        float distanceToPlayerCenter = Vector3.Distance(hit.point, playerPosition.position + new Vector3(0, centerOffset, 0));
                        if (distanceToPlayerCenter <= playerCollider.bounds.extents.magnitude / 2)
                        {
                            Shoot();
                        }
                    }

                    //else if (!hit.collider.CompareTag("Player"))
                    //{
                    //    hitSomething = true;
                    //    break;
                    //}
                }
            }
        //}
    }

    void Shoot()
    {
        isDrawing = false;
        //isShooting = true;

        while (numberOfShots > 0)
        {
            GameObject shootProjectile = Instantiate(enemyArrow, enemyWeapon.transform.position, Quaternion.identity);
            Rigidbody projectileRigidbody = shootProjectile.GetComponent<Rigidbody>();
            projectileRigidbody.AddForce(shootDirection, ForceMode.VelocityChange);
            shootProjectile.transform.rotation = enemyWeapon.transform.rotation;

            numberOfShots--;
        }

        //LINE RENDERER NOT NEEDED!
        for (int i = 0; trajectoryVisual.Length > i; i++)
        {
            trajectoryLineRenderer.enabled = false;
            trajectoryVisual[i].SetActive(false);
        }

        //Reset parameters after shooting
        ResetDraw();
        attackTimer = 0;
        numberOfShots = 1;
    }
    Vector3 TrajectoryVisualization(float t)
    {
        Vector3 currentPointPosition = enemyWeapon.transform.position + shootDirection * t + 0.5f * t * t * Physics.gravity;
        return currentPointPosition;
    }

    void ResetDraw()
    {
        isDrawing = false;
        //isShooting = false;
        currentLaunchVelocity = minLaunchVelocity;
        launchVelocityTimer = 0;
        targetRotation = idlePosition;

        //LINE RENDERER NOT NEEDED!
        trajectoryLineRenderer.enabled = false;
    }
}
