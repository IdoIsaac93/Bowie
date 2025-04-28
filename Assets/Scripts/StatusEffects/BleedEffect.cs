using UnityEngine;

public class BleedEffect : StatusEffect
{
    private float damagePerTick;
    private float tickInterval = 0.5f;
    private float tickTimer = 0f;
    private ArrowheadType damageType;

    public BleedEffect(float damage, float duration, ArrowheadType type)
    {
        this.damagePerTick = damage;
        this.duration = duration;
        this.damageType = type;
    }

    public override void UpdateEffect(GameObject target)
    {
        timer += Time.deltaTime;
        tickTimer += Time.deltaTime;

        if (tickTimer >= tickInterval)
        {
            target.TryGetComponent(out IDamageable damageable);
            damageable?.TakeDamage(damagePerTick, damageType);
            tickTimer = 0f;
        }
    }
}