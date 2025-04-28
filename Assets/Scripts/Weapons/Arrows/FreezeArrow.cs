using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FreezeArrow : Arrow
{
    //Status effect
    [Header("Freeze Effect")]
    [SerializeField] private float slowAmount = 2f;
    [SerializeField] private float slowDuration = 3f;

    //Properties
    public float SlowAmount { get => slowAmount; set => slowAmount = value; }
    public float SlowDuration { get => slowDuration; set => slowDuration = value; }

    protected override void HandleCollision(Collider collider)
    {
        if (!isFlying) return;

        float calculatedDamage = CalculateDamage(collider);

        ApplyDamage(collider, calculatedDamage);

        collider.TryGetComponent(out StatusEffectManager statusManager);
        if (statusManager != null)
        {
            StatusEffect freeze = new FreezeEffect(slowAmount, slowDuration);
            statusManager.AddEffect(freeze);
        }

        StickArrow(collider);
    }
}
