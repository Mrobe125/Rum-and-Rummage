using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactableLayer;

    [Header("Drag & Hold Settings")]
    [Tooltip("Empty child transform under the Player Camera where carried objects float.")]
    [SerializeField] private Transform holdPoint;

    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI interactUI;
    [SerializeField] private string interactPromptText = "Press E to Interact";

    [Header("Input Setup")]
    [SerializeField] private InputActionReference interactAction;

    [Header("Dependencies")]
    [SerializeField] private Inventory playerInventory;

    // Interaction Target References
    private IInteractable currentInteractableTarget;
    private ItemObject currentItemTarget;
    private PuzzleReceptor currentReceptorTarget;
    private DraggableObject currentDraggableTarget;

    // Currently dragged object reference
    private DraggableObject activeDraggedObject;

    private void Awake()
    {
        if (playerCamera == null) playerCamera = Camera.main;
        if (playerInventory == null) playerInventory = GetComponent<Inventory>();

        // Fallback hold position if not assigned
        if (holdPoint == null && playerCamera != null)
        {
            GameObject autoHold = new GameObject("HoldPoint");
            autoHold.transform.SetParent(playerCamera.transform);
            autoHold.transform.localPosition = new Vector3(0f, -0.2f, 1.8f);
            holdPoint = autoHold.transform;
        }

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
        // If holding an object that drops itself (e.g. pulled too far), clear reference
        if (activeDraggedObject != null && !activeDraggedObject.IsBeingDragged)
        {
            activeDraggedObject = null;
        }

        DetectInteractable();
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        ExecuteInteraction();
    }

    private void DetectInteractable()
    {
        // 1. If currently dragging an object, update UI to prompt release
        if (activeDraggedObject != null)
        {
            ShowUI("Press E to Release Object");
            return;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayer))
        {
            // 2. Check for Draggable Objects
            if (hit.collider.TryGetComponent(out DraggableObject draggable))
            {
                currentDraggableTarget = draggable;
                ClearOtherTargets(exceptDraggable: true);
                ShowUI("Press E to Grab Object");
                return;
            }

            // 3. Check for Generic IInteractable objects (Pillars, Levers, Switches, Doors, etc.)
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                currentInteractableTarget = interactable;
                ClearOtherTargets(exceptInteractable: true);
                ShowUI(interactable.GetPromptText());
                return;
            }

            // 4. Check for Item Pickups
            if (hit.collider.TryGetComponent(out ItemObject itemObject))
            {
                if (itemObject.Data != null)
                {
                    currentItemTarget = itemObject;
                    ClearOtherTargets(exceptItem: true);
                    ShowUI($"{interactPromptText} ({itemObject.Data.itemName})");
                    return;
                }
            }

            // 5. Check for Puzzle Receptors
            if (hit.collider.TryGetComponent(out PuzzleReceptor receptor))
            {
                if (!receptor.IsSolved)
                {
                    currentReceptorTarget = receptor;
                    ClearOtherTargets(exceptReceptor: true);
                    ShowUI(receptor.GetPromptText());
                    return;
                }
            }
        }

        ClearTargets();
    }

    private void ExecuteInteraction()
    {
        // 1. Handle Release if already dragging
        if (activeDraggedObject != null)
        {
            activeDraggedObject.StopDragging();
            activeDraggedObject = null;
            ClearTargets();
            return;
        }

        // 2. Handle Start Dragging
        if (currentDraggableTarget != null)
        {
            activeDraggedObject = currentDraggableTarget;
            activeDraggedObject.StartDragging(holdPoint);
            currentDraggableTarget = null;
            return;
        }

        // 3. Handle Interface Interaction (Pillars, Levers, Buttons, etc.)
        if (currentInteractableTarget != null)
        {
            currentInteractableTarget.Interact();
            ClearTargets();
            return;
        }

        // 4. Handle Item Pickup
        if (currentItemTarget != null)
        {
            ItemObject targetToPickUp = currentItemTarget;

            if (playerInventory != null && playerInventory.AddItem(targetToPickUp.Data))
            {
                ClearTargets();
                targetToPickUp.OnPickup();
            }
            return;
        }

        // 5. Handle Puzzle Placement
        if (currentReceptorTarget != null)
        {
            if (currentReceptorTarget.TryActivate(playerInventory))
            {
                ClearTargets();
            }
            return;
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

    private void ClearOtherTargets(bool exceptInteractable = false, bool exceptDraggable = false, bool exceptItem = false, bool exceptReceptor = false)
    {
        if (!exceptInteractable) currentInteractableTarget = null;
        if (!exceptDraggable) currentDraggableTarget = null;
        if (!exceptItem) currentItemTarget = null;
        if (!exceptReceptor) currentReceptorTarget = null;
    }

    private void ClearTargets()
    {
        currentInteractableTarget = null;
        currentDraggableTarget = null;
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