using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BleedArrow : Arrow
{
    //Status effect
    [Header("Bleed Effect")]
    [SerializeField] private float bleedDamage = 1f;
    [SerializeField] private float bleedDuration = 3f;

    //Properties
    public float BleedDamage { get => bleedDamage; set => bleedDamage = value; }
    public float BleedDuration { get => bleedDuration; set => bleedDuration = value; }
    protected override void HandleCollision(Collider collider)
    {
        if (!isFlying) return;

        float calculatedDamage = CalculateDamage(collider);

        ApplyDamage(collider, calculatedDamage);

        collider.TryGetComponent(out StatusEffectManager statusManager);
        if (statusManager != null)
        {
            StatusEffect bleed = new BleedEffect(bleedDamage, bleedDuration, Arrowhead);
            statusManager.AddEffect(bleed);
        }

        StickArrow(collider);
    }
}
