using UnityEngine;

[RequireComponent(typeof(MeleeAttack))]
public class MeleeEnemyAI : EnemyAIController
{
    private MeleeAttack meleeAttack;

    protected override void Awake()
    {
        base.Awake();
        meleeAttack = GetComponent<MeleeAttack>();
    }

    protected override void HandleAttack()
    {
        meleeAttack.PerformSwing(player, stats);
        base.HandleAttack(); // Checks if player ran out of melee range
    }
}