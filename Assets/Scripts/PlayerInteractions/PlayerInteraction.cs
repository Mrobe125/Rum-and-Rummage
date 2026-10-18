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
        if (interactAction != null && interactAction.action != null)
        {
            interactAction.action.Enable();
            interactAction.action.performed += OnInteract;
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

    private void OnInteract(InputAction.CallbackContext context)
    {
        // 1. Debug input detection
        Debug.Log("[PlayerInteraction] Interact input key pressed!");
        ExecuteInteraction();
    }

    private void DetectInteractable()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayer))
        {
            // Check for Item Pickups
            if (hit.collider.TryGetComponent(out ItemObject itemObject))
            {
                if (itemObject.Data != null)
                {
                    // Log only when target changes to prevent spamming Update logs every frame
                    if (currentItemTarget != itemObject)
                    {
                        Debug.Log($"[PlayerInteraction] Raycast looking at interactable item: {itemObject.gameObject.name} ({itemObject.Data.itemName})");
                    }

                    currentItemTarget = itemObject;
                    currentReceptorTarget = null;
                    ShowUI($"{interactPromptText} ({itemObject.Data.itemName})");
                    return;
                }
            }

            // Check for Puzzle Receptors
            if (hit.collider.TryGetComponent(out PuzzleReceptor receptor))
            {
                if (!receptor.IsSolved)
                {
                    if (currentReceptorTarget != receptor)
                    {
                        Debug.Log($"[PlayerInteraction] Raycast looking at receptor: {receptor.gameObject.name}");
                    }

                    currentReceptorTarget = receptor;
                    currentItemTarget = null;
                    ShowUI(receptor.GetPromptText());
                    return;
                }
            }
        }

        ClearTargets();
    }

    private void ExecuteInteraction()
    {
        // Handle Item Pickup
        if (currentItemTarget != null)
        {
            ItemObject targetToPickUp = currentItemTarget;

            // 2. Debug attempted grab
            Debug.Log($"[PlayerInteraction] Attempting to pick up object: {targetToPickUp.gameObject.name}");

            if (playerInventory != null && playerInventory.AddItem(targetToPickUp.Data))
            {
                // 3. Debug pickup success
                Debug.Log($"[PlayerInteraction] SUCCESS: Successfully picked up {targetToPickUp.Data.itemName}!");
                ClearTargets();
                targetToPickUp.OnPickup();
            }
            else
            {
                // 3. Debug pickup failure
                Debug.LogWarning($"[PlayerInteraction] FAILED: Could not pick up {targetToPickUp.Data.itemName}. Check if Inventory reference is missing or full.");
            }
            return;
        }

        // Handle Puzzle Placement
        if (currentReceptorTarget != null)
        {
            Debug.Log($"[PlayerInteraction] Attempting to interact with receptor: {currentReceptorTarget.gameObject.name}");

            if (currentReceptorTarget.TryActivate(playerInventory))
            {
                Debug.Log($"[PlayerInteraction] SUCCESS: Activated receptor {currentReceptorTarget.gameObject.name}!");
                ClearTargets();
            }
            else
            {
                Debug.LogWarning($"[PlayerInteraction] FAILED: Could not activate receptor {currentReceptorTarget.gameObject.name}.");
            }
            return;
        }

        Debug.Log("[PlayerInteraction] Interact pressed, but no target object or receptor was found in range.");
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