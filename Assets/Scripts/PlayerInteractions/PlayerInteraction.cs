using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

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
    [SerializeField] private PillarPuzzleManager pillarPuzzleManager;

    private ItemObject currentItemTarget;
    private PuzzleReceptor currentReceptorTarget;
    private Pillar currentPillarTarget;

    private void Awake()
    {
        if (playerCamera == null) playerCamera = Camera.main;
        if (playerInventory == null) playerInventory = GetComponent<Inventory>();
        if (pillarPuzzleManager == null) pillarPuzzleManager = FindFirstObjectByType<PillarPuzzleManager>();

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
        Debug.Log("[PlayerInteraction] Interact input key pressed!");
        ExecuteInteraction();
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
                    if (currentItemTarget != itemObject)
                    {
                        Debug.Log($"[PlayerInteraction] Looking at item: {itemObject.gameObject.name} ({itemObject.Data.itemName})");
                    }

                    currentItemTarget = itemObject;
                    currentReceptorTarget = null;
                    currentPillarTarget = null;
                    ShowUI($"{interactPromptText} ({itemObject.Data.itemName})");
                    return;
                }
            }

            // 2. Check for Puzzle Receptors
            if (hit.collider.TryGetComponent(out PuzzleReceptor receptor))
            {
                if (!receptor.IsSolved)
                {
                    if (currentReceptorTarget != receptor)
                    {
                        Debug.Log($"[PlayerInteraction] Looking at receptor: {receptor.gameObject.name}");
                    }

                    currentReceptorTarget = receptor;
                    currentItemTarget = null;
                    currentPillarTarget = null;
                    ShowUI(receptor.GetPromptText());
                    return;
                }
            }

            // 3. Check for Pillars
            if (hit.collider.TryGetComponent(out Pillar pillar))
            {
                if (currentPillarTarget != pillar)
                {
                    Debug.Log($"[PlayerInteraction] Looking at pillar: {pillar.gameObject.name}");
                }

                currentPillarTarget = pillar;
                currentItemTarget = null;
                currentReceptorTarget = null;
                ShowUI(pillar.GetPromptText());
                return;
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
            Debug.Log($"[PlayerInteraction] Attempting to pick up object: {targetToPickUp.gameObject.name}");

            if (playerInventory != null && playerInventory.AddItem(targetToPickUp.Data))
            {
                Debug.Log($"[PlayerInteraction] SUCCESS: Successfully picked up {targetToPickUp.Data.itemName}!");
                ClearTargets();
                targetToPickUp.OnPickup();
            }
            else
            {
                Debug.LogWarning($"[PlayerInteraction] FAILED: Could not pick up {targetToPickUp.Data.itemName}. Check if Inventory reference is missing.");
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

        // Handle Pillar Rotation
        if (currentPillarTarget != null)
        {
            if (currentPillarTarget.IsRotating)
            {
                Debug.Log("[PlayerInteraction] Pillar is currently rotating. Wait until movement finishes.");
                return;
            }

            Debug.Log($"[PlayerInteraction] Rotating pillar: {currentPillarTarget.gameObject.name}");
            Pillar rotatedPillar = currentPillarTarget;
            rotatedPillar.Interact();

            if (pillarPuzzleManager != null)
            {
                StartCoroutine(ValidatePuzzleAfterRotation(rotatedPillar));
            }

            return;
        }

        Debug.Log("[PlayerInteraction] Interact pressed, but no target was found in range.");
    }

    private IEnumerator ValidatePuzzleAfterRotation(Pillar pillar)
    {
        // Wait until pillar completes its rotation coroutine before checking puzzle state
        yield return new WaitUntil(() => !pillar.IsRotating);

        if (pillarPuzzleManager != null)
        {
            pillarPuzzleManager.CheckPuzzleState();
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
        currentPillarTarget = null;
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