using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour
{
  [SerializeField] GameObject pointA;
  [SerializeField] GameObject pointB;
  [SerializeField] float speed = 5f;
  [SerializeField] float delay = 1f;
  [SerializeField] GameObject platform;

  private Vector3 targetPosition;

  void Start()
  {
    platform.transform.position = pointA.transform.position;
    targetPosition = pointB.transform.position;
    StartCoroutine(MovePlatform());
  }

  IEnumerator MovePlatform()
  {
    while (true)
    {
      while ((targetPosition - platform.transform.position).sqrMagnitude > 0.01f)
      {
        platform.transform.position = Vector3.MoveTowards(platform.transform.position,
          targetPosition, speed * Time.deltaTime);
        yield return null;
      }

      targetPosition = targetPosition == pointA.transform.position
        ? pointB.transform.position : pointA.transform.position;

      yield return new WaitForSeconds(delay);
    }
  }
}
