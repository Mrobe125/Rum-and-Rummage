using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class PressurePlateTrigger : MonoBehaviour
{
    [Header("Weight Configuration")]
    [Tooltip("The exact total weight required to activate the pressure plate.")]
    [SerializeField] private float requiredWeight = 10f;

    [Tooltip("If true, the plate triggers when total weight is GREATER THAN OR EQUAL to requiredWeight. If false, it requires an EXACT match.")]
    [SerializeField] private bool allowExcessWeight = false;

    [Tooltip("Tolerance for exact matching floating-point weights (e.g. 9.999 triggers a required 10).")]
    [SerializeField] private float weightTolerance = 0.05f;

    [Header("Accepted Items Filter")]
    [Tooltip("If assigned, only objects with this specific ItemData will contribute weight.")]
    [SerializeField] private ItemData requiredItemData;

    [Header("Visual Feedback / Animation")]
    [Tooltip("Optional transform for the plate mesh that depresses downward when weight is added.")]
    [SerializeField] private Transform plateMesh;
    [SerializeField] private Vector3 depressedOffset = new Vector3(0f, -0.1f, 0f);
    [SerializeField] private float moveSpeed = 5f;

    [Header("Events")]
    [Tooltip("Fires when the plate reaches the required total weight.")]
    [SerializeField] private UnityEvent OnWeightRequirementMet;

    [Tooltip("Fires when the weight drops back below the required amount after being activated.")]
    [SerializeField] private UnityEvent OnWeightRequirementLost;

    [Header("Read-Only Status")]
    [SerializeField] private float currentTotalWeight = 0f;
    [SerializeField] private bool isActivated = false;

    // Track objects currently resting on the plate
    private readonly HashSet<WeightedObject> currentObjectsOnPlate = new HashSet<WeightedObject>();

    private Vector3 initialPlatePosition;
    private Vector3 targetPlatePosition;

    public float CurrentTotalWeight => currentTotalWeight;
    public bool IsActivated => isActivated;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;

        if (plateMesh != null)
        {
            initialPlatePosition = plateMesh.localPosition;
            targetPlatePosition = initialPlatePosition;
        }
    }

    private void Update()
    {
        // Smoothly move the plate mesh down/up based on current state
        if (plateMesh != null && Vector3.Distance(plateMesh.localPosition, targetPlatePosition) > 0.001f)
        {
            plateMesh.localPosition = Vector3.Lerp(plateMesh.localPosition, targetPlatePosition, Time.deltaTime * moveSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out WeightedObject weightedObj))
        {
            // Validate required ItemData if specified
            if (requiredItemData != null)
            {
                if (!other.TryGetComponent(out ItemObject itemObj) || itemObj.Data != requiredItemData)
                {
                    Debug.Log($"[PressurePlateTrigger] Ignored '{other.name}' — ItemData does not match '{requiredItemData.itemName}'.");
                    return;
                }
            }

            if (currentObjectsOnPlate.Add(weightedObj))
            {
                RecalculateWeight();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out WeightedObject weightedObj))
        {
            if (currentObjectsOnPlate.Remove(weightedObj))
            {
                RecalculateWeight();
            }
        }
    }

    private void RecalculateWeight()
    {
        // Clean up any destroyed/null objects from the set
        currentObjectsOnPlate.RemoveWhere(obj => obj == null);

        currentTotalWeight = 0f;
        foreach (var obj in currentObjectsOnPlate)
        {
            currentTotalWeight += obj.Weight;
        }

        Debug.Log($"[PressurePlateTrigger] Current Plate Weight: {currentTotalWeight} / {requiredWeight}");

        // Depress plate visually proportionally to weight (capped at max required weight)
        if (plateMesh != null)
        {
            float weightPercent = Mathf.Clamp01(currentTotalWeight / requiredWeight);
            targetPlatePosition = initialPlatePosition + (depressedOffset * weightPercent);
        }

        EvaluateActivation();
    }

    private void EvaluateActivation()
    {
        bool weightMatches = false;

        if (allowExcessWeight)
        {
            weightMatches = currentTotalWeight >= (requiredWeight - weightTolerance);
        }
        else
        {
            // Exact weight check within tolerance
            weightMatches = Mathf.Abs(currentTotalWeight - requiredWeight) <= weightTolerance;
        }

        if (weightMatches && !isActivated)
        {
            isActivated = true;
            Debug.Log($"[PressurePlateTrigger] Target weight reached! Plate Activated.");
            OnWeightRequirementMet?.Invoke();
        }
        else if (!weightMatches && isActivated)
        {
            isActivated = false;
            Debug.Log($"[PressurePlateTrigger] Weight requirement lost. Plate Deactivated.");
            OnWeightRequirementLost?.Invoke();
        }
    }
}