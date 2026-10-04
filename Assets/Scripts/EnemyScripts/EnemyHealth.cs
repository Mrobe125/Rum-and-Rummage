using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private Renderer enemyRenderer;
    [SerializeField] private float damageFlashDuration = 0.2f;

    private float currentHealth;
    private Color originalColor;
    private Coroutine damageFlashCoroutine;

    private void Awake()
    {
        currentHealth = enemyData.maxHealth;

        if (enemyRenderer == null)
            enemyRenderer = GetComponentInChildren<Renderer>();

        if (enemyRenderer != null)
            originalColor = enemyRenderer.material.color;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        Debug.Log(enemyData.enemyName + " took " + damage + " damage.");

        if (damageFlashCoroutine != null)
            StopCoroutine(damageFlashCoroutine);

        damageFlashCoroutine = StartCoroutine(DamageFlash());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator DamageFlash()
    {
        enemyRenderer.material.color = Color.red;

        yield return new WaitForSeconds(damageFlashDuration);

        enemyRenderer.material.color = originalColor;

        damageFlashCoroutine = null;
    }

    private void Die()
    {
        Debug.Log(enemyData.enemyName + " has died.");
        Destroy(gameObject);
    }
}

