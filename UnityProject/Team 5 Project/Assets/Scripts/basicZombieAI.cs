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
    [SerializeField] protected int sprintSpeed;
    [SerializeField] protected int walkingSpeed;


    [Header("----- Damage Stats -----")]
    [SerializeField] protected int hitDamage;
    [SerializeField] protected float HitRate;

    [Header("----- Model Items -----")]
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Renderer model;
    [SerializeField] protected Renderer modelHead;

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
    protected IDamage target;
    protected Animator myAnimator;

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
        //zombie will always be chasing player at different speed 
        agent.SetDestination(gameManager.instance.player.transform.position);
        //if player is close, they will run else walk 
        agent.speed = playerInRange ? sprintSpeed : walkingSpeed;

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


    protected IEnumerator Attack()
    {
        // zombie will attack till player is dead or steps out of range 
        while (isAttacking)
        {
            //if player isnt dead it will attack only the player  no zombies should be hit 
            if (!gameManager.instance.playerScript.isDead)
            {
                target.takeDamage(hitDamage);
                yield return new WaitForSeconds(HitRate);
            }
            else
            {
                //player must have died and will set attacking to false to break loop
                isAttacking = false;
            }

        }
    }


    //Trigger functions
    protected virtual void OnAttackRangeTriggerEnter(Collider other)
    {
        // grabs other compponent
        target = other.GetComponent<IDamage>();
        //makes sure it is the player
        if (target != null && other.CompareTag("Player"))
        {
            //set animation to attacking true 
            myAnimator.SetBool("Attack", true);
            //starts attack
            isAttacking = true;
            StartCoroutine(Attack());

        }

    }
    protected virtual void OnAttackRangeTriggerExit(Collider other)
    {
        // check if the other is a player that has left the range 
        if (other.CompareTag("Player"))
        {
            //stop attacking animation 
            myAnimator.SetBool("Attack", false);
            target = null;
            isAttacking = false;
        }
    }

    protected virtual void OnVisionRangeTriggerEnter(Collider other)
    {
        //if other is player they will run and start the animation 
        if (other.CompareTag("Player"))
        {
            myAnimator.SetBool("Chase", true);
            playerInRange = true;
        }
    }
    protected virtual void OnVisionRangeTriggerExit(Collider other)
    {
        //if other is player they will start walking animation
        if (other.CompareTag("Player"))
        {
            myAnimator.SetBool("Chase", false);
            playerInRange = false;
        }
    }

    public virtual void resetTriggers()
    {
        myAnimator.SetBool("Attack", false);
        myAnimator.SetBool("Chase", false);
        target = null;
        isAttacking = false;
        playerInRange = false;
    }

    //Not used anymore in this way
    //protected virtual void AddPoints()
    //{
    //    currencyManager.instance.AddCurrency(valuePoints);
    //}   
}

