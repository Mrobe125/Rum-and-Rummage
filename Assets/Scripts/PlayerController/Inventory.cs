using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<ItemData> items = new List<ItemData>();

    public IReadOnlyList<ItemData> Items => items;

    public bool AddItem(ItemData item)
    {
        if (item == null) return false;

        items.Add(item);
        Debug.Log($"Added {item.itemName} to inventory. Total items: {items.Count}");
        return true;
    }

    public bool HasItem(ItemData item)
    {
        return items.Contains(item);
    }

    public bool RemoveItem(ItemData item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            Debug.Log($"Removed {item.itemName} from inventory. Remaining: {items.Count}");
            return true;
        }
        return false;
    }
}