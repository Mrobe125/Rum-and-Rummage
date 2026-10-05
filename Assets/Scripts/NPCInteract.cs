using UnityEngine;

public class NPCInteract : MonoBehaviour
{
  public GameObject dialogPanel;
  public GameObject interactText;

  public MonoBehaviour playerController;

  private bool playerInRange = false;
  private bool dialogOpen = false;
   
    private void Start()
    {
        interactText.SetActive(false);
        dialogPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    
    private void Update()
    {
        if(playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!dialogOpen)
            {
              dialogPanel.SetActive(true);
              interactText.SetActive(false);

              playerController.enabled = false;

              Cursor.lockState = CursorLockMode.None;
              Cursor.visible = true;

              dialogOpen = true;
            }
            else
            {
              dialogPanel.SetActive(false);

              playerController.enabled = true;

              Cursor.lockState = CursorLockMode.Locked;
              Cursor.visible = false;

              dialogOpen = false;
            }
        }
    }

  private void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Player"))
    {
      playerInRange = true;

      if (!dialogOpen)
      {
        interactText.SetActive(true);
      }
    }
  }

  private void OnTriggerExit(Collider other)
  {
    if (other.CompareTag("Player"))
    {
      playerInRange = false;
      interactText.SetActive(false);
    }
  }
}
