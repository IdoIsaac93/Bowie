using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BleedArrow : Arrow
{
    //Status effect
    [Header("Bleed Effect")]
    [SerializeField] private float bleedDamage = 1f;
    [SerializeField] private float bleedDuration = 3f;
    [SerializeField] private float weakspotDamageMod = 1.5f;

    //Properties
    public float BleedDamage { get => bleedDamage; set => bleedDamage = value; }
    public float BleedDuration { get => bleedDuration; set => bleedDuration = value; }
    protected override void HandleCollision(Collider collider)
    {
        base.HandleCollision(collider);

        //Don't apply status effect if hit shield
        if (isShield) { return; }

        //Deal extra bleed damage if hit a weakspot
        float finalBleedDamage = bleedDamage;
        if(isWeakspot) { finalBleedDamage *= weakspotDamageMod; }
        _ = Mathf.Round(finalBleedDamage);

        //Find the status effect manager
        StatusEffectManager statusManager = collider.GetComponentInParent<StatusEffectManager>();
        if (statusManager != null)
        {
            //Apply the status effect
            StatusEffect bleed = new BleedEffect(finalBleedDamage, bleedDuration, Arrowhead);
            statusManager.AddEffect(bleed);
        }

    }
}
