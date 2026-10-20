using UnityEngine;

public class UIInput : MonoBehaviour
{
  public GameObject questPanel;
  public HookThrow hookThrow;
  public MonoBehaviour playerController;

  void Start()
  {
    questPanel.SetActive(false);
  }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            bool isOpen = !questPanel.activeSelf;
            questPanel.SetActive(isOpen);
            playerController.enabled = !isOpen;
            hookThrow.enabled = !isOpen;
            Cursor.visible = isOpen;
            Cursor.lockState = isOpen
              ? CursorLockMode.None
              : CursorLockMode.Locked;
        }
    }
}
