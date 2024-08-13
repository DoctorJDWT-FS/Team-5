using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour, IDamage
{
    [Header("----- Enemy Stats -----")]
    [SerializeField] int HP;

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
    private Animator myAnimator;

    // Start is called before the first frame update
    void Start()
    {
        //sets trigger
        attackRangeTrigger.EnteredTrigger += OnAttackRangeTriggerEnter;
        attackRangeTrigger.ExitTrigger += OnAttackRangeTriggerExit;
        visionRangeTrigger.EnteredTrigger += OnVisionRangeTriggerEnter;
        visionRangeTrigger.ExitTrigger += OnVisionRangeTriggerExit;


        colorigin = model.material.color;
        coloriginHead = modelHead.material.color;
        //gameManager.instance.updateGameGoal(1);
        myAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {
            agent.SetDestination(gameManager.instance.player.transform.position);

        }
        if (!playerInRange)
        {
            myAnimator.SetInteger("Chase", 0);
        }
        

    }

    public void takeDamage(int amount)
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
            target.takeDamage(hitDamage);
            yield return new WaitForSeconds(HitRate);
        }

    }


    //Trigger functions
    private void OnAttackRangeTriggerEnter(Collider other)
    {

        target = other.GetComponent<IDamage>();
        if (target != null)
        {
            myAnimator.SetInteger("Attack", 1);
            isAttacking = true;
            StartCoroutine(Attack());

        }

    }
    private void OnAttackRangeTriggerExit(Collider other)
    {
        myAnimator.SetInteger("Attack", 0);
        target = null;
        isAttacking = false;
    }

    private void OnVisionRangeTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            myAnimator.SetInteger("Chase", 1);
            playerInRange = true;
        }
    }
    private void OnVisionRangeTriggerExit(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

}

