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

    [Header("Input Setup")]
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private bool useDirectKey = false;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Dependencies")]
    [SerializeField] private Inventory playerInventory;

    private ItemObject currentItemTarget;
    private PuzzleReceptor currentReceptorTarget;

    private void Awake()
    {
        if (playerCamera == null) playerCamera = Camera.main;
        if (playerInventory == null) playerInventory = GetComponent<Inventory>();
        SetUIVisible(false);
    }

    private void OnEnable()
    {
        if (!useDirectKey && interactAction != null && interactAction.action != null)
        {
            interactAction.action.actionMap?.Enable();
            interactAction.action.Enable();
            interactAction.action.performed += OnInteractInput;
        }
    }

    private void OnDisable()
    {
        if (!useDirectKey && interactAction != null && interactAction.action != null)
        {
            interactAction.action.performed -= OnInteractInput;
            interactAction.action.Disable();
        }
    }

    private void Update()
    {
        DetectInteractable();

        if (useDirectKey && Input.GetKeyDown(interactKey))
        {
            ExecuteInteraction();
        }
    }

    private void DetectInteractable()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayer))
        {
            // 1. Check for Item Pickups
            if (hit.collider.TryGetComponent(out ItemObject itemObject))
            {
                if (itemObject.Data != null)
                {
                    currentItemTarget = itemObject;
                    currentReceptorTarget = null;
                    ShowUI($"{interactPromptText} ({itemObject.Data.itemName})");
                    return;
                }
            }

            // 2. Check for Puzzle Receptors
            if (hit.collider.TryGetComponent(out PuzzleReceptor receptor))
            {
                if (!receptor.IsSolved)
                {
                    currentReceptorTarget = receptor;
                    currentItemTarget = null;
                    ShowUI(receptor.GetPromptText());
                    return;
                }
            }
        }

        ClearTargets();
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        ExecuteInteraction();
    }

    private void ExecuteInteraction()
    {
        // Handle Item Pickup
        if (currentItemTarget != null)
        {
            if (playerInventory.AddItem(currentItemTarget.Data))
            {
                ItemObject target = currentItemTarget;
                ClearTargets();
                target.OnPickup();
            }
            return;
        }

        // Handle Puzzle Placement
        if (currentReceptorTarget != null)
        {
            if (currentReceptorTarget.TryActivate(playerInventory))
            {
                ClearTargets();
            }
        }
    }

    private void ShowUI(string text)
    {
        if (interactUI != null)
        {
            interactUI.text = text;
            interactUI.gameObject.SetActive(true);
        }
    }

    private void ClearTargets()
    {
        currentItemTarget = null;
        currentReceptorTarget = null;
        SetUIVisible(false);
    }

    private void SetUIVisible(bool visible)
    {
        if (interactUI != null)
        {
            interactUI.gameObject.SetActive(visible);
        }
    }
}