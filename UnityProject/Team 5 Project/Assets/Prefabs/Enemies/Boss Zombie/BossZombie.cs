using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BossZombieAI : MonoBehaviour
{
    [Header("----- Boss Stats -----")]
    [SerializeField] private float walkingSpeed = 2f;      // Walking speed of the boss
    [SerializeField] private float sprintSpeed = 5f;       // Sprint speed of the boss
    [SerializeField] private float accelerationTime = 2f;  // Time it takes to accelerate to full speed
    [SerializeField] private float decelerationTime = 2f;  // Time it takes to decelerate to zero speed
    [SerializeField] private float attackRange = 1.5f;     // Range within which the boss can attack the player
    [SerializeField] private float attackCooldown = 2f;    // Cooldown between attacks (time in seconds)
    [SerializeField] private int attackDamage = 25;        // Damage dealt per attack

    [Header("----- Components -----")]
    [SerializeField] private NavMeshAgent agent;           // NavMeshAgent for movement
    [SerializeField] private Animator myAnimator;          // Animator component for handling animations

    [Header("----- Player Tracking -----")]
    private Transform playerTransform;                     // Reference to the player's transform
    private float currentSpeed;                            // Current smoothed speed
    private bool isAttacking = false;                      // Is the boss currently attacking
    private float nextAttackTime = 0f;                     // Time for the next allowed attack

    private void Start()
    {
        // Get the player transform from the game manager
        playerTransform = gameManager.instance.player.transform;

        // Initialize the animator component
        myAnimator = GetComponent<Animator>();

        // Set initial speed values
        currentSpeed = 0f;
        agent.speed = 0f;
    }

    private void Update()
    {
        // Always move toward the player
        agent.SetDestination(playerTransform.position);

        // Calculate the distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Check if within attack range
        if (distanceToPlayer <= attackRange)
        {
            // Handle attack logic
            HandleAttack();
        }
        else
        {
            // Handle movement logic
            HandleMovement(distanceToPlayer);
        }
    }

    private void HandleMovement(float distanceToPlayer)
    {
        // Reset attacking state if moving
        isAttacking = false;
        myAnimator.SetBool("isAttacking", false);

        // Adjust speed based on distance
        float targetSpeed = distanceToPlayer > 5f ? sprintSpeed : walkingSpeed;

        // Smoothly transition the speed
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime / (targetSpeed > currentSpeed ? accelerationTime : decelerationTime));

        // Apply the smoothed speed to the NavMeshAgent
        agent.speed = currentSpeed;

        // Update the Animator parameter to control the blend tree
        myAnimator.SetFloat("Speed", currentSpeed);
    }

    private void HandleAttack()
    {
        // Stop the movement when attacking
        agent.speed = 0f;
        myAnimator.SetFloat("Speed", 0f);

        // Check for cooldown and whether the boss can attack
        if (!isAttacking && Time.time >= nextAttackTime)
        {
            // Trigger attack animation
            myAnimator.SetTrigger("Attack1");

            // Perform the attack after a delay
            StartCoroutine(PerformAttack());

            // Set the next attack time based on the cooldown
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;

        // Wait for the attack animation to hit
        yield return new WaitForSeconds(0.5f);

        // Deal damage to the player
        playerController player = gameManager.instance.player.GetComponent<playerController>();
        if (player != null)
        {
            player.takeDamage(attackDamage);
        }

        // Reset attacking state
        isAttacking = false;
    }
}
