using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{
  public Material hitMaterial;
  public Animator chestOpen;
  public AudioClip treasureGet;

  //Quest Panel Stuff
  public Image questPanelImage;
  public Sprite questCompleteSprite;

  private Renderer objectRenderer;
  private AudioSource audioSource;

  private bool hasBeenOpened = false;

  private void Start()
  {
    objectRenderer = GetComponent<Renderer>();
    audioSource = GetComponent<AudioSource>();
  }

  //Temp fix for keeping Quest Complete if chest is opened before talking to npc
  private void LateUpdate()
  {
    if (hasBeenOpened && questPanelImage != null && questCompleteSprite != null
        && questPanelImage.sprite != questCompleteSprite)
    {
      questPanelImage.sprite = questCompleteSprite;
    }
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

      if (!questPanelImage != null && questCompleteSprite != null)
      {
        questPanelImage.sprite = questCompleteSprite;
        Debug.Log("Chest Opened UI Updated");
      }
    }
  }
}
