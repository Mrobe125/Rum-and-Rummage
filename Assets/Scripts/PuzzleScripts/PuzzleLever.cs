using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleLever : MonoBehaviour, IInteractable
{
    [Header("References")]
    [Tooltip("Reference to the manager that validates the puzzle steps.")]
    [SerializeField] private PillarPuzzleManager puzzleManager;

    [Tooltip("Optional transform of the lever handle/arm mesh to animate on pull.")]
    [SerializeField] private Transform leverHandle;

    [Header("Prompt Settings")]
    [SerializeField] private string leverName = "Puzzle Lever";

    [Header("Lever Animation Settings")]
    [Tooltip("Local rotation angle when the lever is pulled down.")]
    [SerializeField] private Vector3 pulledRotationOffset = new Vector3(45f, 0f, 0f);
    [SerializeField] private float pullSpeed = 4f;

    [Tooltip("If true, the lever springs back to its original position after submission.")]
    [SerializeField] private bool autoReset = true;
    [SerializeField] private float resetDelay = 1f;

    [Header("Events")]
    [Tooltip("Fires when the lever is pulled (regardless of result).")]
    [SerializeField] private UnityEvent OnLeverPulled;

    [Tooltip("Fires if the submitted guess is INCORRECT.")]
    [SerializeField] private UnityEvent OnWrongSolutionSubmitted;

    private bool isPulling = false;
    private Quaternion restRotation;
    private Quaternion pulledRotation;

    public bool IsPulling => isPulling;

    private void Awake()
    {
        if (leverHandle != null)
        {
            restRotation = leverHandle.localRotation;
            pulledRotation = restRotation * Quaternion.Euler(pulledRotationOffset);
        }
    }

    public string GetPromptText()
    {
        if (puzzleManager != null && puzzleManager.IsSolved)
        {
            return $"{leverName} (Already Solved)";
        }

        return $"Press E to Pull {leverName}";
    }

    public void Interact()
    {
        if (isPulling) return;

        // Prevent pulling if puzzle is already solved
        if (puzzleManager != null && puzzleManager.IsSolved) return;

        StartCoroutine(PullLeverRoutine());
    }

    private IEnumerator PullLeverRoutine()
    {
        isPulling = true;
        OnLeverPulled?.Invoke();

        // 1. Animate lever pulling down
        if (leverHandle != null)
        {
            yield return StartCoroutine(AnimateLeverHandle(pulledRotation));
        }

        // 2. Submit guess to PillarPuzzleManager
        if (puzzleManager != null)
        {
            bool solved = puzzleManager.CheckPuzzleState();

            if (!solved)
            {
                Debug.Log($"[PuzzleLever] Incorrect configuration submitted via {gameObject.name}.");
                OnWrongSolutionSubmitted?.Invoke();
            }
        }

        // 3. Optional auto-reset handle back to starting position
        if (autoReset)
        {
            yield return new WaitForSeconds(resetDelay);

            if (leverHandle != null)
            {
                yield return StartCoroutine(AnimateLeverHandle(restRotation));
            }
        }

        isPulling = false;
    }

    private IEnumerator AnimateLeverHandle(Quaternion targetRotation)
    {
        while (Quaternion.Angle(leverHandle.localRotation, targetRotation) > 0.1f)
        {
            leverHandle.localRotation = Quaternion.Slerp(leverHandle.localRotation, targetRotation, Time.deltaTime * pullSpeed);
            yield return null;
        }

        leverHandle.localRotation = targetRotation;
    }
}