// FIRST PERSON MOVEMENT in 10 MINUTES - Unity Tutorial
// https://www.youtube.com/watch?v=f473C43s8nE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition;

    // Update is called once per frame
    void Update()
    {
        transform.position = cameraPosition.position;
    }
}
