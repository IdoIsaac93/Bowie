using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShieldController : MonoBehaviour, IDamageable
{
    //Health
    public float shieldMaxHealth = 5;
    float shieldCurrentHealth;

    //Status effects
    bool isOnFire = false;
    float burnTimer = 0f;
    bool isSlowed = false;
    float slowTimer = 0f;

    //Damage resistance/vulnerability
    public float damageMultiplier = 2;
    public bool isFireResistant = false;
    public bool isFireVulnerable = false;
    public bool isIceResistant = false;
    public bool isIceVulnerable = false;

    //Damage visual
    GameObject damageVisualObject;

    //Shield rotation
    GameObject shieldRotation;
    public float posChangeInterval = 10f;
    bool isRaised = false;
    Quaternion raisedAngle = Quaternion.Euler(-45f,-90f,0f);
    Quaternion idleAngle = Quaternion.Euler(0, -90, 0);
    Quaternion loweredAngle = Quaternion.Euler(45f, -90f, 0f);

    void Start()
    {
        shieldCurrentHealth = shieldMaxHealth;

        damageVisualObject = Resources.Load<GameObject>("DamageVisual");

        shieldRotation = transform.parent.gameObject;

        StartCoroutine(ChangeShieldState());
    }

    void Update()
    {
        if (isSlowed)
        {
            shieldRotation.transform.rotation = Quaternion.Slerp(shieldRotation.transform.rotation, loweredAngle, 1 * Time.deltaTime);
        }
        else
        {
            if (isRaised)
            {
                shieldRotation.transform.rotation = Quaternion.Slerp(shieldRotation.transform.rotation, raisedAngle, 1 * Time.deltaTime);
            }
            else
            {
                shieldRotation.transform.rotation = Quaternion.Slerp(shieldRotation.transform.rotation, idleAngle, 1 * Time.deltaTime);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        damage = Mathf.Round(damage);
        DamageVisual(damage);
        shieldCurrentHealth -= damage;
        if (shieldCurrentHealth <= 0)
        {
            DestroyShield();
        }
    }

    public void TakeDamage(float damage, ArrowheadType type)
    {
        if (type == ArrowheadType.Negative)
        {
            if (isFireVulnerable)
            {
                damage *= damageMultiplier;
            }
            if (isFireResistant)
            {
                damage /= damageMultiplier;
            }
        }

        if (type == ArrowheadType.Positive)
        {
            if (isIceVulnerable)
            {
                damage *= damageMultiplier;
            }
            if (isIceResistant)
            {
                damage /= damageMultiplier;
            }
        }

        damage = Mathf.Round(damage);
        DamageVisual(damage, type);
        shieldCurrentHealth -= damage;
        if (shieldCurrentHealth <= 0)
        {
            DestroyShield();
        }
    }

    void DamageVisual(float damage)
    {
        Vector3 posOffset = transform.position + new Vector3(0, 3, 0);
        GameObject damageVisual = Instantiate(damageVisualObject, posOffset, Quaternion.identity);

        //Change damage visual text to match damage done
        TMPro.TextMeshProUGUI damageVisualText = damageVisual.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        damageVisualText.text = damage.ToString();

        //Activate "Blocked" text
        damageVisual.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.SetActive(true);
    }

    void DamageVisual(float damage, ArrowheadType type)
    {
        Vector3 posOffset = transform.position + new Vector3(0, 3, 0);
        GameObject damageVisual = Instantiate(damageVisualObject, posOffset, Quaternion.identity);

        //Change damage visual text to match damage done
        TMPro.TextMeshProUGUI damageVisualText = damageVisual.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        damageVisualText.text = damage.ToString();

        //Activate "Blocked" text
        damageVisual.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.SetActive(true);

        if (type == ArrowheadType.Negative)
        {
            //Hexadecimal FF9500
            damageVisualText.color = new Color(1f, 0.584f, 0f);
        }
        if (type == ArrowheadType.Positive)
        {
            //Hexadecimal 00FFF9
            damageVisualText.color = new Color(0f, 1f, 0.976f);
        }
    }

    //Start burning or reset timer if already burning
    public void StartBurning(float burnDamage, float burnDuration, ArrowheadType type)
    {
        if (!isOnFire && !isFireResistant)
        {
            StartCoroutine(SetOnFire(burnDamage, burnDuration, type));
            isOnFire = true;
        }
        else
        {
            burnTimer = 0;
        }
    }

    public IEnumerator SetOnFire(float burnDamage, float burnDuration, ArrowheadType type)
    {
        float burnDamageInterval = 0.5f;

        //Initial pause before damage starts
        yield return new WaitForSeconds(burnDamageInterval);

        while (burnTimer < burnDuration)
        {
            TakeDamage(burnDamage, type);
            yield return new WaitForSeconds(burnDamageInterval);
            burnTimer += burnDamageInterval;
        }

        isOnFire = false;
        burnTimer = 0;
    }

    //Start slowing or reset timer if already slowed
    public void StartSlowing(float slowDuration)
    {
        if (!isSlowed && !isIceResistant)
        {
            StartCoroutine(SetOnIce(slowDuration));
            isSlowed = true;
        }
        else
        {
            slowTimer = 0;
        }
    }

    public IEnumerator SetOnIce(float slowDuration)
    {
        //Timer so it can be reset when hit with ice when already slowed
        while (slowTimer < slowDuration)
        {
            yield return new WaitForSeconds(0.5f);
            slowTimer += 0.5f;
        }

        isSlowed = false;
        slowTimer = 0;
    }

    //Randomize if shield is raised or not
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
        Destroy(gameObject);
    }
}
