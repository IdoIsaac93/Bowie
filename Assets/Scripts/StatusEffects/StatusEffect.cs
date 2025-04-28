using UnityEngine;

public abstract class StatusEffect
{
    public float duration;
    protected float timer;
    public float Timer { get { return timer; } set { timer = value; } }

    public virtual void ApplyEffect(GameObject target) { }
    public virtual void UpdateEffect(GameObject target) { }
    public virtual void EndEffect(GameObject target) { }
}