using TMPro;
using UnityEngine;

public class DamageVisualController : Singleton<DamageVisualController>
{
    [Header("References")]
    private static GameObject damageVisualPrefab;
    private static DamageVisual controller;

    public static void ShowDamageVisual(DamageInfo info, Vector3 originPosition)
    {
        if (damageVisualPrefab == null)
        {
            damageVisualPrefab = Resources.Load<GameObject>("DamageVisual/DamageVisual");
            if (damageVisualPrefab == null)
            {
                Debug.LogError("DamageVisual prefab not found in Resources.");
                return;
            }
        }

        Vector3 spawnPosition = originPosition + new Vector3(0, 3, 0);
        GameObject visualInstance = Instantiate(damageVisualPrefab, spawnPosition, Quaternion.identity);
        controller = visualInstance.GetComponent<DamageVisual>();
        Setup(info);
    }

    public static void Setup(DamageInfo info)
    {
        controller.Timer = 0; // Reset timer
        string text = info.Amount.ToString();

        if (info.IsShieldHit)
            text += " Shielded";
        if (info.IsWeakspot)
            text += " Weakspot";

        if (info.Amount < 0)
        {
            text += " Healed";
            controller.DamageText.color = Color.green;
        }
        else if (info.IsCritical)
        {
            controller.DamageText.color = Color.yellow;
        }
        else
        {
            switch (info.DamageType)
            {
                case ArrowheadType.Negative:
                    controller.DamageText.color = new Color(0.5f, 0f, 0.5f); // Purple
                    break;
                case ArrowheadType.Positive:
                    controller.DamageText.color = Color.blue;
                    break;
                case ArrowheadType.Piercing:
                    controller.DamageText.color = Color.black;
                    break;
            }
        }

        controller.DamageText.text = text;
    }
}