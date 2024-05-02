using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimationEvents : MonoBehaviour
{
    public Door doorScript;

    public void DoorOpened()
    {
        doorScript.opened = true;
    }

    public void DoorClosed()
    {
        doorScript.opened = false;
    }
}
