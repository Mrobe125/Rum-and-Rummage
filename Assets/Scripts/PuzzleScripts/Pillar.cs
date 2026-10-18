using System.Collections;
using UnityEngine;

public class Pillar : MonoBehaviour
{
    public enum RotationDirection { Clockwise, Counterclockwise }

    [Header("Rotation Settings")]
    [Tooltip("Amount of degrees per step (e.g., 90 for 4 cardinal directions).")]
    [SerializeField] private float stepAngle = 90f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private RotationDirection rotationDirection = RotationDirection.Clockwise;

    [Header("Prompt Settings")]
    [SerializeField] private string pillarName = "Pillar";

    private bool isRotating = false;
    private float currentYRotation = 0f;

    public int CurrentStep { get; private set; } = 0;
    public bool IsRotating => isRotating;

    private void Awake()
    {
        // Normalize initial Y rotation relative to starting local Euler angles
        currentYRotation = transform.localEulerAngles.y;
    }

    public string GetPromptText()
    {
        string dirText = rotationDirection == RotationDirection.Clockwise ? "Clockwise" : "Counterclockwise";
        return $"Press E to Rotate {pillarName} ({dirText})";
    }

    public void Interact()
    {
        if (isRotating) return;

        // Calculate directional multiplier
        float directionSign = (rotationDirection == RotationDirection.Clockwise) ? 1f : -1f;

        // Update Step Counter (0 to 3 for 90-degree steps)
        int totalSteps = Mathf.RoundToInt(360f / stepAngle);
        int stepChange = (rotationDirection == RotationDirection.Clockwise) ? 1 : -1;
        CurrentStep = (CurrentStep + stepChange + totalSteps) % totalSteps;

        // Target rotation calculation
        currentYRotation += stepAngle * directionSign;
        StartCoroutine(AnimateRotation(currentYRotation));
    }

    private IEnumerator AnimateRotation(float targetY)
    {
        isRotating = true;
        Quaternion startRotation = transform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(startRotation.eulerAngles.x, targetY, startRotation.eulerAngles.z);

        while (Quaternion.Angle(transform.localRotation, targetRotation) > 0.1f)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
            yield return null;
        }

        transform.localRotation = targetRotation;
        isRotating = false;
    }
}