using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    //Components
    PlayerStats stats;
    WeaponController weapon;

    //UI
    public GameObject loseScreen;
    public TMP_Text xpCounter;
    public Slider slider;
    public TMP_Text text;
    [SerializeField] GameObject damageVisualObject;

    //Weapon cooldown UI
    public List<Slider> allCDSliders;

    //Needs to be "Start" so sliders will find weapon cooldown only after "WeaponController" completes its "Awake" method
    private void Start()
    {
        if (!TryGetComponent(out stats))
        {
            Debug.Log("PlayerUI couldn't find PlayerStats");
        }

        if (!TryGetComponent(out weapon))
        {
            Debug.Log("PlayerUI couldn't find WeaponController");
        }

        //Find lose screen object and deactivate it
        loseScreen = GameObject.Find("LoseScreenContainer").transform.Find("LoseScreen").gameObject;
        if (loseScreen != null)
        {
            loseScreen.SetActive(false);
        }
        else
        {
            Debug.Log("PlayerUI couldn't find lose screen container");
        }

        //Health slider
        SetMaxHealth(stats.MaxHealth);
        SetHealth(stats.CurrentHealth);

        //Damage visual
        damageVisualObject = Resources.Load<GameObject>("DamageVisual");
        if (damageVisualObject == null)
        {
            Debug.Log("PlayerUI couldn't find damage visual object");
        }

        // Initialize the list with the correct capacity
        allCDSliders = new List<Slider>(7);

        // Find the parent object that contains the sliders
        Transform weaponLoadout = transform.Find("HUDCanvas/WeaponLoadout");

        // Loop through the number of weapons
        for (int i = 0; i < weapon.allWeapons.Count; i++)
        {
            // Find the slider by its child name
            string sliderName = "Weapon" + (i + 1) + "/WeaponSlider" + (i+1);
            Transform sliderTransform = weaponLoadout.Find(sliderName);

            if (sliderTransform != null)
            {
                Slider slider = sliderTransform.GetComponent<Slider>();
                allCDSliders.Add(slider);
            }
            else
            {
                Debug.LogWarning("Slider not found: " + sliderName);
            }
        }

        // Sets max value on UI sliders
        for (int i = 0; i < allCDSliders.Count; i++)
        {
            allCDSliders[i].maxValue = weapon.allWeapons[(ArrowType)i].Cooldown;
        }
    }

    public void Update()
    {
        //Weapon cooldown UI update
        for (int i = 0; i < allCDSliders.Count; i++)
        {
            //allCDSliders[i].value = weapon.weaponCDTimers[i];
        }
    }


    public void SetMaxHealth(float maxHealth)
    {
        slider.minValue = 0;
        slider.maxValue = maxHealth;
    }

    public void SetHealth(float currentHealth)
    {
        slider.value = currentHealth;
        text.text = $"{currentHealth}";
    }

    public void UpdateXP(int currentXP)
    {
        xpCounter.text = "Experience: " + currentXP;
    }

    //Create damage visual above player
    public void DamageVisual(float damage)
    {
        Vector3 posOffset = transform.position + new Vector3(0, 3, 0);
        GameObject damageVisual = Instantiate(damageVisualObject, posOffset, Quaternion.identity);

        //Change damage visual text to match damage done
        TMPro.TextMeshProUGUI damageVisualText = damageVisual.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        damageVisualText.text = damage.ToString();
    }

    public void DamageVisual(float damage, string type)
    {
        Vector3 posOffset = transform.position + new Vector3(0, 3, 0);
        GameObject damageVisual = Instantiate(damageVisualObject, posOffset, Quaternion.identity);

        //Change damage visual text to match damage done
        TMPro.TextMeshProUGUI damageVisualText = damageVisual.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        damageVisualText.text = damage.ToString();

        if (type == "fire")
        {
            //Hexadecimal FF9500
            damageVisualText.color = new Color(1f, 0.584f, 0f);
        }
        if (type == "ice")
        {
            //Hexadecimal 00FFF9
            damageVisualText.color = new Color(0f, 1f, 0.976f);
        }
    }
}