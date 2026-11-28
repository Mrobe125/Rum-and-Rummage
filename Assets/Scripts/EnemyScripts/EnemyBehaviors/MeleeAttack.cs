using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [Header("Hitbox Settings")]
    public Transform attackPoint;
    public float hitRadius = 1.2f;
    public LayerMask playerLayer;

    private float cooldownTimer;

    private void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

    public bool CanAttack() => cooldownTimer <= 0f;

    public void PerformSwing(Transform target, EnemyStats stats)
    {
        if (!CanAttack()) return;

        cooldownTimer = stats.attackCooldown;

        // Rotate smoothly toward player on the horizontal plane
        Vector3 lookPos = new Vector3(target.position.x, transform.position.y, target.position.z);
        transform.LookAt(lookPos);

        // Trigger Animator here if needed: animator.SetTrigger("Swing");
        ApplyHit(stats.damage);
    }

    // Can also be called via an Animation Event at the exact frame the blade connects
    public void ApplyHit(float damage)
    {
        Vector3 origin = attackPoint != null ? attackPoint.position : transform.position + transform.forward;
        Collider[] hits = Physics.OverlapSphere(origin, hitRadius, playerLayer);

        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 origin = attackPoint != null ? attackPoint.position : transform.position + transform.forward;
        Gizmos.DrawWireSphere(origin, hitRadius);
    }
}