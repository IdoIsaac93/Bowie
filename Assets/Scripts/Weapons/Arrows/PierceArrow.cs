using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PierceArrow : Arrow
{
    //Piercing
    [Header("Pierce Effect")]
    [SerializeField] private int pierceAmount = 2;

    //List of enemies hit
    private HashSet<GameObject> hitEnemies = new();

    //Properties
    public int PierceAmount { get => pierceAmount; set => pierceAmount = value; }
    protected override void Update()
    {
        base.Update();
        IgnoreHitEnemies();
    }

    private void IgnoreHitEnemies()
    {
        foreach (GameObject enemy in hitEnemies)
        {
            if (enemy != null)
            {
                Collider[] enemyColliders = enemy.GetComponentsInChildren<Collider>();
                foreach (Collider enemyCollider in enemyColliders)
                {
                    Physics.IgnoreCollision(col, enemyCollider, true);
                }
            }
        }
    }

    protected override void HandleCollision(Collider collider)
    {
        if (!isFlying) return;

        // Handle shield separately
        if (collider.CompareTag("EnemyShield"))
        {
            ShieldController shield = collider.GetComponentInChildren<ShieldController>();
            if (shield != null)
            {
                float shieldDamage = CalculateDamage(collider);
                shield.TakeDamage(shieldDamage, arrowhead);
            }
            // Do NOT add shield to hitEnemies
            return;
        }

        // Handle ground/obstacles
        if (!collider.CompareTag("Enemy") && !collider.CompareTag("WeakSpot"))
        {
            StickArrow(collider);
            return;
        }

        // If it's a valid enemy/weakspot
        EnemyHealth enemyHealth = collider.GetComponentInParent<EnemyHealth>();
        if (enemyHealth != null && !hitEnemies.Contains(enemyHealth.gameObject))
        {
            float finalDamage = CalculateDamage(collider);
            ApplyDamage(collider, finalDamage);
            hitEnemies.Add(enemyHealth.gameObject);
            pierceAmount--;
        }

        // If no pierces left, stick the arrow
        if (pierceAmount <= 0)
        {
            StickArrow(collider);
        }
    }
}