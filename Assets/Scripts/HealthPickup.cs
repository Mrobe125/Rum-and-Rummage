using UnityEngine;

public class HealthPickup : MonoBehaviour
{
  public int healAmount = 1;

  private void OnTriggerEnter(Collider other)
  {
    HeartTest playerHealth = other.GetComponent<HeartTest>();

    if (playerHealth != null)
    {

      //Ignore at max health
      if (playerHealth.health >= playerHealth.hearts.Length)
        return;

      playerHealth.health += healAmount;

      playerHealth.health = Mathf.Min(
        playerHealth.health,
        playerHealth.hearts.Length
      );

      for ( int i = 0; i < playerHealth.hearts.Length; i++)
      {
        if (playerHealth.hearts[i] != null)
        {
          playerHealth.hearts[i].gameObject.SetActive(
            i < playerHealth.health
          );
        }
      }

      Destroy(gameObject);
    }
  }
}
