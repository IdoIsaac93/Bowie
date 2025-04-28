using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    //Health
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private float currentHealth;

    //UI
    [SerializeField] private EnemyHealthBar healthBar;

    //Damage resistance/vulnerability
    [SerializeField] private float negativeResistance = 0f;
    [SerializeField] private float piercingResistance = 0f;
    [SerializeField] private float positiveResistance = 0f;

    //Player
    private PlayerStats playerStats;
    [SerializeField] private int experienceWorth = 10;

    //Damage visual
    private GameObject damageVisualObject;

    void Start()
    {
        healthBar = GetComponentInChildren<EnemyHealthBar>();
        healthBar.SetMaxHealth(maxHealth);
        currentHealth = maxHealth;

        damageVisualObject = Resources.Load<GameObject>("DamageVisual");
        playerStats = GameObject.FindWithTag("Player").GetComponent<PlayerStats>();
    }

    void Update()
    {
        healthBar.SetHealth((int)currentHealth);
    }

    public void TakeDamage(float damage, ArrowheadType type)
    {
        float resistance = 0f;
        switch (type)
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
        damage *= resistanceMultiplier;

        damage = Mathf.Round(damage);
        DamageVisual(damage , type);
        
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    //Create damage visual above enemy
    void DamageVisual(float damage, ArrowheadType type)
    {
        Vector3 posOffset = transform.position + new Vector3(0, 3, 0);
        GameObject damageVisual = Instantiate(damageVisualObject, posOffset, Quaternion.identity);

        //Change damage visual text to match damage done
        TMPro.TextMeshProUGUI damageVisualText = damageVisual.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        damageVisualText.text = damage.ToString();

        switch (type)
        {
            case ArrowheadType.Negative:
                damageVisualText.color = Color.black;
                break;
            case ArrowheadType.Piercing:
                //Hexadecimal FF9500
                damageVisualText.color = new Color(1f, 0.584f, 0f);
                break;
            case ArrowheadType.Positive:
                //Hexadecimal 00FFF9
                damageVisualText.color = new Color(0f, 1f, 0.976f);
                break;
        }
    }

    void Die()
    {
        playerStats.ExperiencePoints += experienceWorth;
        Destroy(gameObject);
    }
}
