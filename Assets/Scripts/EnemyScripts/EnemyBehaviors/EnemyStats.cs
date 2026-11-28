using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyStats", menuName = "Enemies/Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    [Header("Identity & Behavior")]
    public string enemyName = "Pirate Grunt";
    public bool canPatrol = true; // False for Charging Beasts

    [Header("Vitals & Movement")]
    public float maxHealth = 100f;
    public float moveSpeed = 3.5f;

    [Header("Detection & Ranges")]
    public float detectionRange = 15f;
    public float attackRange = 2f;     // ~2f for Melee, ~12-18f for Ranged
    public float searchDuration = 4f;

    [Header("Combat")]
    public float damage = 15f;
    public float attackCooldown = 1.5f; // Lower for Daggers, higher for Muskets

    [Header("Ranged Specific (Optional)")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 20f;
}