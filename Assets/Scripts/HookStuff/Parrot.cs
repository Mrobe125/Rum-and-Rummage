using UnityEngine;

public class Parrot : MonoBehaviour
{
    public HookThrow hookThrow;
    public KeyCode returnClick = KeyCode.Mouse1;
    public float speed = 15f;
    public AudioClip squackSound;

    [Header("Item Retrieval")]
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private float searchRadius = 10f;
    [SerializeField] private LayerMask itemLayer;

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

            // 2. If no hook active, look for a nearby item that the parrot can carry
            ItemObject targetItem = FindNearestFetchableItem();
            if (targetItem != null)
            {
                carriedItem = targetItem;
                transform.SetParent(null);
                goToItem = true;
                PlaySquack();
            }
        }
    }

    private ItemObject FindNearestFetchableItem()
    {
        Collider[] hits = Physics.OverlapSphere(player.position, searchRadius, itemLayer);
        ItemObject nearest = null;
        float minDistance = float.MaxValue;

        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent(out ItemObject item) && item.Data != null)
            {
                if (item.Data.canBePickedUpByParrot)
                {
                    float dist = Vector3.Distance(transform.position, item.transform.position);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        nearest = item;
                    }
                }
            }
        }

        return nearest;
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
}