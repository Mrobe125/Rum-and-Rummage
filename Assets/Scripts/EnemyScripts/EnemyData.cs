
using UnityEngine;

public enum EnemyType
{
    Pirate,
    Skeleton,
    Beast
}

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Combat/Enemy")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public EnemyType enemyType;

    public float maxHealth = 100f;
    public float damage = 10f;
    public float movementSpeed = 3f;
}