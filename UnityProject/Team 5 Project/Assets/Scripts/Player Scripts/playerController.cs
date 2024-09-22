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
    [SerializeField] Throwgrenade grenade; // Reference to ThrowGrenade

    [Header("----- Shield Sound Effects -----")]
    [SerializeField] private AudioClip shieldBreakSound;  
    [SerializeField] private AudioClip shieldChargedSound;
    [SerializeField] private AudioClip[] takeDamageSounds;
    [SerializeField] private AudioClip shieldBeepSound;


    [Header("----- Hand Collider -----")]
    [SerializeField] Collider handCollider; // Reference to the hand collider

    [Header("----- Punch Settings -----")]
    [SerializeField] float punchDuration = 0.5f; // Duration the hand collider is enabled during a punch
    [SerializeField] int punchDamage = 50; // Amount of damage the punch does
    [SerializeField] float punchCooldownTime = 1.5f;
    [SerializeField] private AudioClip shieldOverloadSound;

    [Header("----- Player Stats -----")]
    [SerializeField] public int HP; // Player health points
    [SerializeField] public int shield; // Player shield points
    [SerializeField] public float speed; // Base movement speed
    [SerializeField] int sprintMod; // Speed multiplier when sprinting
    [SerializeField] int jumpMax; // Maximum number of jumps allowed
    [SerializeField] int jumpSpeed; // Vertical speed when jumping
    [SerializeField] int gravity; // Gravity applied to the player
    [SerializeField] Transform grenadeThrowPos;

    [Header("----- Movement Mechanics -----")]
    [SerializeField] float slideMultiplier = 1.5f; // Multiplier to increase speed during slide
    [SerializeField] float slideDuration = 0.5f; // Duration of the slide action
    [SerializeField] float slideCooldown = 1f; // Cooldown time between slides
    [SerializeField] float dashMultiplier = 2.5f; // Multiplier to increase speed during dash
    [SerializeField] float dashDuration = 0.2f; // Duration of the dash action
    [SerializeField] float dashCooldown = 4.5f; // Cooldown time before dashing again
    [SerializeField] AudioClip slideSound; // Sound effect for sliding
    [SerializeField] AudioClip dashSound; // Sound effect for dashing
    [SerializeField] private AudioClip[] footstepSounds;  // Sound effect for footstep

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

    public bool regenEnabled;
    private bool isRegenerating;

    [Header("----- Dash and Slide State Variables -----")]
    private bool canSlide = true; // Tracks if the player can slide
    private bool isSliding = false; // Tracks if the player is currently sliding
    private bool canDash = true; // Tracks if the player can dash
    private bool isDashing = false; // Tracks if the player is currently dashing
    private bool isSprinting = false; // Tracks if the player is currently sprinting
    private float originalSpeed; // Stores the player's original speed
    private float originalHeight; // Stores the player's original height
    private float dashTimer; // Timer to track dash cooldown
    private Coroutine shieldBeepCoroutine;


    [Header("----- Player Actions -----")]
    public static Action shootInput; // Action event for shooting
    public static Action reloadInput; // Action event for reloading
    private bool canPunch = true;
    public gun currentGun; // Current gun equipped by the player

    void Start()
    {
        // Initialize player stats and components
        HPOrig = HP;
        shieldOrig = shield;
        originalSpeed = speed;
        originalHeight = controller.height;
        updatePlayerUI();   
        myAnimator = GetComponent<Animator>();
        deathCamera.gameObject.SetActive(false);
        spawnPlayer();
        audioSource = GetComponent<AudioSource>();

        // Store original values for speed and height
        dashTimer = dashCooldown;

        // Disable the hand collider at the start
        if (handCollider != null)
        {
            handCollider.enabled = false;
        }
    }

    // Method to respawn the player at the start of the game or after death
    public void spawnPlayer()
    {
        HP = HPOrig;
        shield = shieldOrig;
        speed = originalSpeed;
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
            Slide();
            HandleDashAndSprint();
            UpdateDashTimer();
            HandlePunch();
            throwGrenade();
            if (!isRegenerating && shield <= (shieldOrig - 5))
            {
                StartCoroutine(Regen(3, 5));
            }
        }
        TriggerPull();
        currentGun = GetComponentInChildren<gun>();
        UpdateWeaponType();
    }
    
    // Method to handle player movement and jumping
    void movement()
    {
        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * speed * Time.deltaTime);

        myAnimator.SetFloat("Speed", speed);
        myAnimator.SetFloat("LR", moveX);
        myAnimator.SetFloat("FB", moveZ);

        if (Input.GetKeyDown(playerSettings.jump) && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
        }

        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;
    }

    // Method to handle sliding input
    void Slide()
    {
        if (Input.GetKeyDown(playerSettings.slide) && canSlide && isSprinting && !isSliding)
        {
            StartCoroutine(PerformSlide());
        }
    }

    // Coroutine to perform the sliding action
    private IEnumerator PerformSlide()
    {
        isSliding = true;
        canSlide = false;

        if (slideSound != null)
        {
            audioSource.PlayOneShot(slideSound);
        }

        speed *= slideMultiplier;
        controller.height = originalHeight / 2;

        yield return new WaitForSeconds(slideDuration);

        speed = originalSpeed;
        controller.height = originalHeight;
        isSliding = false;

        yield return new WaitForSeconds(slideCooldown);
        canSlide = true;
    }

    // Method to handle dash and sprint input
    void HandleDashAndSprint()
    {
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

    // Coroutine to perform the dash action
    private IEnumerator PerformDash()
    {
        isDashing = true;
        canDash = false;
        dashTimer = 0;

        if (dashSound != null)
        {
            audioSource.PlayOneShot(dashSound);
        }

        speed *= dashMultiplier;

        yield return new WaitForSeconds(dashDuration);

        speed = originalSpeed;
        isDashing = false;
    }

    // Method to update the dash timer
    void UpdateDashTimer()
    {
        if (dashTimer < dashCooldown)
        {
            dashTimer += Time.deltaTime;
        }
        else if (dashTimer >= dashCooldown)
        {
            canDash = true;
        }
    }

    // Method to start sprinting
    void StartSprinting()
    {
        if (!isSprinting)
        {
            speed *= sprintMod;
            isSprinting = true;
        }
    }

    // Method to stop sprinting
    void StopSprinting()
    {
        if (isSprinting)
        {
            speed = originalSpeed;
            isSprinting = false;
        }
    }

    // Check if the player is moving forward only (no strafing)
    bool IsMovingForwardOnly()
    {
        float moveZ = Input.GetAxis("Vertical");
        float moveX = Input.GetAxis("Horizontal");

        return moveZ > 0 && moveX == 0;
    }

    void FootStep()
    {
        // Placeholder for footstep logic
    }

    // Method to handle punch input
    void HandlePunch()
    {
        if (Input.GetKeyDown(playerSettings.punch))
        {
            Punch();
        }
    }

    // Method to perform a punch
    // Method to perform a punch
    void Punch()
    {
        if (!canPunch)
            return; // Exit if punch is on cooldown

        // Play the shield overload sound when punching
        if (shieldOverloadSound != null)
        {
            audioSource.PlayOneShot(shieldOverloadSound);
        }

        // Enable the hand collider for dealing damage
        if (handCollider != null)
        {
            handCollider.enabled = true; // Enable the hand collider
            StartCoroutine(DisableHandColliderAfterPunch());
        }

        // Start cooldown for punch
        StartCoroutine(PunchCooldown());
    }

    // Coroutine to manage the punch cooldown
    private IEnumerator PunchCooldown()
    {
        canPunch = false; // Disable punching
        yield return new WaitForSeconds(punchCooldownTime); // Wait for cooldown duration
        canPunch = true; // Re-enable punching
    }

    // Punch collision
    private void OnTriggerEnter(Collider other)
    {
        if (handCollider != null && handCollider.enabled)
        {
            IDamage target = other.GetComponent<IDamage>();
            if (target != null)
            {
                target.takeDamage(punchDamage); // Apply punch damage
            }
        }
    }

    // Coroutine to disable the hand collider after the punch duration
    private IEnumerator DisableHandColliderAfterPunch()
    {
        yield return new WaitForSeconds(punchDuration);
        if (handCollider != null)
        {
            handCollider.enabled = false;
        }
    }

    // Method to handle shooting and reloading input
    private void TriggerPull()
    {
        if (gameManager.instance.isPaused || (shopInteractable.instance != null && shopInteractable.instance.isPaused))
            return;

        if (Input.GetKey(playerSettings.shoot) && !isSprinting && !isReloading)
        {
            Debug.Log("Shoot Input Detected");
            myAnimator.SetTrigger("Shoot");
            shootInput?.Invoke();
        }

        if (Input.GetKeyDown(playerSettings.reload))
        {
            Debug.Log("Reload Input Detected");
            reloadInput?.Invoke();
            StartCoroutine(Reload());
        }
    }

    // Coroutine to handle reloading logic
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

    // Update weapon UI based on current weapon type
    public void UpdateWeaponType()
    {
        if (currentGun == null || magCount == null || ammoCount == null)
            return;

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
            myAnimator.SetBool("Pistol", false);
            myAnimator.SetBool("Rifle", false);
        }
    }

    public void takeDamage(int amount)
    {
        if (isInvincible)
            return;

        // Play a random damage sound when the player takes damage
        if (takeDamageSounds != null && takeDamageSounds.Length > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, takeDamageSounds.Length);
            audioSource.PlayOneShot(takeDamageSounds[randomIndex]);
        }

        // If the player has shield
        if (shield > 0)
        {
            // Reduce the shield by the damage amount
            shield -= amount;

            // Clamp shield to 0 if it goes negative
            if (shield < 0)
            {
                shield = 0;
            }

            // Play shield break sound if shield reaches 0
            if (shield == 0)
            {
                if (shieldBreakSound != null)
                {
                    audioSource.PlayOneShot(shieldBreakSound);
                }

                if (shieldBeepSound != null && shieldBeepCoroutine == null)
                {
                    shieldBeepCoroutine = StartCoroutine(PlayShieldBeep());
                }
            }
            updatePlayerUI();
            StartCoroutine(shieldDamage());
        }
        else
        {
            // If no shield, apply damage to health
            HP -= amount;
            StartCoroutine(flashDamage());
            updatePlayerUI();
        }

        // Check if player has died
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

        // Stop the shield beep if shield is restored
        if (shield > 0 && shieldBeepCoroutine != null)
        {
            StopCoroutine(shieldBeepCoroutine);
            shieldBeepCoroutine = null;
        }
    }

    // Plays a beeping sound every 1 second while shield is 0 or less.
    private IEnumerator PlayShieldBeep()
    {
        while (shield <= 0)
        {
            if (shieldBeepSound != null)
            {
                audioSource.PlayOneShot(shieldBeepSound);
            }
            yield return new WaitForSeconds(1f);
        }
    }
    public void applySlow(float i, float b)
    {

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

    public void throwGrenade()
    {
        if (!isDead && !isSprinting && !isDashing && !isShooting)
        {
            if (Input.GetKeyDown(playerSettings.grenade))
            {
                grenade.startHoldingGrenade(grenade.currentGrenadeStats);
            }
            if (Input.GetKeyUp(playerSettings.grenade))
            {
                grenade.stopHoldingGrenade();
            }
        }
    }

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

        myAnimator.SetBool("Dead", true);

        mainCamera.gameObject.SetActive(false);
        deathCamera.gameObject.SetActive(true);

        yield return new WaitForSeconds(4.0f);
        gameManager.instance.youLose();
    }

    IEnumerator Regen(int delay, int amount)
    {
        isRegenerating = true;
        yield return new WaitForSeconds(delay);
        addShield(amount);
        isRegenerating = false;
    }

    //Play Footstep Sound
    public void PlayFootstepSound()
    {
        int randomIndex = UnityEngine.Random.Range(0, footstepSounds.Length);
        audioSource.PlayOneShot(footstepSounds[randomIndex]);
    }

    public void updatePlayerUI()
    {
        if (shield < 0)
        {
            shield = 0;
        }
        gameManager.instance.PlayerHPBar.fillAmount = (float)HP / HPOrig;
        gameManager.instance.playerHp = HPOrig - HP;
        gameManager.instance.playerShieldBar.fillAmount = (float)shield / shieldOrig;
        gameManager.instance.playerShield = shieldOrig - shield;
        gameManager.instance.UpdateHealthCount();
        gameManager.instance.UpdateSheildCount();
    }
}
