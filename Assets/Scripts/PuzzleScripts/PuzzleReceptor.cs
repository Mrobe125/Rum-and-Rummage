using UnityEngine;
using UnityEngine.Events;

public class PuzzleReceptor : MonoBehaviour
{
    [Header("Required Item")]
    [Tooltip("The specific ItemData asset required to activate this receptor.")]
    [SerializeField] private ItemData requiredItem;

    [Header("Receptor Visuals")]
    [Tooltip("Optional visual mesh or GameObject to activate when the item is placed (e.g., skull model on pedestal).")]
    [SerializeField] private GameObject placedItemVisual;

    [Header("Puzzle Settings")]
    [SerializeField] private bool consumesItem = true;
    [SerializeField] private string interactPrompt = "Press E to place";

    [Header("Events")]
    [Tooltip("Actions triggered when the correct item is placed.")]
    [SerializeField] private UnityEvent OnItemPlaced;

    public bool IsSolved { get; private set; }
    public ItemData RequiredItem => requiredItem;

    private void Start()
    {
        if (placedItemVisual != null)
            placedItemVisual.SetActive(false);
    }

    public string GetPromptText()
    {
        if (IsSolved) return string.Empty;
        return $"{interactPrompt} {requiredItem.itemName}";
    }

    public bool TryActivate(Inventory playerInventory)
    {
        if (IsSolved || requiredItem == null || playerInventory == null)
            return false;

        if (playerInventory.HasItem(requiredItem))
        {
            if (consumesItem)
            {
                playerInventory.RemoveItem(requiredItem);
            }

            IsSolved = true;

            if (placedItemVisual != null)
                placedItemVisual.SetActive(true);

            Debug.Log($"[PuzzleReceptor] {requiredItem.itemName} placed successfully on {gameObject.name}!");
            OnItemPlaced?.Invoke();
            return true;
        }

        Debug.Log($"[PuzzleReceptor] Player missing required item: {requiredItem.itemName}");
        return false;
    }
}