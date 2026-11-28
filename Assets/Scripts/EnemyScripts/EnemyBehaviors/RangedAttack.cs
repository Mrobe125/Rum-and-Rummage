using UnityEngine;

public class RangedAttack : MonoBehaviour
{
    [Header("Projectile Settings")]
    public Transform firePoint;

    private float cooldownTimer;

    private void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

    public bool CanAttack() => cooldownTimer <= 0f;

    public void PerformRangedAttack(Transform target, EnemyStats stats)
    {
        if (!CanAttack() || stats.projectilePrefab == null) return;

        cooldownTimer = stats.attackCooldown;

        // Face the target
        Vector3 lookPos = new Vector3(target.position.x, transform.position.y, target.position.z);
        transform.LookAt(lookPos);

        // Calculate trajectory toward player's chest height
        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position + Vector3.up + transform.forward;
        Vector3 aimTarget = target.position + Vector3.up;
        Vector3 direction = (aimTarget - spawnPos).normalized;

        GameObject proj = Instantiate(stats.projectilePrefab, spawnPos, Quaternion.LookRotation(direction));

        if (proj.TryGetComponent(out Rigidbody rb))
        {
            rb.linearVelocity = direction * stats.projectileSpeed; // Use rb.velocity if on Unity 2022 or older
        }

        if (proj.TryGetComponent(out EnemyProjectile projectileScript))
        {
            projectileScript.Initialize(stats.damage);
        }
    }
}