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
    [Tooltip("Fires when all pillars match their target steps.")]
    [SerializeField] private UnityEvent OnPuzzleSolved;

    public bool IsSolved { get; private set; } = false;

    public void CheckPuzzleState()
    {
        if (IsSolved) return;

        foreach (var item in requiredSequence)
        {
            if (item.pillar == null) continue;

            // If any pillar is currently moving or not at target step, return
            if (item.pillar.IsRotating || item.pillar.CurrentStep != item.targetStep)
            {
                return;
            }
        }

        // All steps match!
        IsSolved = true;
        Debug.Log("[PillarPuzzleManager] Puzzle Solved!");
        OnPuzzleSolved?.Invoke();
    }
}