using UnityEngine;
using UnityEngine.UI;

public class InteractAim : MonoBehaviour
{
  public Image reticle;

  public Sprite defaultReticle;
  public Sprite interactReticle;

  //How close till Interact
  public float interactDistance = 1f;
  public LayerMask interactableLayer;

    // Update is called once per frame
    void Update()
    {
    //Cam check
      bool lookingAtInteractable = Physics.Raycast(
        transform.position,
        transform.forward,
        out RaycastHit hit,
        interactDistance,
        interactableLayer
      );
      //Change sprite for what player is looking at
      reticle.sprite = lookingAtInteractable ? interactReticle : defaultReticle;
    }
}
