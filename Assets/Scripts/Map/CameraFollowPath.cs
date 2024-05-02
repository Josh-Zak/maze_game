using System.Collections;
using UnityEngine;

public class CameraFollowPath : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 2.0f;
    public Transform target; // The door to look at

    public IEnumerator StartCameraMovement()
    {
        foreach (Transform waypoint in waypoints)
        {
            while (transform.position != waypoint.position)
            {
                transform.position = Vector3.MoveTowards(transform.position, waypoint.position, speed * Time.deltaTime);
                transform.LookAt(target);
                yield return null;
            }
        }
    }
}