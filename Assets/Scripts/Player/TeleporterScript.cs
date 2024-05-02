using System.Collections;
using UnityEngine;

public class TeleporterScript : MonoBehaviour
{
    public GameObject siblingTeleporter; // The linked teleporter object
    public bool isTeleporting; // Controls teleport state to prevent loops
    public AudioSource source;

    private void Start()
    {
        isTeleporting = false; // Initially, teleporting should be allowed
        source = GameObject.Find("Teleporters").GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTeleporting)
        {
            // Start the teleportation process for both this and the sibling teleporter
            StartTeleportProcess(other.gameObject);
        }
    }

    private void StartTeleportProcess(GameObject player)
    {
        if (siblingTeleporter != null)
        {
            // Set both teleporters to prevent further teleportation
            isTeleporting = true;
            siblingTeleporter.GetComponent<TeleporterScript>().isTeleporting = true;

            // Teleport the player
            player.transform.parent.transform.position = siblingTeleporter.transform.position;
            //player.transform.parent.transform.rotation = siblingTeleporter.GetComponent<TeleporterScript>().orien.transform.rotation;
            //GameObject.Find("CameraHolder").transform.rotation = siblingTeleporter.GetComponent<TeleporterScript>().orien.transform.rotation;
            Debug.Log("Here");
            source.Play();

            // Start cooldowns for both teleporters
            StartCoroutine(delayUntilTeleporting());
            StartCoroutine(siblingTeleporter.GetComponent<TeleporterScript>().delayUntilTeleporting());
        }
    }

    private IEnumerator delayUntilTeleporting()
    {
        yield return new WaitForSeconds(2f); // Delay to prevent immediate re-teleportation
        isTeleporting = false;
    }
}
