using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class TrapdoorTrigger : MonoBehaviour
{
    [Header("Accepted Objects")]
    [Tooltip("If assigned, only draggable objects carrying this specific ItemData will trigger the trapdoor. Leave empty to accept any DraggableObject.")]
    [SerializeField] private ItemData requiredItemData;

    [Header("Trapdoor Animations/Movement")]
    [Tooltip("The trapdoor mesh/hinge object that should rotate or open.")]
    [SerializeField] private Transform trapdoorHinge;
    [SerializeField] private Vector3 openRotationAngle = new Vector3(90f, 0f, 0f); // Swing down 90 degrees
    [SerializeField] private float openSpeed = 4f;

    [Header("Auto-Reset Settings")]
    [Tooltip("Should the trapdoor automatically reset and close after opening?")]
    [SerializeField] private bool autoReset = true;

    [Tooltip("Delay in seconds before closing the trapdoor and resetting the trigger.")]
    [SerializeField] private float resetDelay = 3f;

    [Header("Events")]
    [Tooltip("Fires when a valid DraggableObject opens the trapdoor.")]
    [SerializeField] private UnityEvent OnTrapdoorOpened;

    [Tooltip("Fires when the trapdoor automatically closes after the delay.")]
    [SerializeField] private UnityEvent OnTrapdoorReset;

    private bool isOpened = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Quaternion targetRotation;
    private Coroutine resetCoroutine;

    public bool IsOpened => isOpened;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;

        if (trapdoorHinge != null)
        {
            closedRotation = trapdoorHinge.localRotation;
            openRotation = closedRotation * Quaternion.Euler(openRotationAngle);
            targetRotation = closedRotation;
        }
    }

    private void Update()
    {
        // Smoothly animate the trapdoor hinge towards target rotation
        if (trapdoorHinge != null && Quaternion.Angle(trapdoorHinge.localRotation, targetRotation) > 0.01f)
        {
            trapdoorHinge.localRotation = Quaternion.Slerp(trapdoorHinge.localRotation, targetRotation, Time.deltaTime * openSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignore if already open
        if (isOpened) return;

        // Check if the entering object is a DraggableObject
        if (other.TryGetComponent(out DraggableObject draggable))
        {
            // Validate required ItemData if specified
            if (requiredItemData != null)
            {
                if (!other.TryGetComponent(out ItemObject itemObj) || itemObj.Data != requiredItemData)
                {
                    Debug.Log($"[TrapdoorTrigger] Object '{other.name}' does not match required ItemData '{requiredItemData.itemName}'.");
                    return;
                }
            }

            OpenTrapdoor(other.gameObject);
        }
    }

    public void OpenTrapdoor(GameObject triggeringObject)
    {
        isOpened = true;

        if (trapdoorHinge != null)
        {
            targetRotation = openRotation;
        }

        Debug.Log($"[TrapdoorTrigger] Trapdoor triggered by '{triggeringObject.name}'!");
        OnTrapdoorOpened?.Invoke();

        // Start 3-second reset timer
        if (autoReset)
        {
            if (resetCoroutine != null) StopCoroutine(resetCoroutine);
            resetCoroutine = StartCoroutine(ResetTimerRoutine());
        }
    }

    private IEnumerator ResetTimerRoutine()
    {
        yield return new WaitForSeconds(resetDelay);

        isOpened = false;

        if (trapdoorHinge != null)
        {
            targetRotation = closedRotation;
        }

        Debug.Log($"[TrapdoorTrigger] Reset delay elapsed ({resetDelay}s). Closing trapdoor.");
        OnTrapdoorReset?.Invoke();
    }
}