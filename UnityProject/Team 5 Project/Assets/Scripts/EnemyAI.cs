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

    [Header("----- Damage Color -----")]
    [SerializeField] Color colorDamage;

   

    [Header("---- Custom Trigger ----")]
    [SerializeField] CustomTrigger attackRangeTrigger;
    [SerializeField] CustomTrigger visionRangeTrigger;

    //bool isShooting;
    bool playerInRange;
    bool isAttacking;
    Color colorigin;
    IDamage target;
    // Start is called before the first frame update
    void Start()
    {
        //sets trigger
        attackRangeTrigger.EnteredTrigger += OnAttackRangeTriggerEnter;
        attackRangeTrigger.ExitTrigger += OnAttackRangeTriggerExit;
        visionRangeTrigger.EnteredTrigger += OnVisionRangeTriggerEnter;
        visionRangeTrigger.ExitTrigger += OnVisionRangeTriggerExit;


        colorigin = model.material.color;
        gameManager.instance.updateGameGoal(1);
       
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {
            agent.SetDestination(gameManager.instance.player.transform.position);

           
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
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorigin;
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
            isAttacking = true;
            StartCoroutine(Attack());

        }

    }
    private void OnAttackRangeTriggerExit(Collider other)
    {
        target = null;
        isAttacking = false;
    }

    private void OnVisionRangeTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
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
