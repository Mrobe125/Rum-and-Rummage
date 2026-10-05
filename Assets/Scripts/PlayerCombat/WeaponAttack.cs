using UnityEngine;

public class WeaponAttack : MonoBehaviour
{
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private LayerMask enemyLayer;

    public float AttackCooldown => weaponData.attackCooldown;

    public void Attack()
    {
        if (Physics.Raycast(
            playerCamera.transform.position,
            playerCamera.transform.forward,
            out RaycastHit hit,
            weaponData.attackRange,
            enemyLayer))
        {
            EnemyHealth enemy = hit.collider.GetComponentInParent<EnemyHealth>();

            if (enemy != null)
                enemy.TakeDamage(weaponData.damage);
        }
    }
}