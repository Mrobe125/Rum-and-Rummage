using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;

    [Header("Parrot Settings")]
    public bool canBePickedUpByParrot = false;
}