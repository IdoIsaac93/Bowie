using System.Collections;
using System.Linq;
using UnityEngine;

public class ShieldController : MonoBehaviour, IDamageable
{
    //References
    [Header("References")]
    [SerializeField] private StatusEffectManager statusEffectManager;
    
    //Health
    [Header("Health")]
    [SerializeField] private float shieldMaxHealth = 5;
    [SerializeField] private float shieldCurrentHealth;

    //Resistances
    [Header("Resistances")]
    [SerializeField] private float negativeResistance = 0f;
    [SerializeField] private float piercingResistance = 0f;
    [SerializeField] private float positiveResistance = 0f;

    // Shield rotation
    [Header("Shield Rotation")]
    private GameObject shieldRotation;
    [SerializeField] private float posChangeInterval = 10f;
    private bool isRaised = false;
    [SerializeField] private Quaternion raisedAngle = Quaternion.Euler(-45f, -90f, 0f);
    [SerializeField] private Quaternion idleAngle = Quaternion.Euler(0, -90, 0);
    [SerializeField] private Quaternion loweredAngle = Quaternion.Euler(45f, -90f, 0f);

    void Start()
    {
        statusEffectManager = GetComponentInParent<StatusEffectManager>();
        shieldCurrentHealth = shieldMaxHealth;
        shieldRotation = transform.parent.gameObject;
        StartCoroutine(ChangeShieldState());
    }

    void Update()
    {
        bool isFrozen = statusEffectManager.ActiveEffects.Any(effect => effect is FreezeEffect);

        if (isFrozen)
        {
            shieldRotation.transform.rotation = Quaternion.Slerp(shieldRotation.transform.rotation, loweredAngle, 1 * Time.deltaTime);
        }
        else
        {
            shieldRotation.transform.rotation = Quaternion.Slerp(shieldRotation.transform.rotation,
                isRaised ? raisedAngle : idleAngle, 1 * Time.deltaTime);
        }
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

        shieldCurrentHealth -= damageInfo.Amount;
        if (shieldCurrentHealth <= 0)
        {
            DestroyShield();
        }
    }

    IEnumerator ChangeShieldState()
    {
        while (true)
        {
            yield return new WaitForSeconds(posChangeInterval);
            isRaised = Random.Range(0, 2) == 0;
        }
    }

    void DestroyShield()
    {
        gameObject.SetActive(false);
    }
}