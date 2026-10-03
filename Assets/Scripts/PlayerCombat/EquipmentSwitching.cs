
using UnityEngine;
using UnityEngine.InputSystem;

public class EquipmentSwitching : MonoBehaviour
{
    [Header("Equipment Slots")]
    [SerializeField] private GameObject weaponSlot;
    [SerializeField] private GameObject hookSlot;

    [Header("Input")]
    [SerializeField] private InputActionReference weaponAction;
    [SerializeField] private InputActionReference hookAction;
    [SerializeField] private InputActionReference swapAction;

    private int currentSlot = 1;

    public int CurrentSlot => currentSlot;

    private void OnEnable()
    {
        weaponAction.action.Enable();
        hookAction.action.Enable();
        swapAction.action.Enable();

        weaponAction.action.performed += EquipWeapon;
        hookAction.action.performed += EquipHook;
        swapAction.action.performed += SwapEquipment;
    }

    private void OnDisable()
    {
        weaponAction.action.performed -= EquipWeapon;
        hookAction.action.performed -= EquipHook;
        swapAction.action.performed -= SwapEquipment;

        weaponAction.action.Disable();
        hookAction.action.Disable();
        swapAction.action.Disable();
    }

    private void Start()
    {
        EquipSlot(1);
    }

    private void EquipWeapon(InputAction.CallbackContext context)
    {
        EquipSlot(1);
    }

    private void EquipHook(InputAction.CallbackContext context)
    {
        EquipSlot(2);
    }

    private void SwapEquipment(InputAction.CallbackContext context)
    {
        EquipSlot(currentSlot == 1 ? 2 : 1);
    }

    private void EquipSlot(int slot)
    {
        currentSlot = slot;

        weaponSlot.SetActive(currentSlot == 1);
        hookSlot.SetActive(currentSlot == 2);
    }
}