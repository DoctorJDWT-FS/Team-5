using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BossZombieAI : MonoBehaviour, IDamage
{
    [Header("----- Boss Stats -----")]
    [SerializeField] private float walkingSpeed = 2f;
    [SerializeField] private float sprintSpeed = 5f;
    [SerializeField] private float accelerationTime = 2f;
    [SerializeField] private float decelerationTime = 2f;
    [SerializeField] private int maxHP = 500;

    [Header("----- Attack 1 Settings -----")]
    [SerializeField] private float attack1Range = 1.5f;           // Range for Attack1
    [SerializeField] private float attack1Cooldown = 2f;          // Cooldown for Attack1
    [SerializeField] private int attack1Damage = 25;              // Damage value for Attack1

    [Header("----- Components -----")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator myAnimator;
    [SerializeField] private Collider rightHandCollider;          // Reference to the right-hand collider
    [SerializeField] private Collider leftHandCollider;           // Reference to the left-hand collider

    [Header("----- Player Tracking -----")]
    private Transform playerTransform;
    private float currentSpeed;
    private float nextAttack1Time = 0f;                           // Tracks when the next Attack1 can occur
    private bool isDead = false;                                  // Tracks if the boss is dead
    private int currentHP;

    private void Start()
    {
        playerTransform = gameManager.instance.player.transform;
        myAnimator = GetComponent<Animator>();
        currentSpeed = 0f;
        agent.speed = 0f;
        currentHP = maxHP;

        // Disable hand colliders initially
        rightHandCollider.enabled = false;
        leftHandCollider.enabled = false;
    }

    private void Update()
    {
        if (isDead) return;  // Stop all actions if dead

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= attack1Range)
        {
            // Player is in attack range, try attacking
            if (Time.time >= nextAttack1Time)
            {
                Attack1();
            }
            else
            {
                // While on cooldown, keep boss in idle state
                myAnimator.SetFloat("Speed", 0f);
                agent.speed = 0f;
            }
        }
        else
        {
            // Player is out of attack range, handle movement
            HandleMovement(distanceToPlayer);
        }
    }

    private void Attack1()
    {
        // Trigger attack animation
        myAnimator.SetTrigger("Attack1");

        // Set the next attack time after the cooldown
        nextAttack1Time = Time.time + attack1Cooldown;

        // Enable hand colliders to apply damage
        rightHandCollider.enabled = true;
        leftHandCollider.enabled = true;
    }

    private void HandleMovement(float distanceToPlayer)
    {
        // Adjust speed based on distance
        float targetSpeed = distanceToPlayer > 5f ? sprintSpeed : walkingSpeed;

        // Smooth speed transition
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime / (targetSpeed > currentSpeed ? accelerationTime : decelerationTime));

        agent.speed = currentSpeed;
        agent.SetDestination(playerTransform.position); // Move towards the player
        myAnimator.SetFloat("Speed", currentSpeed);
        myAnimator.SetBool("isWalking", true); // Set isWalking to true when moving
    }

    // Animation event method to enable hand colliders
    public void EnableHandColliders()
    {
        rightHandCollider.enabled = true;
        leftHandCollider.enabled = true;
    }

    // Animation event method to disable hand colliders
    public void DisableHandColliders()
    {
        rightHandCollider.enabled = false;
        leftHandCollider.enabled = false;
    }

    // Method to get attack damage
    public int GetAttackDamage()
    {
        return attack1Damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the trigger is from the hand colliders and the player is hit
        if ((other == rightHandCollider || other == leftHandCollider) && other.CompareTag("Player"))
        {
            IDamage playerDamage = other.GetComponent<IDamage>();
            if (playerDamage != null)
            {
                playerDamage.takeDamage(GetAttackDamage()); // Use the method to get attack damage
            }
        }
    }

    // Implementing the takeDamage method from IDamage interface
    public void takeDamage(int amount)
    {
        if (isDead) return; // Don't take damage if already dead

        currentHP -= amount;
        Debug.Log($"BossZombie takes {amount} damage!");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;  // Set the dead flag
        myAnimator.SetBool("isDead", true);  // Set the 'isDead' boolean in the animator to true

        // Disable components that should not be active after death
        agent.enabled = false;  // Disable the NavMeshAgent to stop movement
        rightHandCollider.enabled = false;
        leftHandCollider.enabled = false;

        Debug.Log("BossZombie has died.");

        // Start a coroutine to destroy the object after the animation ends
        StartCoroutine(DestroyAfterDeathAnimation());
    }

    private IEnumerator DestroyAfterDeathAnimation()
    {
        // Wait for the duration of the death animation
        yield return new WaitForSeconds(myAnimator.GetCurrentAnimatorStateInfo(0).length);

        // Destroy the GameObject
        Destroy(gameObject);
    }
}
