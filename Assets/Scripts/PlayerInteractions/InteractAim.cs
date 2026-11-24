using UnityEngine;
using UnityEngine.UI;

public class InteractAim : MonoBehaviour
{
  public Image reticle;
    public GameObject eText;
  public Color defaultColor, interactableColor;
  public Sprite defaultReticle;
  public Sprite interactReticle;
    public Vector3 defaultRot, interactableRot;

  //How close till Interact
  public float interactDistance = 1f;
  public LayerMask interactableLayer;

    // Update is called once per frame
    void Update()
    {
        //Cam check
        //bool lookingAtInteractable = Physics.Raycast(
        //  transform.position,
        //  transform.forward,
        //  out RaycastHit hit,
        //  interactDistance,
        //  interactableLayer
        //);
        //Ray cameraRay = Camera.main.ScreenPointToRay(Vector3.zero);
        RaycastHit hit;
        bool lookingAtInteractable = false;
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * interactDistance);
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactDistance, interactableLayer))
        {
            Debug.Log(hit.transform.name);
            lookingAtInteractable = true;
        }
        
        //Change sprite for what player is looking at
        reticle.color = lookingAtInteractable ? interactableColor : defaultColor;
        reticle.rectTransform.localRotation = lookingAtInteractable ? Quaternion.Euler(interactableRot): Quaternion.Euler(defaultRot);
        eText.SetActive(lookingAtInteractable ? true : false);

    }
}
