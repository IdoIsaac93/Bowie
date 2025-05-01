using System.Collections;
using UnityEngine;

public class PlayerStats : Singleton<PlayerStats>
{
    // Movement
    [Header("Movement")]
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float dodgeCooldown = 5f;
    [SerializeField] private float dodgeForce = 10f;

    // Health
    [Header("Health")]
    [SerializeField] private float currentHealth;
    [SerializeField] private float maxHealth = 100f;

    //Manergy
    [Header("Manergy")]
    [SerializeField] private float currentManergy;
    [SerializeField] private float maxManergy;

    // Attack
    [Header("Attack")]
    [SerializeField] private float criticalChance = 0.05f;
    [SerializeField] private float criticalMultiplier = 1.2f;
    [SerializeField] private float aimSpeed = 5f;
    [SerializeField] private float minLaunchVelocity = 1f;
    [SerializeField] private float maxLaunchVelocity = 30f;
    [SerializeField] private float drawSpeed = 0.2f;
    [SerializeField] private float delayBetweenShots = 1f;
    [SerializeField] private float weakSpotDamageMod = 1.5f;

    // Experience
    [Header("Experience")]
    [SerializeField] private int experiencePoints = 0;
    [SerializeField] private int maxExperiencePoints = 10;
    [SerializeField] private int playerLevel = 1;

    // Resistances
    [Header("Resistances")]
    [SerializeField] private float blindResistance = 0f;
    [SerializeField] private int fireResistance = 0;
    [SerializeField] private int iceResistance = 0;
    [SerializeField] private int armor = 0;

    // Status effects
    [Header("Status Effects")]
    [SerializeField] private bool isBlinded = false;
    private float blindedTimer = 0f;
    private Transform blindCanvas;
    private Transform blindSpotlight;
    [SerializeField] private float blindSpotlightSize = 15f;
    private bool isBurning = false;
    private float burnTimer = 0f;
    private bool isSlowed = false;
    private float slowTimer = 0f;

    // UI
    [SerializeField] private PlayerUI UI;

    //Properties
    public float MovementSpeed { get => movementSpeed;  set => movementSpeed = value; }
    public float DodgeCooldown { get => dodgeCooldown; set => dodgeCooldown = value; }
    public float DodgeForce { get => dodgeForce; set => dodgeForce = value; }
    public float CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public float MaxHealth { get => maxHealth; set => maxHealth = value; }
    public float CurrentManergy { get => currentManergy; set => currentManergy = value; }
    public float MaxManergy { get => maxManergy; set => maxManergy = value; }
    public float CriticalChance { get => criticalChance; set => criticalChance = value; }
    public float CriticalMultiplier { get => criticalMultiplier; set => criticalMultiplier = value; }
    public float AimSpeed { get => aimSpeed; set => aimSpeed = value; }
    public float MinLaunchVelocity { get => minLaunchVelocity; set => minLaunchVelocity = value; }
    public float MaxLaunchVelocity { get => maxLaunchVelocity; set => maxLaunchVelocity = value; }
    public float DrawSpeed { get => drawSpeed; set => drawSpeed = value; }
    public float DelayBetweenShots { get => delayBetweenShots; set => delayBetweenShots = value; }
    public float WeakSpotDamageMod { get => weakSpotDamageMod; set => weakSpotDamageMod = value; }
    public int ExperiencePoints { get => experiencePoints; set => experiencePoints = value; }
    public int MaxExperiencePoints { get => maxExperiencePoints; set => maxExperiencePoints = value; }
    public int PlayerLevel { get => playerLevel; set => playerLevel = value; }
    public float BlindResistance { get => blindResistance; set => blindResistance = value; }
    public int FireResistance { get => fireResistance; set => fireResistance = value; }
    public int IceResistance { get => iceResistance; set => iceResistance = value; }
    public int Armor { get => armor; set => armor = value; }
    public bool IsBlinded { get => isBlinded; set => isBlinded = value; }
    public float BlindSpotlightSize { get => blindSpotlightSize; set => blindSpotlightSize = value; }


