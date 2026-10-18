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
        if (interactAction != null && interactAction.action != null)
        {
            // Enable the entire Action Map if available, as well as the action itself
            interactAction.action.actionMap?.Enable();
            interactAction.action.Enable();
            interactAction.action.performed += OnInteract;
        }
        else
        {
            Debug.LogError("PlayerInteraction: Interact Action Reference is missing or unassigned in the Inspector!", this);
        }
    }

    private void OnDisable()
    {
        if (interactAction != null && interactAction.action != null)
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
                if (itemObject.Data == null)
                {
                    Debug.LogWarning($"PlayerInteraction: ItemObject '{itemObject.name}' is missing an ItemData asset!", itemObject);
                    currentTarget = null;
                    SetUIVisible(false);
                    return;
                }

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

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (currentTarget == null) return;

        if (playerInventory == null)
        {
            Debug.LogError("PlayerInteraction: Inventory component is missing on the player!", this);
            return;
        }

        if (playerInventory.AddItem(currentTarget.Data))
        {
            currentTarget.OnPickup();
            currentTarget = null;
            SetUIVisible(false);
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