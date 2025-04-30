using System.Collections;
using System.Linq;
using UnityEngine;

public class ShieldController : MonoBehaviour, IDamageable
{
    public bool isFrozen;
    //References
    [Header("References")]
    [SerializeField] private StatusEffectManager statusEffectManager;
    
    //Health
    [Header("Health")]
    [SerializeField] private float shieldMaxHealth = 5;
    [SerializeField] private float shieldCurrentHealth;


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
        //bool 
            isFrozen = statusEffectManager.ActiveEffects.Any(effect => effect is FreezeEffect);

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

    public void TakeDamage(DamageInfo info)
    {
        info.Amount = Mathf.Round(info.Amount);
        DamageVisualController.ShowDamageVisual(info, transform.position);

        shieldCurrentHealth -= info.Amount;
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