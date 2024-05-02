using UnityEngine;

public class RotatingKeys : MonoBehaviour
{

    [SerializeField] private float rotationSpeed = 20f;

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger");
    }
}
