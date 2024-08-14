using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;

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

    Vector3 move;
    Vector3 playerVel;

    int jumpCount;
    int HPOrig;

    bool isSprinting;
    bool isShooting;
    public bool isDead;
    bool isInvincible;

    public Animator myAnimator;

    // Start is called before the first frame update
    void Start()
    {
        HPOrig = HP;
        updatePlayerUI();
        myAnimator = GetComponent<Animator>();
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

        move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // Define a small threshold to ignore tiny movement inputs
        float movementThreshold = 0.1f;

        // Check if the player is actually moving beyond the threshold
        bool isMoving = move.magnitude > movementThreshold;

        // Set the Walking parameter in the animator
        myAnimator.SetBool("Walking", isMoving);

        // Apply the movement
        transform.position += move * speed * Time.deltaTime;

        move = Input.GetAxis("Vertical") * transform.forward +
               Input.GetAxis("Horizontal") * transform.right;
        controller.Move(move * speed * Time.deltaTime);

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
        }
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            myAnimator.SetBool("Sprint", true);
            speed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            myAnimator.SetBool("Sprint", false);
            speed /= sprintMod;
            isSprinting = false;
        }
    }



    IEnumerator shoot()
    {
        isShooting = true;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreMask))
        {
            //Debug.Log(hit.collider.name);
            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }
        }
        yield return new WaitForSeconds(shootRate);
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
            isDead = true;
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
        // Play death animation
        myAnimator.SetBool("Dead", isDead);

        // Restrict movement

        // Wait for 2 seconds 
        yield return new WaitForSeconds(4.0f);

        // Call the youLose method after the delay
        Debug.Log("still in this for some reason");
        gameManager.instance.youLose();
    }

    public void updatePlayerUI()
    {
        gameManager.instance.PlayerHPBar.fillAmount = (float)HP / HPOrig;
    }

}
