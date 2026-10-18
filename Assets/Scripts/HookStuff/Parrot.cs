using UnityEngine;

public class Parrot : MonoBehaviour
{
    public HookThrow hookThrow;
    public KeyCode returnClick = KeyCode.Mouse1;
    public float speed = 15f;
    public AudioClip squackSound;

    [Header("Item Raycast Settings")]
    [SerializeField] private Transform raycastOrigin; // Assign Player Camera here
    [SerializeField] private float raycastDistance = 15f;
    [SerializeField] private LayerMask itemLayer;

    [Header("Dependencies")]
    [SerializeField] private Inventory playerInventory;

    private Transform player;
    private Vector3 startPos;
    private Quaternion startRot;

    private HookStick hook;
    private ItemObject carriedItem;
    private AudioSource audioSource;
    private bool goToHook = false;
    private bool goToItem = false;
    private bool goToPlayer = false;

    private void Start()
    {
        // Parrot shoulder spot
        player = transform.parent;
        startPos = transform.localPosition;
        startRot = transform.localRotation;

        if (raycastOrigin == null && Camera.main != null)
        {
            raycastOrigin = Camera.main.transform;
        }

        if (playerInventory == null && player != null)
        {
            playerInventory = player.GetComponent<Inventory>();
        }

        audioSource = GetComponent<AudioSource>();

        // STOP STICKING TO PARROT PLZZZ
        if (TryGetComponent(out Collider col))
        {
            col.enabled = false;
        }
    }

    private void Update()
    {
        CheckInput();

        if (goToHook)
            GoingToHook();

        if (goToItem)
            GoingToItem();

        if (goToPlayer)
            ReturnToPlayer();
    }

    private void CheckInput()
    {
        if (Input.GetKeyDown(returnClick) && !goToHook && !goToItem && !goToPlayer)
        {
            // 1. Try finding a thrown hook first
            hook = FindFirstObjectByType<HookStick>();
            if (hook != null && !hookThrow.hookHeld.activeSelf)
            {
                transform.SetParent(null);
                goToHook = true;
                PlaySquack();
                return;
            }

            // 2. If no hook active, raycast for a fetchable item
            ItemObject targetItem = GetLookedAtFetchableItem();
            if (targetItem != null)
            {
                carriedItem = targetItem;
                transform.SetParent(null);
                goToItem = true;
                PlaySquack();
            }
        }
    }

    private ItemObject GetLookedAtFetchableItem()
    {
        if (raycastOrigin == null) return null;

        Ray ray = new Ray(raycastOrigin.position, raycastOrigin.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, itemLayer))
        {
            if (hit.collider.TryGetComponent(out ItemObject item) && item.Data != null)
            {
                if (item.Data.canBePickedUpByParrot)
                {
                    return item;
                }
            }
        }

        return null;
    }

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
            if (hook.TryGetComponent(out Collider col)) col.enabled = false;

            hook.transform.SetParent(transform);
            hook.transform.localPosition = new Vector3(0, -0.6f, 0);

            goToHook = false;
            goToPlayer = true;
        }
    }

    private void GoingToItem()
    {
        if (carriedItem == null)
        {
            goToItem = false;
            goToPlayer = true;
            return;
        }

        MoveTo(carriedItem.transform.position);
        if (Vector3.Distance(transform.position, carriedItem.transform.position) < 0.3f)
        {
            if (carriedItem.TryGetComponent(out Collider col)) col.enabled = false;
            if (carriedItem.TryGetComponent(out Rigidbody rb)) Destroy(rb);

            carriedItem.transform.SetParent(transform);
            carriedItem.transform.localPosition = new Vector3(0, -0.4f, 0);

            goToItem = false;
            goToPlayer = true;
        }
    }

    private void ReturnToPlayer()
    {
        Vector3 home = player.TransformPoint(startPos);
        MoveTo(home);

        if (Vector3.Distance(transform.position, home) < 0.3f)
        {
            // Drop/Destroy hook if returning with hook
            if (hook != null)
            {
                Destroy(hook.gameObject);
                hookThrow.ResetThrow();
            }

            // Give item to inventory if returning with an item
            if (carriedItem != null)
            {
                if (playerInventory != null)
                {
                    playerInventory.AddItem(carriedItem.Data);
                }
                carriedItem.OnPickup();
                carriedItem = null;
            }

            // Re-Equip to player at shoulder position/rotation
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

    private void PlaySquack()
    {
        if (audioSource != null && squackSound != null)
        {
            audioSource.PlayOneShot(squackSound);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (raycastOrigin != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(raycastOrigin.position, raycastOrigin.forward * raycastDistance);
        }
    }
}