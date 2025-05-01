using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    //References
    [Header("References")]
    [SerializeField] private EnemyHealthBar healthBar;

    //Health
    [Header("Health")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private float currentHealth;

    //Damage resistance/vulnerability
    [Header("Resistances")]
    [SerializeField] private float negativeResistance = 0f;
    [SerializeField] private float piercingResistance = 0f;
    [SerializeField] private float positiveResistance = 0f;

    //Status Effect immunities
    [Header("Status Effects Immunity")]
    [SerializeField] private List<StatusEffectType> immuneEffects = new();

    //Experience gain
    [Header("Experience")]
    [SerializeField] private int experienceWorth = 10;
    public static event UnityAction<int> OnEnemyDeath;

    void Start()
    {
        healthBar = GetComponentInChildren<EnemyHealthBar>();
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
        else
        {
            Debug.LogWarning("EnemyHealth is missing EnemyHealthBar reference");
        }
        currentHealth = maxHealth;
    }

    void Update()
    {
        healthBar.SetHealth((int)currentHealth);
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        float resistance = 0f;
        switch (damageInfo.DamageType)
        {
            case ArrowheadType.Negative:
                resistance = negativeResistance;
                break;
            case ArrowheadType.Positive:
                resistance = positiveResistance;
                break;
            case ArrowheadType.Piercing:
                resistance = piercingResistance;
                break;
        }

        float resistanceMultiplier = 1f - (resistance / 100f);
        float damage = Mathf.Round(damageInfo.Amount * resistanceMultiplier);

        DamageInfo finalInfo = new DamageInfo(
            damage,
            damageInfo.IsCritical,
            damageInfo.IsWeakspot,
            damageInfo.IsShieldHit,
            damageInfo.DamageType
        );

        //Create damage visual above enemy
        DamageVisualController.ShowDamageVisual(finalInfo, transform.position);

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    //Checks if the enemy is immune to a certain effect
    public bool IsImmuneTo(StatusEffectType type)
    {
        return immuneEffects.Contains(type);
    }

    void Die()
    {
        OnEnemyDeath?.Invoke(experienceWorth);
        Destroy(gameObject);
    }
}
