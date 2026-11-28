using UnityEngine;

[RequireComponent(typeof(RangedAttack))]
public class RangedEnemyAI : EnemyAIController
{
    private RangedAttack rangedAttack;

    protected override void Awake()
    {
        base.Awake();
        rangedAttack = GetComponent<RangedAttack>();
    }

    protected override void HandleAttack()
    {
        // Ranged enemies must re-chase if a wall blocks their shot
        if (!CanSeePlayer())
        {
            currentState = AIState.Chase;
            return;
        }

        rangedAttack.PerformRangedAttack(player, stats);
        base.HandleAttack(); // Checks if player moved beyond attackRange
    }
}