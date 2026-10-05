using UnityEngine;

public class Target : MonoBehaviour
{
  public Material hitMaterial;
  public Animator chestOpen;
  public AudioClip treasureGet;

  private Renderer objectRenderer;
  private AudioSource audioSource;

  private bool hasBeenOpened = false;

  private void Start()
  {
    objectRenderer = GetComponent<Renderer>();
    audioSource = GetComponent<AudioSource>();
  }

  private void OnCollisionEnter(Collision collision)
  {
    if (collision.gameObject.CompareTag("Hook"))
    {
      if (hasBeenOpened)
        return;

      hasBeenOpened = true;

      if (objectRenderer != null && hitMaterial != null)
      {
        objectRenderer.material = hitMaterial;
      }

      if (chestOpen != null)
      {
        chestOpen.SetTrigger("OpenChest");
      }

      if (audioSource != null && treasureGet != null)
      {
        audioSource.PlayOneShot(treasureGet);
      }
    }
  }
}
