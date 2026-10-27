using UnityEngine;

public enum ItemType
{
    Quest,
    Puzzle,
    Collectable
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Basic Information")]
    public ItemType itemType;
    public string itemName;
    [TextArea(3, 5)]
    public string description;
    public Sprite icon;
    

    [Header("Parrot Settings")]
    public bool canBePickedUpByParrot = false;
}