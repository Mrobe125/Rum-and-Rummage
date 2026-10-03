using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Combat/Weapon")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public float damage = 25f;
    public float attackRange = 2f;
    public float attackCooldown = 0.5f;
}
