using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DraggableObject : MonoBehaviour
{
    [Header("Drag Properties")]
    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private float maxDistance = 4f;

    private Rigidbody rb;
    private Transform holdPoint;
    private bool isBeingDragged = false;

    public bool IsBeingDragged => isBeingDragged;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void StartDragging(Transform playerHoldPoint)
    {
        isBeingDragged = true;
        holdPoint = playerHoldPoint;
        rb.useGravity = false;
        rb.linearDamping = 10f; // High damping prevents wild swinging
        rb.angularDamping = 5f;
    }

    public void StopDragging()
    {
        isBeingDragged = false;
        holdPoint = null;
        rb.useGravity = true;
        rb.linearDamping = 1f;
        rb.angularDamping = 0.05f;
    }

    private void FixedUpdate()
    {
        if (!isBeingDragged || holdPoint == null) return;

        // Force drop if player walks too far away from the object
        float distance = Vector3.Distance(transform.position, holdPoint.position);
        if (distance > maxDistance)
        {
            StopDragging();
            return;
        }

        // Move towards hold position using physics forces
        Vector3 moveDirection = (holdPoint.position - transform.position);
        rb.linearVelocity = moveDirection * followSpeed;
    }
}