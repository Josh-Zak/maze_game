using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIGeneral : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;
    public int damage;

    public float timeToDespawn;
    public float damageCooldown = 0.1f; // Cooldown duration in seconds
    private float lastDamageTime = -1;

    public Animator animator;
    public AudioSource audioSource;  // Assume you have one AudioSource for simplicity
    public AudioClip[] damageSounds; // Array of damage sound clips
    public Transform playerTransform;  // Assign this to reference the player transform

    public LayerMask playerLayer;
    public LayerMask bulletLayer;

    void Awake()
    {
        animator.speed = 2f;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform; // Find and assign the player transform
    }

    public void TakeDamage(int incomingDamage)
    {
        if (Time.time < lastDamageTime + damageCooldown) return; // Check if within cooldown period

        PlayRandomDamageSound();
        curHealth -= incomingDamage;
        lastDamageTime = Time.time; // Update last damage time

        if (curHealth <= 0)
        {
            curHealth = 0;
            animator.SetTrigger("Death");
            gameObject.GetComponent<NavMeshAgent>().enabled = false;
            gameObject.GetComponent<AIMovement>().enabled = false;
            gameObject.GetComponent<CapsuleCollider>().excludeLayers += bulletLayer;
            gameObject.GetComponent<CapsuleCollider>().excludeLayers += playerLayer;
        }
    }

    void PlayRandomDamageSound()
    {
        if (audioSource != null && playerTransform != null)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            float volume = Mathf.Clamp(1 - distance / audioSource.maxDistance, 0.1f, 1);  // Clamped to avoid complete silence

            if (!audioSource.isPlaying)
            {
                AudioClip clip = damageSounds[Random.Range(0, damageSounds.Length)];
                audioSource.PlayOneShot(clip, volume);
            }
        }
    }

    public IEnumerator destroyEnemy()
    {
        yield return new WaitForSeconds(timeToDespawn);
        Destroy(gameObject);
    }
}
