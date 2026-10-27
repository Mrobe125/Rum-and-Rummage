using UnityEngine;

public class WeightedObject : MonoBehaviour
{
    [Header("Weight Settings")]
    [Tooltip("The weight value this object contributes when placed on a pressure plate.")]
    [SerializeField] private float weight = 5f;

    public float Weight
    {
        get => weight;
        set => weight = value;
    }
}