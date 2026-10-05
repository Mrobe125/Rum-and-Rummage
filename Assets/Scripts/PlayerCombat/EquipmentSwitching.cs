
using UnityEngine;
using UnityEngine.InputSystem;

public class EquipmentSwap : MonoBehaviour
{
    [Header("Equipment")]
    [SerializeField] private GameObject sword;
    [SerializeField] private GameObject hook;

    [Header("Combat")]
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private HookThrow hookThrow;

    [Header("Input")]
    [SerializeField] private InputActionReference swapAction;

    private bool hookEquipped;

    private void OnEnable()
    {
        swapAction.action.Enable();
        swapAction.action.performed += OnSwap;
    }

    private void OnDisable()
    {
        swapAction.action.performed -= OnSwap;
        swapAction.action.Disable();
    }

    private void Start()
    {
        EquipSword();
    }

    private void OnSwap(InputAction.CallbackContext context)
    {
        SwapEquipment();
    }

    public void SwapEquipment()
    {
        if (hookEquipped)
            EquipSword();
        else
            EquipHook();
    }

    private void EquipSword()
    {
        hookEquipped = false;

        sword.SetActive(true);
        hook.SetActive(false);

        playerCombat.enabled = true;
        hookThrow.enabled = false;
    }

    private void EquipHook()
    {
        hookEquipped = true;

        sword.SetActive(false);
        hook.SetActive(true);

        playerCombat.enabled = false;
        hookThrow.enabled = true;
    }
}