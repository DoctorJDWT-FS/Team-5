using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class zombieCyborgAI : MonoBehaviour, IDamage
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

    [Header("----- Shooting Stats -----")]
    [SerializeField] float shootRate;
    [SerializeField] CustomTrigger shootRangeTrigger;

    [Header("----- Shooting Info -----")]
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;
    [SerializeField] int facePlayerSpeed;
    [SerializeField] Transform headPos;


    bool isShooting;
    Vector3 playerDir;
    
   
   

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
        shootRangeTrigger.EnteredTrigger += OnShootingRangeTriggerEnter;
        shootRangeTrigger.ExitTrigger += OnShootingRangeTriggerExit;


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
        if (isShooting)
        {
            playerDir = gameManager.instance.player.transform.position - headPos.position;
            facePlayer();
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
        //will lopp will player is in attacking range 
        while (isAttacking)
        {
                target.takeDamage(hitDamage);
                yield return new WaitForSeconds(HitRate);
           
        }
    }

    IEnumerator Shoot()
    {
        //will loop till player is out of range 
        while (isShooting)
        {
            Instantiate(bullet, shootPos.position, transform.rotation);
            yield return new WaitForSeconds(shootRate);
        }
    }


    void facePlayer()
    {

        Quaternion rot = Quaternion.LookRotation(playerDir);

        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * facePlayerSpeed);

    }

    //Trigger functions
    void OnAttackRangeTriggerEnter(Collider other)
    {

        target = other.GetComponent<IDamage>();
        if (target != null && other.CompareTag("Player"))
        {
            isShooting = false;
            myAnimator.SetBool("Shoot", false);
            myAnimator.SetBool("Attack", true);
            isAttacking = true;
            StartCoroutine(Attack());
        }

    }
     void OnAttackRangeTriggerExit(Collider other)
     {
        if (other.CompareTag("Player"))
        {
            isShooting = true;
            myAnimator.SetBool("Shoot", true);
            myAnimator.SetBool("Attack", false);
            target = null;
            isAttacking = false;
        }
        
     }

     void OnVisionRangeTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            myAnimator.SetBool("Chase", true);
            playerInRange = true;
        }
    }
     void OnVisionRangeTriggerExit(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            myAnimator.SetBool("Chase", false);
            playerInRange = false;
        }
    }
    private void OnShootingRangeTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            myAnimator.SetBool("Shoot", true);
            
            isShooting = true;
            StartCoroutine(Shoot());

        }


    }
    private void OnShootingRangeTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            myAnimator.SetBool("Shoot", false);
            isShooting = false;
        }


    }

    public void resetTriggers()
    {
        myAnimator.SetBool("Attack", false);
        myAnimator.SetBool("Chase", false);
        myAnimator.SetBool("Shoot", false);
        target = null;
        isAttacking = false;
        playerInRange = false;
        isShooting = false;
        isAttacking = false;
    }
   

}
