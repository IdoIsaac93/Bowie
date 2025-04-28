using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBasicBullet : MonoBehaviour
{
    //Damage
    public float damage;
    public int minDamage = 1;
    public int maxDamage = 3;

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
        }

        //Destroy projectile
        Destroy(gameObject);
    }
}
