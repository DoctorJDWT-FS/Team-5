using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller; // Handles player movement and collisions
    [SerializeField] LayerMask ignoreMask; // LayerMask to ignore during certain checks
    [SerializeField] Camera mainCamera; // Main player camera
    [SerializeField] Camera deathCamera; // Camera used when player dies
    [SerializeField] AudioSource audioSource; // Audio source for playing sound effects
    [SerializeField] Animator myAnimator; // Animator for handling player animations
    [SerializeField] PlayerSettings playerSettings; // Reference to PlayerSettings

    [Header("----- Player Stats -----")]
    [SerializeField] int HP; // Player health points
    [SerializeField] int shield; // Player shield points
    [SerializeField] float speed; // Base movement speed
    [SerializeField] int sprintMod; // Speed multiplier when sprinting
    [SerializeField] int jumpMax; // Maximum number of jumps allowed
    [SerializeField] int jumpSpeed; // Vertical speed when jumping
    [SerializeField] int gravity; // Gravity applied to the player

    [Header("----- Movement Mechanics -----")]
    [SerializeField] float slideMultiplier = 1.5f; // Multiplier to increase speed during slide
    [SerializeField] float slideDuration = 0.5f; // Duration of the slide action
    [SerializeField] float slideCooldown = 1f; // Cooldown time between slides
    [SerializeField] float dashMultiplier = 2.5f; // Multiplier to increase speed during dash
    [SerializeField] float dashDuration = 0.2f; // Duration of the dash action
    [SerializeField] float dashCooldown = 4.5f; // Cooldown time before dashing again
    [SerializeField] AudioClip slideSound; // Sound effect for sliding
    [SerializeField] AudioClip dashSound; // Sound effect for dashing

    [Header("----- Guns -----")]
    [SerializeField] TMP_Text ammoCount; // UI element to display ammo count
    [SerializeField] TMP_Text magCount; // UI element to display magazine count

    [Header("----- Player State Variables -----")]
    Vector3 move; // Player movement vector
    Vector3 playerVel; // Player vertical velocity
    int jumpCount; // Tracks the number of jumps performed
    public int HPOrig; // Original health points for reset purposes
    int shieldOrig; // Original shield points for reset purposes

    bool isReloading = false; // Flag to check if player is reloading
    bool isShooting; // Flag to check if player is shooting
    public bool isDead; // Flag to check if player is dead
    bool isInvincible; // Flag for temporary invincibility

    [Header("----- Dash and Slide State Variables -----")]
    private bool canSlide = true; // Tracks if the player can slide
    private bool isSliding = false; // Tracks if the player is currently sliding
    private bool canDash = true; // Tracks if the player can dash
    private bool isDashing = false; // Tracks if the player is currently dashing
    private bool isSprinting = false; // Tracks if the player is currently sprinting
    private float originalSpeed; // Stores the player's original speed
    private float originalHeight; // Stores the player's original height
    private float dashTimer; // Timer to track dash cooldown

    [Header("----- Player Actions -----")]
    public static Action shootInput; // Action event for shooting
    public static Action reloadInput; // Action event for reloading

    public gun currentGun; // Current gun equipped by the player

    // Start is called before the first frame update
    void Start()
    {
        // Initialize player stats and components
        HPOrig = HP;
        shieldOrig = shield;
        updatePlayerUI();
        myAnimator = GetComponent<Animator>();
        deathCamera.gameObject.SetActive(false);
        spawnPlayer();
        audioSource = GetComponent<AudioSource>();

        // Store original values for speed and height
        originalSpeed = speed;
        originalHeight = controller.height;
        dashTimer = dashCooldown; // Initialize dash timer
    }

    // Respawn player at the start of the game or after death
    public void spawnPlayer()
    {
        HP = HPOrig;
        shield = shieldOrig;
        updatePlayerUI();
        transform.position = gameManager.instance.playerSpawnPos.transform.position;

        isDead = false;
        myAnimator.SetBool("Dead", false);
        myAnimator.Play("Grounded");

        // Reset player collider and controller
        controller.enabled = false;
        Collider playerCollider = GetComponent<Collider>();
        if (playerCollider != null)
        {
            playerCollider.enabled = false;
            playerCollider.enabled = true;
        }
        controller.enabled = true;

        gameManager.instance.loadSettings();
        StartCoroutine(InvincibilityPeriod());
    }

    void Update()
    {
        // Only update when the game is not paused and player is alive
        if (!gameManager.instance.isPaused && !isDead)
        {
            movement();
            Slide(); // Handle sliding
            HandleDashAndSprint(); // Handle dashing and sprinting input
            UpdateDashTimer(); // Update the dash timer
        }
        TriggerPull(); // Handle shooting input
        currentGun = GetComponentInChildren<gun>(); // Get current gun
        UpdateWeaponType(); // Update weapon UI
    }

    // Handle player movement and jumping
    void movement()
    {
        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        // Capture player input for movement
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        move = transform.right * moveX + transform.forward * moveZ; // Create movement vector
        controller.Move(move * speed * Time.deltaTime); // Apply movement

        // Update animator parameters for movement
        myAnimator.SetFloat("Speed", speed);
        myAnimator.SetFloat("LR", moveX);
        myAnimator.SetFloat("FB", moveZ);

        // Handle jumping
        if (Input.GetKeyDown(playerSettings.jump) && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
        }

        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;
    }

    void Slide()
    {
        // Slide only if the player is sprinting and not already sliding
        if (Input.GetKeyDown(playerSettings.slide) && canSlide && isSprinting && !isSliding)
        {
            StartCoroutine(PerformSlide());
        }
    }

    private IEnumerator PerformSlide()
    {
        isSliding = true;
        canSlide = false;

        // Play slide sound effect
        if (slideSound != null)
        {
            audioSource.PlayOneShot(slideSound);
        }

        // Temporarily increase speed and reduce height
        speed *= slideMultiplier;
        controller.height = originalHeight / 2;

        yield return new WaitForSeconds(slideDuration);

        // Reset speed and height to normal
        speed = originalSpeed;
        controller.height = originalHeight;
        isSliding = false;

        // Wait for cooldown
        yield return new WaitForSeconds(slideCooldown);
        canSlide = true;
    }

    void HandleDashAndSprint()
    {
        // Dash on LeftShift press, sprint on hold
        if (Input.GetKeyDown(playerSettings.dash) && canDash && !isSprinting && !isDashing)
        {
            StartCoroutine(PerformDash());
        }
        else if (Input.GetKey(playerSettings.sprint) && !isDashing && !isSliding && IsMovingForwardOnly())
        {
            StartSprinting();
        }
        else if (Input.GetKeyUp(playerSettings.sprint))
        {
            StopSprinting();
        }
    }

    private IEnumerator PerformDash()
    {
        isDashing = true;
        canDash = false;
        dashTimer = 0; // Reset dash timer

        // Play dash sound effect
        if (dashSound != null)
        {
            audioSource.PlayOneShot(dashSound);
        }

        // Temporarily increase speed
        speed *= dashMultiplier;

        yield return new WaitForSeconds(dashDuration);

        // Reset speed to normal
        speed = originalSpeed;
        isDashing = false;
    }

    void UpdateDashTimer()
    {
        if (dashTimer < dashCooldown)
        {
            dashTimer += Time.deltaTime; // Increment dash timer
        }
        else if (dashTimer >= dashCooldown)
        {
            canDash = true; // Allow dashing again
        }
    }

    void StartSprinting()
    {
        if (!isSprinting)
        {
            speed *= sprintMod;
            isSprinting = true;
        }
    }

    void StopSprinting()
    {
        if (isSprinting)
        {
            speed = originalSpeed; // Reset speed after sprinting
            isSprinting = false;
        }
    }

    // Check if the player is moving forward only
    bool IsMovingForwardOnly()
    {
        float moveZ = Input.GetAxis("Vertical");
        float moveX = Input.GetAxis("Horizontal");

        return moveZ > 0 && moveX == 0; // True if moving forward and not strafing
    }

    void FootStep()
    {
        // Handle the footstep event, like playing a sound or spawning a particle.
    }

    private void TriggerPull()
    {
        // Check if the game is paused by the gameManager or if the shop exists and is paused
        if (gameManager.instance.isPaused || (shopInteractable.instance != null && shopInteractable.instance.isPaused))
            return;

        // Use the key from PlayerSettings for shooting
        if (Input.GetKey(playerSettings.shoot) && !isSprinting && !isReloading)
        {
            Debug.Log("Shoot Input Detected");
            myAnimator.SetTrigger("Shoot");
            shootInput?.Invoke();
        }

        // Use the key from PlayerSettings for reloading
        if (Input.GetKeyDown(playerSettings.reload))
        {
            Debug.Log("Reload Input Detected");
            reloadInput?.Invoke();
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        if (currentGun == null)
        {
            yield break;
        }

        myAnimator.SetBool("Reloading", true);
        isReloading = true;
        yield return new WaitForSeconds(currentGun.reloadTime + 0.6f);
        myAnimator.SetBool("Reloading", false);
        isReloading = false;
    }


    public void UpdateWeaponType()
    {
        // If currentGun or magCount or ammoCount is null, return early and do nothing
        if (currentGun == null || magCount == null || ammoCount == null)
            return;

        // Check for specific weapon types
        if (currentGun.weaponType == gun.WeaponType.Pistol)
        {
            myAnimator.SetBool("Pistol", true);
            myAnimator.SetBool("Rifle", false);
            magCount.text = currentGun.currentMagazines + " ";
            ammoCount.text = currentGun.currentAmmo + " ";
        }
        else if (currentGun.weaponType == gun.WeaponType.Rifle)
        {
            myAnimator.SetBool("Pistol", false);
            myAnimator.SetBool("Rifle", true);
            magCount.text = currentGun.currentMagazines + " ";
            ammoCount.text = currentGun.currentAmmo + " ";
        }
        else
        {
            // Handle other weapon types or default to no weapon
            myAnimator.SetBool("Pistol", false);
            myAnimator.SetBool("Rifle", false);
        }
    }

    public void takeDamage(int amount)
    {
        if (isInvincible)
            return;
        //if player has shield , player wont take damage 
        if (shield <= 0)
        {
            HP -= amount;
            StartCoroutine(flashDamage());
            updatePlayerUI();
        }
        else
        {
            //player takes shield damage
            shield -= amount;
            StartCoroutine(shieldDamage());
            updatePlayerUI();
        }

        // I'm dead!
        if (HP <= 0)
        {
            StartCoroutine(HandleDeath());
        }
    }

    public void addHealth(int health)
    {
        if (HPOrig <= HP + health)
        {
            HP = HPOrig;
            updatePlayerUI();
        }
        else
        {
            HP += health;
            updatePlayerUI();
        }
    }

    public void addShield(int _shield)
    {
        if (shieldOrig <= shield + _shield)
        {
            shield = shieldOrig;
            updatePlayerUI();
        }
        else
        {
            shield += _shield;
            updatePlayerUI();
        }
    }

    public void addAmmo(int _Ammo, int _Mags)
    {
        if (currentGun.maxMagazines <= currentGun.currentMagazines + _Mags)
        {
            currentGun.currentMagazines = currentGun.maxMagazines;
            updatePlayerUI();
        }
        else
        {
            currentGun.currentMagazines += _Mags;
            updatePlayerUI();
        }

        if (currentGun.magSize <= currentGun.currentAmmo + _Ammo)
        {
            currentGun.currentAmmo = currentGun.magSize;
            updatePlayerUI();
        }
        else
        {
            currentGun.currentAmmo += _Ammo;
        }
    }

    public void increaseMaxHealth(int amount)
    {
        HPOrig += amount;
        HP += amount;

        if (HP > HPOrig)
        {
            HP = HPOrig;
        }
        addHealth(HPOrig - HP);
        updatePlayerUI();
    }

    public void increaseMaxShield(int amount)
    {
        shieldOrig += amount;
        shield += amount;

        if (shield > shieldOrig)
        {
            shield = shieldOrig;
        }

        updatePlayerUI();
    }

    // Flash damage visual effect
    private IEnumerator flashDamage()
    {
        gameManager.instance.flashDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.flashDamageScreen.SetActive(false);
    }

    private IEnumerator shieldDamage()
    {
        gameManager.instance.flashShieldDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.flashShieldDamageScreen.SetActive(false);
    }

    IEnumerator InvincibilityPeriod()
    {
        isInvincible = true;
        yield return new WaitForSeconds(1.0f);
        isInvincible = false;
    }

    private IEnumerator HandleDeath()
    {
        isDead = true;

        // Play death animation
        myAnimator.SetBool("Dead", true);

        // Switch to the death camera
        mainCamera.gameObject.SetActive(false);
        deathCamera.gameObject.SetActive(true);

        // Call the youLose method after the delay
        yield return new WaitForSeconds(4.0f);
        gameManager.instance.youLose();
    }

    public void updatePlayerUI()
    {
        gameManager.instance.PlayerHPBar.fillAmount = (float)HP / HPOrig;
        gameManager.instance.playerShieldBar.fillAmount = (float)shield / shieldOrig;
    }
}
