using UnityEngine;

public class CoinBag : MonoBehaviour
{
  private bool collected = false;

  private void OnTriggerEnter(Collider other)
  {
    if (collected) return;

    if (other.CompareTag("Player"))
    {
      collected = true;
      FindFirstObjectByType<TreasureCount>().AddGold(100);
      // Hiding/Disable these for now to continue count
      GetComponent<Renderer>().enabled = false;
      GetComponent<Collider>().enabled = false;

      Destroy(gameObject);
    }
  }
}