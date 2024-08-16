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
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpMax;
    [SerializeField] int jumpSpeed;
    [SerializeField] int gravity;

    [Header("----- Guns -----")]
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;

    Vector3 move;
    Vector3 playerVel;

    int jumpCount;
    int HPOrig;

    bool isSprinting;
    bool isShooting;
    public bool isDead;
    bool isInvincible;

    private Animator myAnimator;

    // Start is called before the first frame update
    void Start()
    {
        HPOrig = HP;
        updatePlayerUI();
        myAnimator = GetComponent<Animator>();
        deathCamera.gameObject.SetActive(false);
        spawnPlayer();
    }

    public void spawnPlayer()
    {
        HP = HPOrig;
        updatePlayerUI();
        transform.position = gameManager.instance.playerSpawnPos.transform.position;

        myAnimator.SetBool("Dead", false);
        myAnimator.SetTrigger("Idle");
        myAnimator.Play("Idle");
        isDead = false;

        ZombieAI[] enemies = FindObjectsOfType<ZombieAI>();
        foreach (ZombieAI enemy in enemies)
        {
            enemy.resetTriggers();
        }

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

        // Handle jumping
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
        }

        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        if (Input.GetButton("Shoot") && !isShooting)
        {
            StartCoroutine(shoot());
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

    IEnumerator shoot()
    {
        isShooting = true;

        //RaycastHit hit;
        //if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreMask))
        //{
        //    //Debug.Log(hit.collider.name);
        //    IDamage dmg = hit.collider.GetComponent<IDamage>();

        //    if (dmg != null)
        //    {
        //        dmg.takeDamage(shootDamage);
        //    }
        //}

        // Calculate the time between shots based on the bullets per second
        float timeBetweenShots = 1f / shootRate;

        // Determine the direction the player is aiming
        Vector3 shootDirection = Camera.main.transform.forward;

        Instantiate(bullet, shootPos.position, Quaternion.LookRotation(shootDirection));

        yield return new WaitForSeconds(timeBetweenShots);
        isShooting = false;
    }

    public void takeDamage(int amount)
    {
        if (isInvincible)
            return;

        HP -= amount;
        updatePlayerUI();

        // I'm dead!
        if (HP <= 0)
        {
            StartCoroutine(HandleDeath());
        }
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
        Debug.Log("still in this for some reason");
        gameManager.instance.youLose();
    }

    public void updatePlayerUI()
    {
        gameManager.instance.PlayerHPBar.fillAmount = (float)HP / HPOrig;
    }

}