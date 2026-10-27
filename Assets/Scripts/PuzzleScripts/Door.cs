using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    public enum DoorType { Animator, SmoothTransform }

    [Header("Door Mode")]
    [SerializeField] private DoorType doorType = DoorType.Animator;

    [Header("Animator Settings")]
    [Tooltip("Required if DoorType is set to Animator.")]
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private string openBoolOrTrigger = "IsOpen";

    [Header("Transform Movement Settings")]
    [Tooltip("Target local position or rotation offset when open.")]
    [SerializeField] private Vector3 openPositionOffset = new Vector3(0, 3f, 0); // e.g. move up 3 units
    [SerializeField] private Vector3 openRotationOffset = Vector3.zero;        // e.g. rotate 90 degrees on Y
    [SerializeField] private float moveSpeed = 2f;

    [Header("Sound & State")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;

    private bool isOpen = false;
    private Vector3 closedPosition;
    private Quaternion closedRotation;
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    private void Awake()
    {
        closedPosition = transform.localPosition;
        closedRotation = transform.localRotation;

        targetPosition = closedPosition;
        targetRotation = closedRotation;

        if (doorType == DoorType.Animator && doorAnimator == null)
        {
            doorAnimator = GetComponent<Animator>();
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    /// <summary>
    /// Call this method directly from a UnityEvent (e.g. PuzzleReceptor's OnItemPlaced event).
    /// </summary>
    public void OpenDoor()
    {
        if (isOpen) return;

        isOpen = true;
        PlayOpenSound();

        if (doorType == DoorType.Animator && doorAnimator != null)
        {
            // Set bool parameter or trigger animation
            doorAnimator.SetBool(openBoolOrTrigger, true);
        }
        else if (doorType == DoorType.SmoothTransform)
        {
            targetPosition = closedPosition + openPositionOffset;
            targetRotation = closedRotation * Quaternion.Euler(openRotationOffset);
            StopAllCoroutines();
            StartCoroutine(AnimateDoorTransform());
        }

        Debug.Log($"[Door] {gameObject.name} opened!");
    }

    /// <summary>
    /// Optional: Call this from a UnityEvent to close the door.
    /// </summary>
    public void CloseDoor()
    {
        if (!isOpen) return;

        isOpen = false;

        if (doorType == DoorType.Animator && doorAnimator != null)
        {
            doorAnimator.SetBool(openBoolOrTrigger, false);
        }
        else if (doorType == DoorType.SmoothTransform)
        {
            targetPosition = closedPosition;
            targetRotation = closedRotation;
            StopAllCoroutines();
            StartCoroutine(AnimateDoorTransform());
        }
    }

    private IEnumerator AnimateDoorTransform()
    {
        while (Vector3.Distance(transform.localPosition, targetPosition) > 0.01f ||
               Quaternion.Angle(transform.localRotation, targetRotation) > 0.1f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * moveSpeed);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * moveSpeed);
            yield return null;
        }

        transform.localPosition = targetPosition;
        transform.localRotation = targetRotation;
    }

    private void PlayOpenSound()
    {
        if (audioSource != null && openSound != null)
        {
            audioSource.PlayOneShot(openSound);
        }
    }
}