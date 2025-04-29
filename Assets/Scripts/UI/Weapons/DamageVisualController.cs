using TMPro;
using UnityEngine;

public class DamageVisualController : MonoBehaviour
{
    [Header("Movement and Lifetime")]
    [SerializeField] private float timer = 0;
    [SerializeField] private float timeUntillDestroy = 0.5f;
    [SerializeField] private float risingSpeed = 5f;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI damageText;

    void Update()
    {
        float verticalMovement = risingSpeed * Time.deltaTime;
        transform.position += Vector3.up * verticalMovement;
        timer += Time.deltaTime;
        if (timer >= timeUntillDestroy)
        {
            gameObject.SetActive(false);
        }
    }

    public void Setup(float damageAmount, bool isCritical, ArrowheadType damageType, bool isShieldHit, bool isWeakspotHit)
    {
        timer = 0; // Reset timer
        string text = damageAmount.ToString();

        // Handle Shield/Weakspot extra info
        if (isShieldHit)
            text += " Shielded";
        if (isWeakspotHit)
            text += " Weakspot";

        // Set Color Priority
        if (damageAmount < 0)
        {
            text += " Healed";
            damageAmount = Mathf.Abs(damageAmount);
            damageText.color = Color.green;
        }
        else if (isCritical)
        {
            damageText.color = Color.yellow;
        }
        else
        {
            switch (damageType)
            {
                case ArrowheadType.Negative:
                    damageText.color = new Color(0.5f, 0f, 0.5f); // purple
                    break;
                case ArrowheadType.Positive:
                    damageText.color = Color.blue;
                    break;
                case ArrowheadType.Piercing:
                    damageText.color = Color.black;
                    break;
            }
        }

        damageText.text = text;
    }
}
