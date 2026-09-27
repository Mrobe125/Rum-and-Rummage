using UnityEngine;

public class HookStick : MonoBehaviour
{
  private Rigidbody rb;
  private bool targetHit;
  private Transform target;
  private Vector3 localPosition;
  private Quaternion localRotation;

  private void Start()
  {
    rb = GetComponent<Rigidbody>();
  }

  private void OnCollisionEnter(Collision collision)
  {
    if (targetHit)
      return;

      targetHit = true;
      //Freeze hook at impact point
      rb.linearVelocity = Vector3.zero;
      rb.angularVelocity = Vector3.zero;
      rb.isKinematic = true;
      //Hook moves with object
      target = collision.transform;
      localPosition = target.InverseTransformPoint(transform.position);
      localRotation = Quaternion.Inverse(target.rotation) * transform.rotation;

      transform.SetParent(target);
  }

  private void LateUpdate()
  {
    if (!targetHit || target == null)
      return;

    transform.position = target.TransformPoint(localPosition);
    transform.rotation = target.rotation * localRotation;
  
  }
}
