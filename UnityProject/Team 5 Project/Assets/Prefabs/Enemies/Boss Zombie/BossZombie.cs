using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BossZombieAI : MonoBehaviour, IDamage
{
    // Boss stats including movement speeds, acceleration/deceleration times, health points, and rage settings.
    [Header("--- Boss Stats ---")]
    [SerializeField] private float walkingSpeed = 4f;
    [SerializeField] private float sprintSpeed = 5.5f;
    [SerializeField] private float accelerationTime = 2f;
    [SerializeField] private float decelerationTime = 2f;
    [SerializeField] private int maxHP = 300;
    [SerializeField] private float rageDuration = 15f;
    [SerializeField] private float rageMultiplier = 2f;

    // Attack settings for different attacks, including range, cooldowns, damage, and animation durations.
    [Header("--- Attack Settings ---")]
    [Header("Attack 1")]
    [SerializeField] private float attack1Range = 5f;
    [SerializeField] private float attack1Cooldown = 8f;
    [SerializeField] private int attack1Damage = 50;
    [SerializeField] private float attack1AnimationDuration = 1.6f;

    [Header("Attack 2")]
    [SerializeField] private float attack2MinRange = 5f;
    [SerializeField] private float attack2MaxRange = 8f;
    [SerializeField] private float attack2Cooldown = 15f;
    [SerializeField] private int attack2Damage = 75;

    [Header("Attack 3")]
    [SerializeField] private float attack3MinRange = 0f;
    [SerializeField] private float attack3MaxRange = 5f;
    [SerializeField] private float attack3Cooldown = 8f;
    [SerializeField] private int attack3Damage = 60;
    [SerializeField] private float attack3AnimationDuration = 2.5f;

    [Header("Attack 4")]
    [SerializeField] private float attack4MinRange = 1.5f;
    [SerializeField] private float attack4MaxRange = 3f;
    [SerializeField] private float attack4Cooldown = 8f;
    [SerializeField] private int attack4Damage = 60;
    [SerializeField] private float attack4AnimationDuration = 2.55f;

    // References to key components, including the navigation agent, animator, and hand colliders.
    [Header("--- Components ---")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator myAnimator;
    [SerializeField] private Collider rightHandCollider;
    [SerializeField] private Collider leftHandCollider;

    // Variables for tracking player position, attack cooldowns, boss state.
    [Header("--- Player Tracking ---")]
    private Transform playerTransform;
    private float currentSpeed;
    private float nextAttack1Time = 0f;
    private float nextAttack2Time = 0f;
    private float nextAttack3Time = 0f;
    private float nextAttack4Time = 0f;
    private bool isDead = false;
    private int currentHP;
    private bool isAttacking = false;

    // Settings for the damage flash effect, including the model renderer and the color to flash when damaged.
    [Header("--- Damage Flash Settings ---")]
    [SerializeField] private Renderer model;
    [SerializeField] private Color colorDamage = Color.red;
    private Color originalColor;

    // Audio settings for various sound sources such as voice, punches, and footsteps, with volume control.
    [Header("--- Audio Settings ---")]
    [SerializeField] private AudioSource voiceAudioSource;
    [Range(0f, .1f)][SerializeField] private float voiceVolume = .08f;
    [SerializeField] private AudioSource LHAudioSource;
    [SerializeField] private AudioSource RHAudioSource;
    [Range(0f, .1f)][SerializeField] private float punchVolume = .055f;
    [SerializeField] private AudioSource LFAudioSource;
    [SerializeField] private AudioSource RFAudioSource;
    [Range(0f, .2f)][SerializeField] private float footstepVolume = .15f;

    // Holds audio clips for various boss sounds, including entrance, pain, rage, punch, footsteps, jump, and landing.
    [Header("--- Sound Clips ---")]
    [SerializeField] private AudioClip entranceSound;
    [SerializeField] private AudioClip[] painSounds;
    [SerializeField] private AudioClip rageYellSound;
    [SerializeField] private AudioClip bossPunchSound;
    [SerializeField] private AudioClip[] stepSounds;
    [SerializeField] private AudioClip bossJumpSound;
    [SerializeField] private AudioClip bossLandingSound;

    // Rage state variables
    private float timeSinceLastAttack = 0f;
    private bool isInRageState = false;

    // Pain state variables
    private bool hasPlayed75PercentSound = false;
    private bool hasPlayed50PercentSound = false;
    private bool hasPlayed25PercentSound = false;

    private void Start()
    {
        voiceAudioSource.volume = voiceVolume;
        LHAudioSource.volume = punchVolume;
        RHAudioSource.volume = punchVolume;
        LFAudioSource.volume = footstepVolume;
        RFAudioSource.volume = footstepVolume;

        PlayEntranceYell();
        playerTransform = gameManager.instance.player.transform;
        myAnimator = GetComponent<Animator>();
        currentSpeed = 0f;
        agent.speed = 0f;
        currentHP = maxHP;
        originalColor = model.material.color;
        
        DisableHandColliders();
    }

    // Handles regular boss behavior such as checking attack cooldowns, 
    // managing movement, rage state, and adjusting audio volumes. 
    // It stops further actions if the boss is dead or currently attacking, 
    // otherwise, it evaluates if an attack can be performed or handles movement.
    private void Update()
    {
        if (isDead) return;

        voiceAudioSource.volume = voiceVolume;
        LHAudioSource.volume = punchVolume;
        RHAudioSource.volume = punchVolume;
        LFAudioSource.volume = footstepVolume;
        RFAudioSource.volume = footstepVolume;

        if (nextAttack1Time > 0) nextAttack1Time -= Time.deltaTime;
        if (nextAttack2Time > 0) nextAttack2Time -= Time.deltaTime;
        if (nextAttack3Time > 0) nextAttack3Time -= Time.deltaTime;
        if (nextAttack4Time > 0) nextAttack4Time -= Time.deltaTime;

        timeSinceLastAttack += Time.deltaTime;
        CheckRageState();

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (isAttacking) return;

        if (CheckForAttack(distanceToPlayer)) return;

        if (isInRageState)
        {
            HandleRageMovement();
        }
        else
        {
            HandleMovement(distanceToPlayer);
        }
    }

    // Handles movement based on the player's distance and state.
    private void HandleMovement(float distanceToPlayer)
    {
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

    // Handles movement during the rage state by setting the destination to the player's position.
    private void HandleRageMovement()
    {
        agent.SetDestination(playerTransform.position);
        myAnimator.SetFloat("Speed", agent.speed);
    }

    // Plays the entrance yell sound if available.
    private void PlayEntranceYell()
    {
        if (entranceSound != null)
        {
            voiceAudioSource.clip = entranceSound;
            voiceAudioSource.Play();
        }
    }

    // Plays a random pain sound from the array if available.
    private void PlayPainSounds()
    {
        if (painSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, painSounds.Length);
            voiceAudioSource.clip = painSounds[randomIndex];
            voiceAudioSource.Play();
        }
    }

    // Checks if it's time to enter the rage state based on the time since the last attack.
    private void CheckRageState()
    {
        if (timeSinceLastAttack >= rageDuration && !isInRageState)
        {
            EnterRageState();
        }
    }

    // Enters the rage state by playing the rage yell sound and triggering the rage animation.
    private void EnterRageState()
    {
        isInRageState = true;
        myAnimator.SetTrigger("rage");
        myAnimator.SetBool("isRaging", true);
        agent.isStopped = true;
        StartCoroutine(HandleRageAnimation());
    }

    // Plays the rage yell sound if available.
    private void PlayRageYell()
    {
        if (rageYellSound != null)
        {
            voiceAudioSource.clip = rageYellSound;
            voiceAudioSource.Play();
        }
    }

    // Handles the rage animation by waiting for it to finish before increasing the boss's speed.
    private IEnumerator HandleRageAnimation()
    {
        while (!myAnimator.GetCurrentAnimatorStateInfo(0).IsName("rage"))
        {
            yield return null;
        }

        yield return new WaitForSeconds(myAnimator.GetCurrentAnimatorStateInfo(0).length);

        agent.isStopped = false;
        walkingSpeed *= rageMultiplier;
        sprintSpeed *= rageMultiplier;
        agent.speed = sprintSpeed;
    }

    // Plays the left footstep sound if available.
    public void LeftStep()
    {
        if (stepSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, stepSounds.Length);
            LFAudioSource.clip = stepSounds[randomIndex];
            LFAudioSource.Play();
        }
    }

    // Plays the right footstep sound if available.
    public void RightStep()
    {
        if (stepSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, stepSounds.Length);
            RFAudioSource.clip = stepSounds[randomIndex];
            RFAudioSource.Play();
        }
    }

    // Plays the boss jump sound if available.
    public void PlayBossJump()
    {
        if (bossJumpSound != null)
        {
            RFAudioSource.clip = bossJumpSound;
            RFAudioSource.Play();
        }
    }

    // Plays the boss landing sound if available.
    public void PlayBossLanding()
    {
        if (bossJumpSound != null)
        {
            RFAudioSource.clip = bossLandingSound;
            RFAudioSource.Play();
        }
    }

    // Exits the rage state by resetting the speed and disabling the rage animation.
    private void ExitRageState()
    {
        if (isInRageState)
        {
            isInRageState = false;
            myAnimator.SetBool("isRaging", false);
            walkingSpeed /= rageMultiplier;
            sprintSpeed /= rageMultiplier;
            agent.speed = walkingSpeed;
        }
    }

    // Checks for attack conditions based on the player's distance and triggers the appropriate attack if possible.
    private bool CheckForAttack(float distanceToPlayer)
    {
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
        else if (distanceToPlayer >= attack4MinRange && distanceToPlayer <= attack4MaxRange && nextAttack4Time <= 0)
        {
            PerformAttack4();
            return true;
        }

        return false;
    }

    // Plays the voice sound for the boss.
    public void PlayVoiceSound()
    {
        if (voiceAudioSource != null)
        {
            voiceAudioSource.Play();
        }
    }

    // Performs Attack 1 by stopping movement, triggering the animation, and setting the cooldown timer.
    private void PerformAttack1()
    {
        agent.isStopped = true;
        myAnimator.SetTrigger("Attack1");
        nextAttack1Time = attack1Cooldown;
        isAttacking = true;
        ResetRageState();
        StartCoroutine(HandleEndOfAttack(attack1AnimationDuration));
    }

    // Performs Attack 2 by stopping movement, triggering the animation, and setting the cooldown timer.
    private void PerformAttack2()
    {
        myAnimator.SetTrigger("Attack2");
        nextAttack2Time = attack2Cooldown;
        isAttacking = true;
        ResetRageState();
        isAttacking = false;
    }

    // Performs Attack 3 by stopping movement, triggering the animation, and setting the cooldown timer.
    private void PerformAttack3()
    {
        agent.isStopped = true;
        myAnimator.SetTrigger("Attack3");
        nextAttack3Time = attack3Cooldown;
        isAttacking = true;
        ResetRageState();
        StartCoroutine(HandleEndOfAttack(attack3AnimationDuration));
    }

    // Performs Attack 4 by stopping movement, triggering the animation, and setting the cooldown timer.
    private void PerformAttack4()
    {
        agent.isStopped = true;
        myAnimator.SetTrigger("Attack4");
        nextAttack4Time = attack4Cooldown;
        isAttacking = true;
        ResetRageState();
        StartCoroutine(HandleEndOfAttack(attack4AnimationDuration));
    }

    // Waits for the attack animation to finish before resuming movement and setting the attack state to false.
    private IEnumerator HandleEndOfAttack(float attackDuration)
    {
        yield return new WaitForSeconds(attackDuration);
        agent.isStopped = false;
        isAttacking = false;
    }

    // Resets the rage state by exiting the rage animation and resetting the rage timer.
    private void ResetRageState()
    {
        timeSinceLastAttack = 0f;
        ExitRageState();
    }

    // Resets the attack triggers to prevent multiple attacks from being triggered simultaneously.
    private void ResetAttackParameters()
    {
        myAnimator.ResetTrigger("Attack1");
        myAnimator.ResetTrigger("Attack2");
        myAnimator.ResetTrigger("Attack3");
        myAnimator.ResetTrigger("Attack4");
    }

    // Checks if another attack can be performed based on the player's distance and current attack status.
    private void CheckNextAttack()
    {
        // Immediately check if another attack can be performed
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        CheckForAttack(distanceToPlayer);
    }

    // Enables the left hand collider and plays the punch sound if available for the left-hand attack.
    public void LeftHandAttack()
    {
        leftHandCollider.enabled = true;

        if (bossPunchSound != null)
        {
            LHAudioSource.clip = bossPunchSound;
            LHAudioSource.Play();
        }
    }

    // Enables the right hand collider and plays the punch sound if available for the right-hand attack.
    public void RightHandAttack()
    {
        rightHandCollider.enabled = true;

        if (bossPunchSound != null)
        {
            RHAudioSource.clip = bossPunchSound;
            RHAudioSource.Play();
        }
    }

    // Disables both hand colliders after an attack is completed.
    public void DisableHandColliders()
    {
        rightHandCollider.enabled = false;
        leftHandCollider.enabled = false;
    }

    // Returns the appropriate attack damage based on the current attack animation being played.
    public int GetAttackDamage()
    {
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

    // Handles collision with the player during an attack. If the boss's hand colliders hit the player, 
    // it inflicts damage by calling the player's takeDamage method.
    private void OnTriggerEnter(Collider other)
    {
        if ((other == rightHandCollider || other == leftHandCollider) && other.CompareTag("Player"))
        {
            IDamage playerDamage = other.GetComponent<IDamage>();
            if (playerDamage != null)
            {
                playerDamage.takeDamage(GetAttackDamage());
            }
        }
    }

    // Handles taking damage by reducing health, triggering feedback, 
    // and checking for death when health reaches zero.
    public void takeDamage(int amount)
    {
        if (isDead) return;

        currentHP -= amount;
        CheckHealthThresholds();
        StartCoroutine(flashDamage());

        if (currentHP <= 0)
        {
            Die();
        }
    }

    // Checks health thresholds (75%, 50%, 25%) and plays pain sounds 
    // when those thresholds are crossed for the first time.
    private void CheckHealthThresholds()
    {
        float healthPercentage = (float)currentHP / maxHP;

        if (healthPercentage <= 0.75f && !hasPlayed75PercentSound)
        {
            PlayPainSounds();
            hasPlayed75PercentSound = true;
        }

        if (healthPercentage <= 0.50f && !hasPlayed50PercentSound)
        {
            PlayPainSounds();
            hasPlayed50PercentSound = true;
        }

        if (healthPercentage <= 0.25f && !hasPlayed25PercentSound)
        {
            PlayPainSounds();
            hasPlayed25PercentSound = true;
        }
    }

    // Temporarily changes the model's color to indicate damage and then resets it after a brief delay.
    private IEnumerator flashDamage()
    {
        model.material.color = colorDamage;
        yield return new WaitForSeconds(0.1f);
        model.material.color = originalColor;
    }

    // Handles the boss's death by disabling movement, colliders, and starting the death animation.
    private void Die()
    {
        isDead = true;
        myAnimator.SetBool("isDead", true);
        agent.enabled = false;
        rightHandCollider.enabled = false;
        leftHandCollider.enabled = false;
        StartCoroutine(DestroyAfterDeathAnimation());
    }

    // Waits for the death animation to finish before destroying the boss object.
    private IEnumerator DestroyAfterDeathAnimation()
    {
        yield return new WaitForSeconds(myAnimator.GetCurrentAnimatorStateInfo(0).length);
        Destroy(gameObject);
    }
}
