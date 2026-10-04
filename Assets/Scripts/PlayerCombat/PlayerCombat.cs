
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private InputActionReference attackAction;
    [SerializeField] private WeaponAttack weaponAttack;

    private float nextAttackTime;

    private void OnEnable()
    {
        attackAction.action.Enable();
        attackAction.action.performed += OnAttack;
    }

    private void OnDisable()
    {
        attackAction.action.performed -= OnAttack;
        attackAction.action.Disable();
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        if (weaponAttack == null)
            return;

        if (Time.time < nextAttackTime)
            return;

        weaponAttack.Attack();
        nextAttackTime = Time.time + weaponAttack.AttackCooldown;
    }
}