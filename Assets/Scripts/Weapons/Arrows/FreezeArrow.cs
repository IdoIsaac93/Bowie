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
        base.HandleCollision(collider);

        StatusEffectManager statusManager = collider.GetComponentInParent<StatusEffectManager>();
        if (statusManager != null)
        {
            StatusEffect freeze = new FreezeEffect(slowAmount, slowDuration);
            statusManager.AddEffect(freeze);
        }
    }
}
