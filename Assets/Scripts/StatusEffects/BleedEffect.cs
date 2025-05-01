using UnityEngine;

public class BleedEffect : StatusEffect
{
    private DamageInfo damageInfo;
    private float tickInterval = 0.5f;
    private float tickTimer = 0f;

    public BleedEffect(float damage, float duration, ArrowheadType type)
    {
        this.type = StatusEffectType.Bleed;
        damageInfo = new DamageInfo(damage, false, false, false, type);
        this.duration = duration;
    }

    public override void UpdateEffect(GameObject target)
    {
        base.UpdateEffect(target);
        tickTimer += Time.deltaTime;

        if (tickTimer >= tickInterval)
        {
            target.TryGetComponent(out IDamageable damageable);
            damageable?.TakeDamage(damageInfo);
            tickTimer = 0f;
        }
    }
}