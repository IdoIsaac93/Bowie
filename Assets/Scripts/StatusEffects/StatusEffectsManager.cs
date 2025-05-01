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
            if (effect.Timer >= effect.Duration)
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
        //Check if the enemy is immune to this status effect
        if (TryGetComponent(out EnemyHealth immunity) &&
        immunity.IsImmuneTo(newEffect.Type))
        {
            Debug.Log($"{gameObject.name} is immune to {newEffect.Type}.");
            return;
        }

        // If same type of effect already exists, refresh it instead of stacking
        StatusEffect existing = activeEffects.Find(e => e.Type == newEffect.Type);
        if (existing != null)
        {
            existing.Timer = 0f;
            existing.Duration = newEffect.Duration;
        }
        else
        {
            newEffect.ApplyEffect(gameObject);
            activeEffects.Add(newEffect);
        }
    }
}