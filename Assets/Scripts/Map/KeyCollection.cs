using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class KeyCollection : MonoBehaviour
{
    public AudioSource source;
    public GameObject keyImage;
    public Door door;

    private void OnTriggerEnter(Collider other)
    {
        // Only Player should be able to collide with keys
        if (other.CompareTag("Player"))
        {
            // Play the audio source
            source.Play();

            // Enable Key on HUD
            keyImage.SetActive(true);

            StartCoroutine("Protocol");
        }
    }

    private IEnumerator Protocol()
    {
        Debug.Log("Disabling Collider");
        gameObject.GetComponent<SphereCollider>().enabled = false;

        Debug.Log("Switching Cameras");
        door.doorCam.enabled = true;
        Camera playerCam = GameObject.Find("PlayerCam").GetComponent<Camera>();
        playerCam.enabled = false;

        Debug.Log("Disabling HUD");
        GameObject hud = GameObject.Find("HUD");
        hud.SetActive(false);

        Debug.Log("Starting Camera Movement");
        StartCoroutine(door.doorCam.GetComponent<CameraFollowPath>().StartCameraMovement());

        Debug.Log("Waiting for two seconds");
        yield return new WaitForSeconds(2.5f);

        Debug.Log("Setting lights green and unlocking door");
        // Set lights to green indicating that it's unlocked
        door.setLightsGreen();
        door.locked = false;

        // Play buzzer sound indicating unlocking
        door.buzzer.Play();

        Debug.Log("Waiting for two seconds");
        yield return new WaitForSeconds(2f);

        Debug.Log("Switching Cameras Back");
        playerCam.enabled = true;
        door.doorCam.enabled = false;

        Debug.Log("Enabling HUD");
        hud.SetActive(true);

        Debug.Log("Teleporting Player back to start");
        GameObject player = GameObject.Find("Player");
        GameObject spawn = GameObject.Find("Spawn");
        player.transform.position = spawn.transform.position;

        Debug.Log("Destroying Object");
        Destroy(gameObject);
    }

}
