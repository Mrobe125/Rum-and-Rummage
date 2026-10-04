using UnityEngine;

public class HookThrow : MonoBehaviour
{
  public Transform hookCam;
  public Transform hookPoint;
  public GameObject hookPrefab;
  public GameObject hookHeld;
  public KeyCode throwKey = KeyCode.Mouse0;
  public float throwForce;

  private bool hasThrown = false;

  //private void Start()
  //{
  //  hookHeld.SetActive(true);
  //}

  private void Update()
  {
    if (Input.GetKeyDown(throwKey) && !hasThrown)
    {
      Throw();
    }
  }

  private void Throw()
  {
    hasThrown = true;
    hookHeld.SetActive(false);
    //Set Hook Prefab at held point
    GameObject projectile = Instantiate(
      hookPrefab,
      hookPoint.position,
      hookCam.rotation
    );

    Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

    Vector3 forceDirection = hookCam.forward;

    RaycastHit hit;
    //Aim at Player direction
    if (Physics.Raycast(hookCam.position, hookCam.forward, out hit, 500f))
    {
      forceDirection = (hit.point - hookPoint.position).normalized;
    }
    //Launch
    projectileRb.AddForce(
      forceDirection * throwForce,
      ForceMode.Impulse
      );
  }
  //When picked up able to go again
  public void ResetThrow()
  {
    hasThrown = false;
    hookHeld.SetActive(true);
  }
}
