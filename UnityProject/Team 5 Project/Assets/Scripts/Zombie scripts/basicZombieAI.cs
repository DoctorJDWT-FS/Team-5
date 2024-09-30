using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class basicZombieAI : MonoBehaviour, IDamage 
{
    [Header("----- Enemy Stats -----")]
    [SerializeField] public int valuePoints;
    [Header("----- Enemy Stats -----")]
    [SerializeField] protected int HP;
    [SerializeField] protected float sprintSpeed;
    [SerializeField] protected float walkingSpeed;
    [SerializeField] protected float speedTrans;


    [Header("----- Damage Stats -----")]
    [SerializeField] protected int hitDamage;
    [SerializeField] protected float HitRate;
    [SerializeField] private Collider leftCol;
    [SerializeField] private Collider rightCol;

    [Header("----- Model Items -----")]
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Renderer model;
    [SerializeField] protected Renderer modelHead;
    [SerializeField] protected Transform headPos;
    [SerializeField] protected int facePlayerSpeed;

    [Header("----- Damage Color -----")]
    [SerializeField] protected Color colorDamage;

    [Header("---- Custom Trigger ----")]
    [SerializeField] protected CustomTrigger attackRangeTrigger;
    [SerializeField] protected CustomTrigger visionRangeTrigger;

    [Header("---- Audio ----")]
    [SerializeField] protected AudioSource audPlayer;
    [SerializeField] protected AudioClip walkingSounds;
    [SerializeField] protected AudioClip runningSounds;
    [SerializeField] protected AudioClip attackingSounds;
    [SerializeField] protected AudioClip damagedSounds;
    

    //
    protected bool playerInRange;
    protected bool isAttacking;
    protected Color colorigin;
    protected Color coloriginHead;
    protected Animator myAnimator;
    protected bool isStunned = false;
    protected float originalSpeedWalking;
    protected float originalSpeedSprinting;

    protected Vector3 playerDir;

    //helper bot code 
    bool isDead = false;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        //sets trigger
        attackRangeTrigger.EnteredTrigger += OnAttackRangeTriggerEnter;
        attackRangeTrigger.ExitTrigger += OnAttackRangeTriggerExit;
        visionRangeTrigger.EnteredTrigger += OnVisionRangeTriggerEnter;
        visionRangeTrigger.ExitTrigger += OnVisionRangeTriggerExit;

        originalSpeedSprinting = sprintSpeed;
        originalSpeedWalking = walkingSpeed;

        //sets  skin mesh for damage 
        colorigin = model.material.color;
        coloriginHead = modelHead.material.color;
        //game goal update
        gameManager.instance.updateGameGoal(1);
        //animator on charactor extractor 
        myAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        //if player is close, they will run else walk 
        if (!isDead && !isStunned)  // Check if not stunned before allowing movement
        {
            agent.speed = playerInRange ? sprintSpeed : walkingSpeed;
            float agentSpeed = agent.velocity.magnitude;
            myAnimator.SetFloat("Speed", Mathf.Lerp(myAnimator.GetFloat("Speed"), agentSpeed, Time.deltaTime * speedTrans));
            agent.SetDestination(gameManager.instance.player.transform.position);

            if (playerInRange && isAttacking)
            {
                facePlayer();
            }
        }

    }

    protected virtual void facePlayer()
    {
        audPlayer.clip = walkingSounds;
        audPlayer.loop = true;
        audPlayer.Play();
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * facePlayerSpeed);
        audPlayer.loop = false;
        audPlayer.Stop();
    }

    public virtual void takeDamage(int amount)
    {
        if (isDead)
        {
            return;
        }
        HP -= amount;
        audPlayer.clip = damagedSounds;
        audPlayer.Play();
        //StartCoroutine(StunnedState());
        StartCoroutine(flashDamage());

        if (HP <= 0)
        {
            gameManager.instance.updateGameGoal(-1);

            // Award credits to the player before destroying the object
            iCredit creditComponent = GetComponent<iCredit>();
            if (creditComponent != null)
            {
                creditComponent.AwardCredits();
            }

            agent.isStopped = true; 
            agent.enabled = false;
            isDead = true;
            myAnimator.SetTrigger("Death");
            
           
        }
    }

    protected IEnumerator flashDamage()
    {
        model.material.color = colorDamage;
        modelHead.material.color = colorDamage;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorigin;
        modelHead.material.color = coloriginHead;
    }

    protected IEnumerator StunnedState()
    {

        float tempSprintSpeed = sprintSpeed;
        float tempWalkingSpeed = walkingSpeed;
        float tempSpeedTrans = speedTrans;
        sprintSpeed = 0;
        walkingSpeed = 0;
        speedTrans = 0;
        yield return new WaitForSeconds(0.1f);
        sprintSpeed = tempSprintSpeed;
        walkingSpeed = tempWalkingSpeed;
        speedTrans = tempSpeedTrans;
    }

    public  virtual int getMeleeDmg()
    {
        return hitDamage;
    }

    public void lMeleeColOn()
    {
        leftCol.enabled = true;
    }
    public void lMeleeColOff()
    {
        leftCol.enabled= false;
    }
    public void rMeleeColOn()
    {
        rightCol.enabled = true;
    }
    public void rMeleeColOff()
    {
        rightCol.enabled= false;
    }

    //Trigger functions
    protected virtual void OnAttackRangeTriggerEnter(Collider other)
    {
        if ( other.CompareTag("Player")&& !isDead)
        {
            //set animation to attacking true 
            myAnimator.SetBool("Attack", true);
            isAttacking = true;
        }
    }
    protected virtual void OnAttackRangeTriggerExit(Collider other)
    {
        // check if the other is a player that has left the range 
        if (other.CompareTag("Player"))
        {
            //stop attacking animation 
            myAnimator.SetBool("Attack", false);
            isAttacking= false;
        }
    }

    protected virtual void OnVisionRangeTriggerEnter(Collider other)
    {
        //if other is player they will run and start the animation 
        if (other.CompareTag("Player"))
        {
            audPlayer.clip = runningSounds;
            audPlayer.Play();
            playerInRange = true;
        }
    }
    protected virtual void OnVisionRangeTriggerExit(Collider other)
    {
        //if other is player they will start walking animation
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    // Grenade effects
    public void applySlow(float slowAmount, float duration)
    {
        walkingSpeed /= slowAmount;
        sprintSpeed /= slowAmount;
        StartCoroutine(RemoveSlowAfterDuration(duration));
    }
    public void applyStun(float duration)
    {
        isStunned = true;
        StartCoroutine(RemoveStunAfterDuration(duration));
    }
    private IEnumerator RemoveSlowAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        walkingSpeed = originalSpeedWalking;
        sprintSpeed = originalSpeedSprinting;
    }

    private IEnumerator RemoveStunAfterDuration(float duration)
    {
        walkingSpeed = 0;
        sprintSpeed = 0;
        yield return new WaitForSeconds(duration);
        isStunned = false;
        walkingSpeed = originalSpeedWalking;
        sprintSpeed = originalSpeedSprinting;
    }

    public virtual void resetTriggers()
    {
        myAnimator.SetBool("Attack", false);
        playerInRange = false;
    }
    
    public virtual bool isEliminated()
    {
        return isDead;
    }

    public virtual void PlayDeath ()
    {

        gameManager.instance.spawnItemDrop(transform.position);
        Destroy(gameObject);
        
    }

}

