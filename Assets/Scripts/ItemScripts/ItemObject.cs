using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField] private ItemData itemData;

    public ItemData Data => itemData;

    public void OnPickup()
    {
        Destroy(gameObject);
    }
}