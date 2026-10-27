using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PillarPuzzleManager : MonoBehaviour
{
    [System.Serializable]
    public struct PillarTarget
    {
        public Pillar pillar;
        [Tooltip("The required step index (e.g., 0 = 0 deg, 1 = 90 deg, 2 = 180 deg, 3 = 270 deg).")]
        public int targetStep;
    }

    [Header("Puzzle Configuration")]
    [SerializeField] private List<PillarTarget> requiredSequence = new List<PillarTarget>();

    [Header("Events")]
    [Tooltip("Fires when all pillars match their target steps upon submission.")]
    [SerializeField] private UnityEvent OnPuzzleSolved;

    public bool IsSolved { get; private set; } = false;

    /// <summary>
    /// Validates current pillar states. Returns true if correct, false otherwise.
    /// </summary>
    public bool CheckPuzzleState()
    {
        if (IsSolved) return true;

        foreach (var item in requiredSequence)
        {
            if (item.pillar == null) continue;

            // If any pillar is actively moving or not at its designated target step
            if (item.pillar.IsRotating || item.pillar.CurrentStep != item.targetStep)
            {
                return false;
            }
        }

        // All steps match!
        IsSolved = true;
        Debug.Log("[PillarPuzzleManager] Puzzle Solved!");
        OnPuzzleSolved?.Invoke();
        return true;
    }
}