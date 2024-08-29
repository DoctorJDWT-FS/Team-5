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

    //
    protected bool playerInRange;
    protected bool isAttacking;
    protected Color colorigin;
    protected Color coloriginHead;
    protected Animator myAnimator;

    protected Vector3 playerDir;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        //sets trigger
        attackRangeTrigger.EnteredTrigger += OnAttackRangeTriggerEnter;
        attackRangeTrigger.ExitTrigger += OnAttackRangeTriggerExit;
        visionRangeTrigger.EnteredTrigger += OnVisionRangeTriggerEnter;
        visionRangeTrigger.ExitTrigger += OnVisionRangeTriggerExit;

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
        agent.speed = playerInRange ? sprintSpeed : walkingSpeed;
        //get agent speed
        float agentSpeed = agent.velocity.magnitude;
        //lerp from one animation to the next
        myAnimator.SetFloat("Speed", Mathf.Lerp(myAnimator.GetFloat("Speed"), agentSpeed, Time.deltaTime * speedTrans));
        //zombie will always be chasing player at different speed 
        agent.SetDestination(gameManager.instance.player.transform.position);

        if (playerInRange && isAttacking)
        {

            facePlayer();
           
        }

    }

    protected virtual void facePlayer()
    {
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * facePlayerSpeed);

    }

    public virtual void takeDamage(int amount)
    {
        HP -= amount;
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


            gameManager.instance.spawnItemDrop(transform.position);


            Destroy(gameObject);
           
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
        if ( other.CompareTag("Player"))
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

    public virtual void resetTriggers()
    {
        myAnimator.SetBool("Attack", false);
        playerInRange = false;
    }


   
}

