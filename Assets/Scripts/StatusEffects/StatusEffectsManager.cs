using System.Collections.Generic;
using UnityEngine;

public class StatusEffectManager : MonoBehaviour
{
    private List<StatusEffect> activeEffects = new();

    public List<StatusEffect> ActiveEffects { get =>  activeEffects; }
    void Update()
    {
        List<StatusEffect> effectsToRemove = new();

        foreach (StatusEffect effect in activeEffects)
        {
            effect.UpdateEffect(gameObject);
            if (effect.Timer >= effect.duration)
            {
                effect.EndEffect(gameObject);
                effectsToRemove.Add(effect);
            }
        }

        foreach (StatusEffect effect in effectsToRemove)
        {
            activeEffects.Remove(effect);
        }
    }

    public void AddEffect(StatusEffect newEffect)
    {
        // If same type of effect already exists, refresh it instead of stacking
        StatusEffect existing = activeEffects.Find(e => e.GetType() == newEffect.GetType());
        if (existing != null)
        {
            existing.Timer = 0f;
            existing.duration = newEffect.duration;
        }
        else
        {
            newEffect.ApplyEffect(gameObject);
            activeEffects.Add(newEffect);
        }
    }
}