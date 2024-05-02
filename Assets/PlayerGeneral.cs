using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerGeneral : MonoBehaviour
{
    [Header("Player Health")]
    public int maxHealth;
    public int currentHealth;
    public float regenRate; // Health per second
    public float timeBeforeRegen; // Seconds before regeneration starts after taking damage

    [Header("Health Bar")]
    public HealthBar healthBar;

    [Header("Player Audio")]
    public AudioSource source;
    public AudioClip[] clips;
    public AudioClip gameOver;

    [Header("Player Animation")]
    public Animator animator;

    [Header("Death Camera")]
    public Camera deathCamera;
    public Transform deathCamStartPoint;
    public Image blackScreen;
    public AudioSource gameOverSource;
    public LayerMask enemyLayerMask;

    [Header("Win")]
    public AudioSource winSource;
    public GameObject winningScreen;


    private float lastDamageTime;
    private float healthToRegen; // Accumulator for fractional health regeneration

    void Start()
    {
        currentHealth = maxHealth; // Initialize the player's health to max at the start
        healthBar.SetMaxHealth(maxHealth); healthBar.SetHealth(currentHealth);
        deathCamera.enabled = false;
    }

    private void Update()
    {
        healthBar.SetHealth(currentHealth);
    }

    public void TakeDamage(int damageAmount = 10)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damageAmount;
            currentHealth = Mathf.Max(currentHealth, 0); // Prevent health from dropping below zero
            lastDamageTime = Time.time; // Update the time damage was taken
            StopAllCoroutines(); // Stop any ongoing regeneration
            StartCoroutine(RegenerateHealth()); // Start the regeneration coroutine

            if (currentHealth <= 0)
            {
                StartCoroutine("DeathProtocol");
            }

            // Play damage taken sound
            PlaySound();
        }
    }

    public IEnumerator DeathProtocol()
    {
        GameObject.Find("DeathCam").GetComponent<Camera>().enabled = true;
        GameObject.Find("HUD").SetActive(false);
        GameObject.Find("MiniMapBorder").SetActive(false);
        GameObject.Find("Gun").SetActive(false);
        GameObject.Find("PlayerObj").GetComponent<CapsuleCollider>().excludeLayers = enemyLayerMask;

        Debug.Log("Death Protocol");
        animator.SetTrigger("Death");
        source.PlayOneShot(gameOver); // Play the game over sound

        Camera mainCamera = GameObject.Find("PlayerCam").GetComponent<Camera>();
        mainCamera.enabled = false; // Disable the main camera
        deathCamera.enabled = true; // Enable the death camera
        deathCamera.transform.position = deathCamStartPoint.position; // Set the starting position
        deathCamera.transform.rotation = deathCamStartPoint.rotation; // Set the starting rotation

        yield return new WaitForSeconds(0.5f); // Wait for the death animation to settle

        float duration = 10.0f; // Duration over which the camera moves and spins
        float elapsedTime = 0f;

        Vector3 startPosition = deathCamera.transform.position;
        Vector3 endPosition = startPosition + Vector3.up * 15; // Adjust height to move up by 5 units

        while (elapsedTime < duration)
        {
            float ratio = elapsedTime / duration;
            deathCamera.transform.position = Vector3.Lerp(startPosition, endPosition, ratio);
            deathCamera.transform.RotateAround(transform.position, Vector3.up, 20f * Time.deltaTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        float fadeDuration = 5.0f;  // Duration of the fade to black
        float fadeElapsedTime = 0f;

        while (fadeElapsedTime < fadeDuration)
        {
            fadeElapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(fadeElapsedTime / fadeDuration);
            blackScreen.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // Music fade out after the screen is fully black
        yield return new WaitForSeconds(5f); // Wait five seconds after the screen has fully blackened
        fadeElapsedTime = 0f;
        float musicFadeDuration = 5f;
        float initialVolume = gameOverSource.volume;

        while (fadeElapsedTime < musicFadeDuration)
        {
            gameOverSource.volume = Mathf.Lerp(initialVolume, 0, fadeElapsedTime / musicFadeDuration);
            fadeElapsedTime += Time.deltaTime;
            yield return null;
        }

        gameOverSource.volume = 0;  // Ensure volume is set to 0 after fading out

        yield return new WaitForSeconds(2f);
        // Send user back to main menu to start again.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }



    private IEnumerator RegenerateHealth()
    {
        // Wait for a delay before starting to regenerate health
        yield return new WaitForSeconds(timeBeforeRegen);

        while (currentHealth < maxHealth)
        {
            healthToRegen += regenRate * Time.deltaTime; // Accumulate regen amount based on regen rate and delta time
            if (healthToRegen >= 1) // Only update health when at least 1 unit has been accumulated
            {
                int regenAmount = Mathf.FloorToInt(healthToRegen);
                currentHealth += regenAmount;
                healthToRegen -= regenAmount; // Subtract the integer part used to increase health
                currentHealth = Mathf.Min(currentHealth, maxHealth); // Ensure we do not exceed max health
            }
            yield return null; // Wait until the next frame
        }
        healthToRegen = 0; // Reset the regen accumulator when done
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            TakeDamage(other.GetComponent<AIGeneral>().damage); // Assume some default damage value or adjust as needed
        }
        else if (other.CompareTag("Win"))
        {
            StartCoroutine(WinProtocol());
        }
    }

    public IEnumerator WinProtocol()
    {
        // Activate the winning screen
        winningScreen.SetActive(true);
        // Play the winning sound
        winSource.Play();
        // Wait for 5 seconds
        yield return new WaitForSeconds(5f);


        // Fade out the music
        float fadeDuration = 5.0f;  // Duration for the music fade out
        float startVolume = winSource.volume;


        while (winSource.volume > 0)
        {
            winSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        winSource.Stop();  // Stop the audio after fading out
        // Reload the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    private void PlaySound()
    {
        if (source != null && clips.Length > 0)
        {
            AudioClip clip = clips[Random.Range(0, clips.Length)];
            source.PlayOneShot(clip); // Play a random clip from the available clips
        }
    }
}
