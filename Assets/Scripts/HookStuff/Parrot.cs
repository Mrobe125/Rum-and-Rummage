using UnityEngine;

public class Parrot : MonoBehaviour
{
  public HookThrow hookThrow;
  public KeyCode returnClick = KeyCode.Mouse1;
  public float speed = 15f;
  public AudioClip squackSound;

  private Transform player;
  private Vector3 startPos;
  private Quaternion startRot;

  private HookStick hook;
  private AudioSource audioSource;
  private bool goToHook = false;
  private bool goToPlayer = false;

  private void Start()
  {
    //Parrot shoulder spot
    player = transform.parent;
    startPos = transform.localPosition;
    startRot = transform.localRotation;

    audioSource = GetComponent<AudioSource>();

    //STOP STICKING TO PARROT PLZZZ
    GetComponent<Collider>().enabled = false;
  }

  private void Update()
  {
    CheckInput();

    if (goToHook)
      GoingToHook();

    if (goToPlayer)
      ReturnToPlayer();
  }

  //Send parrot if hook is thrown
  private void CheckInput()
  {
    if (Input.GetKeyDown(returnClick) && !goToHook && !goToPlayer && !hookThrow.hookHeld.activeSelf)
    {
      hook = FindFirstObjectByType<HookStick>();

      if (hook != null)
      {
        //Unchild from player
        transform.SetParent(null);
        goToHook = true;

        //Play obnoxious SQUACK!
        audioSource.PlayOneShot(squackSound);
      }
    }
  }

  //Fly to Hook and grab
  private void GoingToHook()
  {
    if (hook == null)
    {
      goToHook = false;
      goToPlayer = true;
      return;
    }

    MoveTo(hook.transform.position);
    if (Vector3.Distance(transform.position, hook.transform.position) < 0.3f)
    {
      Destroy(hook.GetComponent<Rigidbody>());
      hook.enabled = false;
      hook.GetComponent<Collider>().enabled = false;

      hook.transform.SetParent(transform);
      hook.transform.localPosition = new Vector3(0, -0.6f, 0);

      goToHook = false;
      goToPlayer = true;
    }
  }
  //Back to Player
  private void ReturnToPlayer()
  {
    Vector3 home = player.TransformPoint(startPos);
    MoveTo(home);

    if (Vector3.Distance(transform.position, home) < 0.3f)
    {
      if (hook != null)
      {
        Destroy(hook.gameObject);
      }
      hookThrow.ResetThrow();

      //Re-Equip to player at shoulder positon/rotation
      transform.SetParent(player);
      transform.localPosition = startPos;
      transform.localRotation = startRot;

      goToPlayer = false;
    }
  }

  private void MoveTo(Vector3 target)
  {
    transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
  }
}
