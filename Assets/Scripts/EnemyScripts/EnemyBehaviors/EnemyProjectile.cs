using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private float damage;
    public float lifeTime = 5f;

    public void Initialize(float dmg)
    {
        damage = dmg;
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out IDamageable target))
        {
            target.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            Destroy(gameObject);
        }
    }
}

public interface IDamageable
{
    void TakeDamage(float amount);
}