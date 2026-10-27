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
    [SerializeField] private PillarPuzzleManager pillarPuzzleManager;

    private ItemObject currentItemTarget;
    private PuzzleReceptor currentReceptorTarget;
    private Pillar currentPillarTarget;
    private DraggableObject currentDraggableTarget;

    // Currently dragged object reference
    private DraggableObject activeDraggedObject;

    private void Awake()
    {
        if (playerCamera == null) playerCamera = Camera.main;
        if (playerInventory == null) playerInventory = GetComponent<Inventory>();
        if (pillarPuzzleManager == null) pillarPuzzleManager = FindFirstObjectByType<PillarPuzzleManager>();

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
        // If we are holding an object and it drops itself (e.g. pulled too far), clear reference
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
        // If currently dragging an object, update UI to prompt release
        if (activeDraggedObject != null)
        {
            ShowUI("Press E to Release Object");
            return;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayer))
        {
            // 1. Check for Draggable Objects
            if (hit.collider.TryGetComponent(out DraggableObject draggable))
            {
                currentDraggableTarget = draggable;
                ClearOtherTargets(exceptDraggable: true);
                ShowUI("Press E to Grab Object");
                return;
            }

            // 2. Check for Item Pickups
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

            // 3. Check for Puzzle Receptors
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

            // 4. Check for Pillars
            if (hit.collider.TryGetComponent(out Pillar pillar))
            {
                currentPillarTarget = pillar;
                ClearOtherTargets(exceptPillar: true);
                ShowUI(pillar.GetPromptText());
                return;
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

        // 3. Handle Item Pickup
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

        // 4. Handle Puzzle Placement
        if (currentReceptorTarget != null)
        {
            if (currentReceptorTarget.TryActivate(playerInventory))
            {
                ClearTargets();
            }
            return;
        }

        // 5. Handle Pillar Rotation
        if (currentPillarTarget != null)
        {
            if (currentPillarTarget.IsRotating) return;

            Pillar rotatedPillar = currentPillarTarget;
            rotatedPillar.Interact();

            if (pillarPuzzleManager != null)
            {
                StartCoroutine(ValidatePuzzleAfterRotation(rotatedPillar));
            }
            return;
        }
    }

    private IEnumerator ValidatePuzzleAfterRotation(Pillar pillar)
    {
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

    private void ClearOtherTargets(bool exceptDraggable = false, bool exceptItem = false, bool exceptReceptor = false, bool exceptPillar = false)
    {
        if (!exceptDraggable) currentDraggableTarget = null;
        if (!exceptItem) currentItemTarget = null;
        if (!exceptReceptor) currentReceptorTarget = null;
        if (!exceptPillar) currentPillarTarget = null;
    }

    private void ClearTargets()
    {
        currentDraggableTarget = null;
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