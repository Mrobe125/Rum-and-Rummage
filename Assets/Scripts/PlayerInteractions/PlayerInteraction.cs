using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactableLayer;

    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI interactUI;
    [SerializeField] private string interactPromptText = "Press E to Interact";

    [Header("Input")]
    [SerializeField] private InputActionReference interactAction;

    [Header("Dependencies")]
    [SerializeField] private Inventory playerInventory;

    private ItemObject currentTarget;

    private void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (playerInventory == null)
            playerInventory = GetComponent<Inventory>();

        SetUIVisible(false);
    }

    private void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.action.Enable();
            interactAction.action.performed += OnInteract;
        }
    }

    private void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.action.performed -= OnInteract;
            interactAction.action.Disable();
        }
    }

    private void Update()
    {
        DetectInteractable();
    }

    private void DetectInteractable()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayer))
        {
            if (hit.collider.TryGetComponent(out ItemObject itemObject))
            {
                currentTarget = itemObject;
                if (interactUI != null)
                {
                    interactUI.text = $"{interactPromptText} ({itemObject.Data.itemName})";
                }
                SetUIVisible(true);
                return;
            }
        }

        currentTarget = null;
        SetUIVisible(false);
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (currentTarget != null)
        {
            if (playerInventory.AddItem(currentTarget.Data))
            {
                currentTarget.OnPickup();
                currentTarget = null;
                SetUIVisible(false);
            }
        }
    }

    private void SetUIVisible(bool visible)
    {
        if (interactUI != null)
        {
            interactUI.gameObject.SetActive(visible);
        }
    }
}