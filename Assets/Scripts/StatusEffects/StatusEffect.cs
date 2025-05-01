using UnityEngine;

public abstract class StatusEffect
{
    [SerializeField] protected StatusEffectType type;
    [SerializeField] protected float duration;
    protected float timer;

    //Properties
    public StatusEffectType Type { get => type; }
    public float Duration { get => duration; set => duration = value; }
    public float Timer { get => timer; set => timer = value; }

    public virtual void ApplyEffect(GameObject target) { }
    public virtual void UpdateEffect(GameObject target)
    {
        timer += Time.deltaTime;
    }
    public virtual void EndEffect(GameObject target) { }
}