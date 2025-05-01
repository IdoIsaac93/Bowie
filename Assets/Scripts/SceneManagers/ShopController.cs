using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    WeaponController weapon;
    PlayerStats stats;
    PlayerUI ui;

    BasicArrow basicArrow;
    PierceArrow pierceArrow;
    BombArrow bombArrow;
    RepeatingArrow repeatingArrow;
    GuidedArrow guidedArrow;
    BleedArrow bleedArrow;
    FreezeArrow freezeArrow;

    bool isPaused = false;

    Transform shopUI;
    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        weapon = playerObject.GetComponent<WeaponController>();
        stats = playerObject.GetComponent<PlayerStats>();
        ui = playerObject.GetComponent<PlayerUI>();

        basicArrow = (BasicArrow)weapon.allWeapons[ArrowType.Basic];
        pierceArrow = (PierceArrow)weapon.allWeapons[ArrowType.Piercing];
        bombArrow = (BombArrow)weapon.allWeapons[ArrowType.Bomb];
        repeatingArrow = (RepeatingArrow)weapon.allWeapons[ArrowType.Repeating];
        guidedArrow = (GuidedArrow)weapon.allWeapons[ArrowType.Guided];
        bleedArrow = (BleedArrow)weapon.allWeapons[ArrowType.Bleed];
        freezeArrow = (FreezeArrow)weapon.allWeapons[ArrowType.Freeze];

        shopUI = transform.GetChild(0);
        shopUI.gameObject.SetActive(false);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                Pause();
            }
            else
            {
                Unpause();
            }
        }
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0;
        shopUI.gameObject.SetActive(true);
    }

    public void Unpause()
    {
        isPaused = false;
        Time.timeScale = 1.0f;
        shopUI.gameObject.SetActive(false);
    }

    //Player
    public void DrawSpeed()
    {
        int price = 5;
        if (stats.ExperiencePoints >= price)
        {
            Pay(price);

            stats.DrawSpeed += 0.1f;
        }
    }

    public void AimSpeed()
    {
        int price = 5;
        if (stats.ExperiencePoints >= price)
        {
            Pay(price);

            stats.AimSpeed += 0.5f;
        }
    }

    public void MaxDraw()
    {
        int price = 5;
        if (stats.ExperiencePoints >= price)
        {
            Pay(price);

            stats.MaxLaunchVelocity += 2;
        }
    }

    public void MaxHealth()
    {
        int price = 5;
        if (stats.ExperiencePoints >= price)
        {
            Pay(price);

            stats.MaxHealth += 5;
            stats.CurrentHealth += 5;
            ui.SetMaxHealth(stats.MaxHealth);
        }
    }

    public void MovementSpeed()
    {
        int price = 5;
        if (stats.ExperiencePoints >= price)
        {
            Pay(price);

            stats.MovementSpeed += 2;
        }
    }

    public void CriticalMultiplier()
    {
        int price = 5;
        if (stats.ExperiencePoints >= price)
        {
            Pay(price);

            stats.CriticalMultiplier += 0.2f;
        }
    }

    public void DodgeCooldown()
    {
        int price = 5;
        if (stats.ExperiencePoints >= price)
        {
            Pay(price);

            stats.DodgeCooldown -= 0.5f;
        }
    }

    //Basic arrow
    public void BasicArrowDamage()
    {
        int price = 5;
        if (stats.ExperiencePoints >= price)
        {
            Pay(price);

            basicArrow.MinDamage += 2;
            basicArrow.MaxDamage += 2;
        }
    }

    public void BasicArrowCooldown()
    {
        int price = 5;
        if (stats.ExperiencePoints >= price)
        {
            Pay(price);

            basicArrow.Cooldown -= 0.2f;
            //weapon.allCDs[0] = weapon.WeaponCD1;
            ui.allCDSliders[(int)ArrowType.Basic].maxValue = basicArrow.Cooldown;
        }
    }

    //Pierce arrow
    public void PierceArrowDamage()
    {
        int price = 5;
        if (stats.ExperiencePoints >= price)
        {
            Pay(price);

            pierceArrow.MinDamage += 2;
            pierceArrow.MaxDamage += 2;
        }
    }

    public void PierceArrowPierceAmount()
    {
        int price = 5;
        if (stats.ExperiencePoints >= price)
        {
            Pay(price);

            pierceArrow.PierceAmount += 1;
        }
    }
    public void PierceArrowCooldown()
    {
        int price = 5;
        if (stats.ExperiencePoints >= price)
        {
            Pay(price);

            pierceArrow.Cooldown -= 0.2f;
            //weapon.allCDs[1] = weapon.WeaponCD2;
            ui.allCDSliders[(int)ArrowType.Piercing].maxValue = pierceArrow.Cooldown;
        }
    }

    //Bomb arrow
    public void BombArrowDamage()
    {
        int price = 5;
        if (stats.ExperiencePoints >= price)
        {
            Pay(price);

            bombArrow.MaxDamage += 2;
            bombArrow.MinDamage += 2;
        }
    }

    public void BombArrowRadius()
    {
        int price = 5;
        if (stats.ExperiencePoints >= price)
        {
            Pay(price);

            bombArrow.ExplosionRadius += 0.5f;
        }
    }
    public void BombArrowCooldown()
    {
        int price = 5;
        if (stats.ExperiencePoints >= price)
        {
            Pay(price);

            bombArrow.Cooldown -= 0.2f;
            //weapon.allCDs[2] = weapon.WeaponCD3;
            ui.allCDSliders[(int)ArrowType.Bomb].maxValue = bombArrow.Cooldown;
        }
    }

    //Repeating arrow
    public void RepeatingArrowDamage()
    {
        int price = 5;
        if (stats.ExperiencePoints >= price)
        {
            Pay(price);

            repeatingArrow.MinDamage += 2;
            repeatingArrow.MaxDamage += 2;
        }
    }
    public void RepeatingNumberOfShots()
    {
        int price = 5;
        if (stats.ExperiencePoints >= price)
        {
            Pay(price);
            repeatingArrow.AdditionalShots += 1;
        }
    }
    public void RepeatingArrowCooldown()
    {
        int price = 5;
        if (stats.ExperiencePoints >= price)
        {
            Pay(price);

            repeatingArrow.Cooldown -= 0.2f;
            //weapon.allCDs[3] = weapon.WeaponCD4;
            ui.allCDSliders[(int)ArrowType.Repeating].maxValue = repeatingArrow.Cooldown;
        }
    }

    public void GuidedArrowDamage()
    {
        int price = 5;
        if (stats.ExperiencePoints >= price)
        {
            Pay(price);

            guidedArrow.MinDamage += 2;
            guidedArrow.MaxDamage += 2;
        }
    }

    public void GuidedArrowAccuracy()
    {
        int price = 5;
        if (stats.ExperiencePoints >= price)
        {
            Pay(price);

            guidedArrow.GuideForce += 2f;
        }
    }

    public void GuidedArrowCooldown()
    {
        int price = 5;
        if (stats.ExperiencePoints >= price)
        {
            Pay(price);

            guidedArrow.Cooldown -= 0.2f;
            //weapon.allCDs[4] = weapon.WeaponCD5;
            ui.allCDSliders[(int)ArrowType.Guided].maxValue = guidedArrow.Cooldown;
        }
    }

    //Bleed arrow
    public void BleedArrowDamage()
    {
        int price = 5;
        if (stats.ExperiencePoints >= price)
        {
            Pay(price);

            bleedArrow.MinDamage += 2;
            bleedArrow.MaxDamage += 2;
        }
    }

    public void BleedArrowBurnDamage()
    {
        int price = 5;
        if (stats.ExperiencePoints >= price)
        {
            Pay(price);

            bleedArrow.BleedDamage += 1;
        }
    }

    public void BleedArrowBurnDuration()
    {
        int price = 5;
        if (stats.ExperiencePoints >= price)
        {
            Pay(price);

            bleedArrow.BleedDuration += 0.5f;
        }
    }

    public void BleedArrowCooldown()
    {
        int price = 5;
        if (stats.ExperiencePoints >= price)
        {
            Pay(price);

            bleedArrow.Cooldown -= 0.2f;
            ui.allCDSliders[(int)ArrowType.Bleed].maxValue = bleedArrow.Cooldown;
        }
    }

    //Freeze arrow
    public void FreezeArrowDamage()
    {
        int price = 5;
        if (stats.ExperiencePoints >= price)
        {
            Pay(price);

            freezeArrow.MinDamage += 2;
            freezeArrow.MaxDamage += 2;
        }
    }

    public void FreezeArrowSlowAmount()
    {
        int price = 5;
        if (stats.ExperiencePoints >= price)
        {
            Pay(price);

            freezeArrow.SlowAmount += 0.5f;
        }
    }

    public void FreezeArrowSlowDuration()
    {
        int price = 5;
        if (stats.ExperiencePoints >= price)
        {
            Pay(price);

            freezeArrow.SlowDuration += 0.5f;
        }
    }

    public void FreezeArrowCooldown()
    {
        int price = 5;
        if (stats.ExperiencePoints >= price)
        {
            Pay(price);

            freezeArrow.Cooldown -= 0.2f;
            ui.allCDSliders[(int)ArrowType.Freeze].maxValue = freezeArrow.Cooldown;
        }
    }

    //Pay
    void Pay(int price)
    {
        stats.ExperiencePoints -= price;
    }
}
