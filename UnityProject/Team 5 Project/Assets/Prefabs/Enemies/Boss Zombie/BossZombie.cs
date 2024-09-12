using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BossZombieAI : MonoBehaviour, IDamage
{
    [Header("----- Boss Stats -----")]
    [SerializeField] private float walkingSpeed = 4f; // Normal walking speed
    [SerializeField] private float sprintSpeed = 5.5f; // Sprinting speed for chasing
    [SerializeField] private float accelerationTime = 2f; // Time to reach max speed
    [SerializeField] private float decelerationTime = 2f; // Time to stop
    [SerializeField] private int maxHP = 300; // Maximum health points
    [SerializeField] private float rageDuration = 15f; // Time before entering rage state
    [SerializeField] private float rageMultiplier = 2f; // Speed multiplier in rage state

    [Header("----- Attack Settings -----")]

    [Header("----- Attack 1 -----")]
    [SerializeField] private float attack1Range = 5f; // Range for Attack 1
    [SerializeField] private float attack1Cooldown = 8f; // Cooldown for Attack 1
    [SerializeField] private int attack1Damage = 50; // Damage for Attack 1
    [SerializeField] private float attack1AnimationDuration = 2f; // Animation duration for Attack 1

    [Header("----- Attack 2 -----")]
    [SerializeField] private float attack2MinRange = 5f; // Minimum range for Attack 2
    [SerializeField] private float attack2MaxRange = 8f; // Maximum range for Attack 2
    [SerializeField] private float attack2Cooldown = 15f; // Cooldown for Attack 2
    [SerializeField] private int attack2Damage = 75; // Damage for Attack 2
    [SerializeField] private float attack2AnimationDuration = 2.2f; // Animation duration for Attack 2

    [Header("----- Attack 3 -----")]
    [SerializeField] private float attack3MinRange = 0f; // Minimum range for Attack 3
    [SerializeField] private float attack3MaxRange = 5f; // Maximum range for Attack 3
    [SerializeField] private float attack3Cooldown = 8f; // Cooldown for Attack 3
    [SerializeField] private int attack3Damage = 60; // Damage for Attack 3
    [SerializeField] private float attack3AnimationDuration = 2.5f; // Animation duration for Attack 3

    [Header("----- Attack 4 -----")]
    [SerializeField] private float attack4Range = 4f; // Range for Attack 4
    [SerializeField] private float attack4Cooldown = 8f; // Cooldown for Attack 4
    [SerializeField] private int attack4Damage = 60; // Damage for Attack 4
    [SerializeField] private float attack4AnimationDuration = 3f; // Animation duration for Attack 4

    [Header("----- Components -----")]
    [SerializeField] private NavMeshAgent agent; // Navigation component
    [SerializeField] private Animator myAnimator; // Animator for handling animations
    [SerializeField] private Collider rightHandCollider; // Collider for the right hand
    [SerializeField] private Collider leftHandCollider; // Collider for the left hand

    [Header("----- Player Tracking -----")]
    private Transform playerTransform; // Reference to the player's position
    private float currentSpeed; // Current movement speed
    private float nextAttack1Time = 0f; // Cooldown timer for Attack 1
    private float nextAttack2Time = 0f; // Cooldown timer for Attack 2
    private float nextAttack3Time = 0f; // Cooldown timer for Attack 3
    private float nextAttack4Time = 0f; // Cooldown timer for Attack 4
    private bool isDead = false; // Tracks if the boss is dead
    private int currentHP; // Current health points
    private bool isAttacking = false; // Tracks if the boss is currently attacking

    // Rage state variables
    private float timeSinceLastAttack = 0f; // Timer to track time since the last attack
    private bool isInRageState = false; // Tracks if the boss is in rage state

    private void Start()
    {
        playerTransform = gameManager.instance.player.transform; // Get the player's transform
        myAnimator = GetComponent<Animator>(); // Get the Animator component
        currentSpeed = 0f; // Initialize speed
        agent.speed = 0f; // Start with zero speed
        currentHP = maxHP; // Set the current HP to max

        // Disable hand colliders initially
        rightHandCollider.enabled = false;
        leftHandCollider.enabled = false;
    }

    private void Update()
    {
        if (isDead) return;  // If dead, do nothing

        // Countdown cooldowns for all attacks
        if (nextAttack1Time > 0) nextAttack1Time -= Time.deltaTime;
        if (nextAttack2Time > 0) nextAttack2Time -= Time.deltaTime;
        if (nextAttack3Time > 0) nextAttack3Time -= Time.deltaTime;
        if (nextAttack4Time > 0) nextAttack4Time -= Time.deltaTime;

        // Update rage timer
        timeSinceLastAttack += Time.deltaTime;
        CheckRageState(); // Check if the boss should enter rage state

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position); // Calculate distance to player

        // If already attacking, wait until attack is complete
        if (isAttacking) return;

        // Check for attack conditions
        if (CheckForAttack(distanceToPlayer)) return;

        // Handle movement based on state
        if (isInRageState)
        {
            HandleRageMovement(); // Handle movement in rage state
        }
        else
        {
            HandleMovement(distanceToPlayer); // Normal movement
        }
    }

    private void CheckRageState()
    {
        // Check if it's time to enter rage state
        if (timeSinceLastAttack >= rageDuration && !isInRageState)
        {
            EnterRageState();
        }
    }

    private void EnterRageState()
    {
        isInRageState = true;
        Debug.Log("BossZombie has entered Rage State!");

        // Trigger rage animation and stop agent
        myAnimator.SetTrigger("rage");
        myAnimator.SetBool("isRaging", true); // Set the isRaging bool to true
        agent.isStopped = true; // Stop the NavMeshAgent while rage animation plays

        // Start coroutine to handle the rage animation and resume chasing
        StartCoroutine(HandleRageAnimation());
    }

    private IEnumerator HandleRageAnimation()
    {
        // Wait until rage animation completes
        while (!myAnimator.GetCurrentAnimatorStateInfo(0).IsName("rage"))
        {
            yield return null;
        }

        yield return new WaitForSeconds(myAnimator.GetCurrentAnimatorStateInfo(0).length);

        // Resume chasing with increased speed after rage
        agent.isStopped = false;
        walkingSpeed *= rageMultiplier;
        sprintSpeed *= rageMultiplier;
        agent.speed = sprintSpeed;
    }

    private void HandleRageMovement()
    {
        // Continue chasing player at increased speed
        agent.SetDestination(playerTransform.position);
        myAnimator.SetFloat("Speed", agent.speed);
    }

    private void ExitRageState()
    {
        // Exit rage state and reset to normal speed
        if (isInRageState)
        {
            isInRageState = false;
            myAnimator.SetBool("isRaging", false); // Reset the bool
            Debug.Log("BossZombie has exited Rage State.");
            walkingSpeed /= rageMultiplier;
            sprintSpeed /= rageMultiplier;
            agent.speed = walkingSpeed;
        }
    }

    private bool CheckForAttack(float distanceToPlayer)
    {
        // Determine which attack to perform based on distance and cooldowns
        if (distanceToPlayer <= attack1Range && nextAttack1Time <= 0)
        {
            PerformAttack1();
            return true;
        }
        else if (distanceToPlayer >= attack2MinRange && distanceToPlayer <= attack2MaxRange && nextAttack2Time <= 0)
        {
            PerformAttack2();
            return true;
        }
        else if (distanceToPlayer >= attack3MinRange && distanceToPlayer <= attack3MaxRange && nextAttack3Time <= 0)
        {
            PerformAttack3();
            return true;
        }
        else if (distanceToPlayer <= attack4Range && nextAttack4Time <= 0)
        {
            PerformAttack4();
            return true;
        }

        return false; // No valid attack conditions met
    }

    private void PerformAttack1()
    {
        Debug.Log("Performing Attack 1");
        myAnimator.SetTrigger("Attack1");
        nextAttack1Time = attack1Cooldown;
        isAttacking = true;
        ResetRageState(); // Exit rage and reset the rage timer

        StartCoroutine(ManageCollidersDuringAttack(attack1AnimationDuration));
    }

    private void PerformAttack2()
    {
        Debug.Log("Performing Attack 2");
        myAnimator.SetTrigger("Attack2");
        nextAttack2Time = attack2Cooldown;
        isAttacking = true;
        ResetRageState(); // Exit rage and reset the rage timer

        StartCoroutine(ManageCollidersDuringAttack(attack2AnimationDuration));
    }

    private void PerformAttack3()
    {
        Debug.Log("Performing Attack 3");
        myAnimator.SetTrigger("Attack3");
        nextAttack3Time = attack3Cooldown;
        isAttacking = true;
        ResetRageState(); // Exit rage and reset the rage timer

        StartCoroutine(ManageCollidersDuringAttack(attack3AnimationDuration));
    }

    private void PerformAttack4()
    {
        Debug.Log("Performing Attack 4");
        myAnimator.SetTrigger("Attack4");
        nextAttack4Time = attack4Cooldown;
        isAttacking = true;
        ResetRageState(); // Exit rage and reset the rage timer

        StartCoroutine(ManageCollidersDuringAttack(attack4AnimationDuration));
    }

    private void ResetRageState()
    {
        // Reset the rage state and timer
        timeSinceLastAttack = 0f;
        ExitRageState();
    }

    private void HandleMovement(float distanceToPlayer)
    {
        // Handle normal movement towards the player
        if (distanceToPlayer <= 2.5f && !isAttacking)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0f, (Time.deltaTime / decelerationTime) * 3f);
            agent.speed = currentSpeed;
            myAnimator.SetFloat("Speed", currentSpeed);

            if (currentSpeed <= 0.01f)
            {
                agent.speed = 0f;
                myAnimator.SetBool("isWalking", false);
            }
            return;
        }

        float targetSpeed = distanceToPlayer > agent.stoppingDistance ? sprintSpeed : walkingSpeed;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime / (targetSpeed > currentSpeed ? accelerationTime : decelerationTime));

        agent.speed = currentSpeed;
        agent.SetDestination(playerTransform.position);
        myAnimator.SetFloat("Speed", currentSpeed);
        myAnimator.SetBool("isWalking", agent.remainingDistance > agent.stoppingDistance);
    }

    private IEnumerator ManageCollidersDuringAttack(float animationDuration)
    {
        // Enable hand colliders at the start of the attack
        EnableHandColliders();
        yield return new WaitForSeconds(0.1f);
        yield return new WaitForSeconds(animationDuration - 0.1f);

        // Disable hand colliders after attack is complete
        DisableHandColliders();
        ResetAttackParameters();
        isAttacking = false;
        CheckNextAttack();
    }

    private void ResetAttackParameters()
    {
        // Reset all attack triggers to avoid repeated attacks
        myAnimator.ResetTrigger("Attack1");
        myAnimator.ResetTrigger("Attack2");
        myAnimator.ResetTrigger("Attack3");
        myAnimator.ResetTrigger("Attack4");
    }

    private void CheckNextAttack()
    {
        // Immediately check if another attack can be performed
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        CheckForAttack(distanceToPlayer);
    }

    public void EnableHandColliders()
    {
        // Enable colliders on the boss's hands for attack hits
        rightHandCollider.enabled = true;
        leftHandCollider.enabled = true;
    }

    public void DisableHandColliders()
    {
        // Disable colliders on the boss's hands after attacks
        rightHandCollider.enabled = false;
        leftHandCollider.enabled = false;
    }

    public int GetAttackDamage()
    {
        // Determine damage based on the current attack animation
        if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
        {
            return attack1Damage;
        }
        else if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
        {
            return attack2Damage;
        }
        else if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack3"))
        {
            return attack3Damage;
        }
        else if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack4"))
        {
            return attack4Damage;
        }
        return 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Handle damage to the player when the boss collides with the player during an attack
        if ((other == rightHandCollider || other == leftHandCollider) && other.CompareTag("Player"))
        {
            IDamage playerDamage = other.GetComponent<IDamage>();
            if (playerDamage != null)
            {
                playerDamage.takeDamage(GetAttackDamage());
            }
        }
    }

    public void takeDamage(int amount)
    {
        if (isDead) return;

        currentHP -= amount;
        Debug.Log($"BossZombie takes {amount} damage!");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Handle the death of the boss
        isDead = true;
        myAnimator.SetBool("isDead", true);
        agent.enabled = false;
        rightHandCollider.enabled = false;
        leftHandCollider.enabled = false;
        Debug.Log("BossZombie has died.");
        StartCoroutine(DestroyAfterDeathAnimation());
    }

    private IEnumerator DestroyAfterDeathAnimation()
    {
        // Wait for the death animation to finish before destroying the object
        yield return new WaitForSeconds(myAnimator.GetCurrentAnimatorStateInfo(0).length);
        Destroy(gameObject);
    }
}
