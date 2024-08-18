using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] Camera mainCamera; 
    [SerializeField] Camera deathCamera;

    [Header("----- Player Stats -----")]
    [SerializeField] int HP;
    [SerializeField] int shield;
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpMax;
    [SerializeField] int jumpSpeed;
    [SerializeField] int gravity;

    [Header("----- Guns -----")]
    [SerializeField] private KeyCode reloadKey;

    Vector3 move;
    Vector3 playerVel;

    int jumpCount;
    int HPOrig;
    int shieldOrig;

    bool isSprinting;
    bool isShooting;
    public bool isDead;
    bool isInvincible;

    public static Action shootInput;
    public static Action reloadInput;

    private Animator myAnimator;

    // Start is called before the first frame update
    void Start()
    {
        HPOrig = HP;
        shieldOrig = shield;
        updatePlayerUI();
        myAnimator = GetComponent<Animator>();
        deathCamera.gameObject.SetActive(false);
        spawnPlayer();
    }

    public void spawnPlayer()
    {
        HP = HPOrig;
        shield = shieldOrig;
        updatePlayerUI();
        transform.position = gameManager.instance.playerSpawnPos.transform.position;
       
        isDead = false;
        myAnimator.SetBool("Dead", false);
        myAnimator.Play("Grounded");


        //ZombieAI[] enemies = FindObjectsOfType<ZombieAI>();
        //foreach (ZombieAI enemy in enemies)
        //{
        //    enemy.resetTriggers();
        //}

        controller.enabled = false;

        Collider playerCollider = GetComponent<Collider>();
        if (playerCollider != null)
        {
            playerCollider.enabled = false;
        }

        if (playerCollider != null)
        {
            playerCollider.enabled = true;
        }
        controller.enabled = true;

        StartCoroutine(InvincibilityPeriod());
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.instance.isPaused && !isDead)
        {
            movement();
        }
        TriggerPull();
        sprint();
    }

    void movement()
    {
        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        // Capture player input
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Create a movement vector based on input
        move = transform.right * moveX + transform.forward * moveZ;

        // Calculate the magnitude of the movement vector
        float moveMagnitude = new Vector3(moveX, 0, moveZ).magnitude;

        // Apply the movement
        controller.Move(move * speed * Time.deltaTime);

        // Calculate the player's velocity based on movement magnitude and speed
        float velocity = moveMagnitude * speed;

        // Set the Speed parameter in the animator to control the Blend Tree
        myAnimator.SetFloat("Speed", velocity);

        // Set the x and y parameters in the animator
        myAnimator.SetFloat("LR", moveX);
        myAnimator.SetFloat("FB", moveZ);

        // Handle jumping
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
        }

        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        if (Input.GetButton("Shoot") && !isShooting && !isSprinting)
        {
            myAnimator.SetTrigger("Shoot");
        }
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            isSprinting = false;
        }
    }

    void FootStep()
    {
        // Handle the footstep event, like playing a sound or spawning a particle.
    }

    private void TriggerPull()
    {
        if (Input.GetMouseButton(0) && !isSprinting)
        {
            //Debug.Log("Shoot Input Detected");
            shootInput?.Invoke();
        }

        if (Input.GetKeyDown(reloadKey))
        {
            //Debug.Log("Shoot Input Detected");
            reloadInput?.Invoke();
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
            //player takes sheild damage
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
    //added flash damage script 
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

        // Debug.Log("still in this for some reason");
    }

    public void updatePlayerUI()
    {
        gameManager.instance.PlayerHPBar.fillAmount = (float)HP / HPOrig;
        gameManager.instance.playerShieldBar.fillAmount = (float)shield / shieldOrig;
    }

}