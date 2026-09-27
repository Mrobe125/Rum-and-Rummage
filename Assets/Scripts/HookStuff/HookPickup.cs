using UnityEngine;

public class HookPickup : MonoBehaviour
{
  private void OnTriggerEnter(Collider other)
  {
    //Player in pickup range
    if (other.CompareTag("Player"))
    {
      HookThrow hookThrow = other.GetComponent<HookThrow>();

      if (hookThrow != null)
      {
        //Destroy and re-enable hook
        hookThrow.ResetThrow();
        Destroy(gameObject);
      }
    }
  }
}
