using UnityEngine;

public class FreezeEffect : StatusEffect
{
    private float slowMultiplier;
    private bool applied = false;

    public FreezeEffect(float slowAmount, float duration)
    {
        this.slowMultiplier = slowAmount;
        this.duration = duration;
    }

    public override void ApplyEffect(GameObject target)
    {
        target.TryGetComponent(out EnemyController enemyController);
        if (enemyController != null)
        {
            enemyController.MovementSpeed /= slowMultiplier;
            applied = true;
        }
    }

    public override void EndEffect(GameObject target)
    {
        if (!applied) return;

        target.TryGetComponent(out EnemyController enemyController);
        if (enemyController != null)
        {
            enemyController.MovementSpeed *= slowMultiplier;
        }
    }
}