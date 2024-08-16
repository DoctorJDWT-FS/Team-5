using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class basicZombieAI : MonoBehaviour, IDamage 
{
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

    //bool isShooting;
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


        colorigin = model.material.color;
        coloriginHead = modelHead.material.color;
        gameManager.instance.updateGameGoal(1);
        myAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        agent.SetDestination(gameManager.instance.player.transform.position);
        agent.speed = playerInRange ? sprintSpeed : walkingSpeed;

    }

    public virtual void takeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flashDamage());
        if (HP <= 0)
        {
            gameManager.instance.updateGameGoal(-1);
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
        while (isAttacking)
        {
            if (!gameManager.instance.playerScript.isDead)
            {
                target.takeDamage(hitDamage);
                yield return new WaitForSeconds(HitRate);
            }
            else
            {
                isAttacking = false;
            }

        }
    }


    //Trigger functions
    protected virtual void OnAttackRangeTriggerEnter(Collider other)
    {

        target = other.GetComponent<IDamage>();
        if (target != null && other.CompareTag("Player"))
        {
            myAnimator.SetBool("Attack", true);
            isAttacking = true;
            StartCoroutine(Attack());

        }

    }
    protected virtual void OnAttackRangeTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            myAnimator.SetBool("Attack", false);
            target = null;
            isAttacking = false;
        }
    }

    protected virtual void OnVisionRangeTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            myAnimator.SetBool("Chase", true);
            playerInRange = true;
        }
    }
    protected virtual void OnVisionRangeTriggerExit(Collider other)
    {

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

}

