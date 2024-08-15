using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour, IDamage
{
    [Header("----- Enemy Stats -----")]
    [SerializeField] int HP;
    [SerializeField] int sprintSpeed;
    [SerializeField] int walkingSpeed;


    [Header("----- Damage Stats -----")]
    [SerializeField] int hitDamage;
    [SerializeField] float HitRate;

    [Header("----- Model Items -----")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Renderer modelHead;

    [Header("----- Damage Color -----")]
    [SerializeField] Color colorDamage;



    [Header("---- Custom Trigger ----")]
    [SerializeField] CustomTrigger attackRangeTrigger;
    [SerializeField] CustomTrigger visionRangeTrigger;

    //bool isShooting;
     bool playerInRange;
     bool isAttacking;
     Color colorigin;
    Color coloriginHead;
    IDamage target;
    Animator myAnimator;

    // Start is called before the first frame update
    void  Start()
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
    void Update()
    {
        agent.SetDestination(gameManager.instance.player.transform.position);
        if (!playerInRange)
        {
            agent.speed = walkingSpeed;
        }
        if (playerInRange)
        {
            agent.speed = sprintSpeed;
        }

    }

    public  void takeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flashDamage());
        if (HP <= 0)
        {
            gameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }
    IEnumerator flashDamage()
    {
        model.material.color = colorDamage;
        modelHead.material.color = colorDamage;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorigin;
        modelHead.material.color = coloriginHead;
    }


    IEnumerator Attack()
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
    private void OnAttackRangeTriggerEnter(Collider other)
    {

        target = other.GetComponent<IDamage>();
        if (target != null && other.CompareTag("Player"))
        {
            myAnimator.SetBool("Attack", true);
            isAttacking = true;
            StartCoroutine(Attack());

        }

    }
    private void OnAttackRangeTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            myAnimator.SetBool("Attack", false);
            target = null;
            isAttacking = false;
        }
    }

    private void OnVisionRangeTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            myAnimator.SetBool("Chase", true);
            playerInRange = true;
        }
    }
    private void OnVisionRangeTriggerExit(Collider other)
    {
       
        if (other.CompareTag("Player"))
        {
            myAnimator.SetBool("Chase", false);
            playerInRange = false;
        }
    }

    public void resetTriggers()
    {
        myAnimator.SetBool("Attack", false);
        myAnimator.SetBool("Chase", false);
        target = null;
        isAttacking = false;
        playerInRange = false;
    }

}

