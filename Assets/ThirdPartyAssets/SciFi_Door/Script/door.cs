using TMPro;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject theDoor;
    private Animation doorAnimation;
    public bool opened;
    public bool playerIn;
    public Light left;
    public Light right;
    public bool locked;

    public Camera doorCam;

    public AudioSource buzzer;

    [Header("Interactable")]
    public Transform camera;
    public GameObject lockedMessage;
    public LayerMask intereactableMask;
    public float interactableRange = 5f;


    private void Start()
    {
        doorAnimation = theDoor.GetComponent<Animation>();
        opened = false;
        playerIn = false;
        locked = true;
        doorCam.enabled = false;
    }

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, interactableRange, intereactableMask))
        {
            if ((hit.collider.gameObject == transform.gameObject) && (locked))
            {
                lockedMessage.SetActive(true);
            }
            else
            {
                lockedMessage.SetActive(false);
            }
        }
        else
        {
            lockedMessage.SetActive(false);
        }



        if (opened && !playerIn)
        {
            PlayAnimation("close");
        }
    }

    void OnTriggerEnter(Collider obj)
    {
        if (obj.CompareTag("Player"))
        {
            playerIn = true;
            if (!opened && !locked)
            {
                PlayAnimation("open");
            }

        }
    }

    void OnTriggerExit(Collider obj)
    {
        if (obj.CompareTag("Player"))
        {
            playerIn = false;
            if (opened && !locked)
            {
                PlayAnimation("close");
            }
        }
    }

    private void PlayAnimation(string animationName)
    {
        doorAnimation.Play(animationName);
    }

    public void setLightsGreen()
    {
        left.color = Color.green;
        right.color = Color.green;
    }
}