    private void OnEnable()
    {
        EnemyHealth.OnEnemyDeath += GainXP;
    }

    private void OnDisable()
    {
        EnemyHealth.OnEnemyDeath -= GainXP;
    }
    private new void Awake()
    {
        TryGetComponent(out PlayerUI UI);
        if (UI == null)
        {
            Debug.Log("PlayerStats couldn't find playerUI");
        }

        //Blind
        blindCanvas = transform.Find("Blind");
        blindSpotlight = transform.Find("Blind").Find("Spotlight");
        blindCanvas.gameObject.SetActive(false);

        //Health
        currentHealth = maxHealth;
    }
    public void TakeDamage(float damage)
    {
        damage -= armor;
        currentHealth -= damage;
        UI.DamageVisual(damage);
        if (currentHealth <= 0)
        {
            UI.SetHealth(currentHealth);
            Die();
        }
    }

    public void TakeDamage(float damage, string type)
    {
        if (type == "fire")
        {
            damage -= fireResistance;
        }
        if (type == "ice")
        {
            damage -= iceResistance;
        }
        currentHealth -= damage;
        UI.DamageVisual(damage, type);
        if (currentHealth <= 0)
        {
            UI.SetHealth(currentHealth);
            Die();
        }
    }

    void Die()
    {
        UI.loseScreen.SetActive(true);
        Destroy(gameObject);
    }

    public void TakeHealing(float healing)
    {
        currentHealth += healing;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public void GainXP(int amount)
    {
        experiencePoints += amount;
        if (experiencePoints >= maxExperiencePoints)
        {
            LevelUp();
        }
        UI.UpdateXP(experiencePoints);
    }
    public void LevelUp()
    {
        playerLevel++;
        experiencePoints -= maxExperiencePoints;
        maxExperiencePoints += playerLevel * 10;
    }

    //Start blind or reset timer if already blinded
    public void StartBlind(float blindDuration)
    {
        //Blindspotlight size
        blindSpotlight.localScale = Vector3.one * blindSpotlightSize;

        if (!isBlinded)
        {
            StartCoroutine(SetBlinded(blindDuration));
            isBlinded = true;
        }
        else
        {
            blindedTimer = 0;
        }
    }

    public IEnumerator SetBlinded(float blindDuration)
    {
        blindCanvas.gameObject.SetActive(true);
        while (blindedTimer < (blindDuration - blindResistance))
        {
            yield return new WaitForSeconds(0.1f);
            blindedTimer += 0.1f;
        }
        blindCanvas.gameObject.SetActive(false);
        isBlinded = false;
        blindedTimer = 0;
    }

    //Start burning or reset timer if already burning
    public void StartBurning(float burnDamage, float burnDuration)
    {
        if (!isBurning)
        {
            StartCoroutine(SetOnFire(burnDamage, burnDuration));
            isBurning = true;
        }
        else
        {
            burnTimer = 0;
        }
    }

    public IEnumerator SetOnFire(float burnDamage, float burnDuration)
    {
        float burnDamageInterval = 0.5f;

        //Initial pause before damage starts
        yield return new WaitForSeconds(burnDamageInterval);

        while (burnTimer < burnDuration)
        {
            TakeDamage(burnDamage, "fire");
            yield return new WaitForSeconds(burnDamageInterval);
            burnTimer += burnDamageInterval;
        }

        isBurning = false;
        burnTimer = 0;
    }

    //Start slowing or reset timer if already slowed
    public void StartSlowing(float slowAmount, float slowDuration)
    {
        if (!isSlowed)
        {
            StartCoroutine(SetOnIce(slowAmount, slowDuration));
            isSlowed = true;
        }
        else
        {
            slowTimer = 0;
        }
    }

    public IEnumerator SetOnIce(float slowAmount, float slowDuration)
    {
        movementSpeed /= slowAmount;

        //Timer so it can be reset when hit with ice when already slowed
        while (slowTimer < slowDuration)
        {
            yield return new WaitForSeconds(0.1f);
            slowTimer += 0.1f;
        }

        movementSpeed *= slowAmount;
        isSlowed = false;
        slowTimer = 0;
    }
}
