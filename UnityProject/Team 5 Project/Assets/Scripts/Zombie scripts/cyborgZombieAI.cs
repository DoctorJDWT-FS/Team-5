using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cyborgZombieAI : basicZombieAI
{
    // shooting stats
    [Header("----- MellStats -----")]
    [SerializeField] Collider meleeCol;
    [Header("----- Shooting Stats -----")]
    [SerializeField] private float shootRate;
    [SerializeField] private CustomTrigger shootRangeTrigger;
        
    //shooting info
    [Header("----- Shooting Info -----")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform shootPos;
   


    private bool inShootingRange;
    private bool isShooting;
   
    float stoppingDis;
    protected override void Start()
    {
        //starts other trigger as well 
        base.Start();
        // added shooting trigger for zombie
        shootRangeTrigger.EnteredTrigger += OnShootingRangeTriggerEnter;
        shootRangeTrigger.ExitTrigger += OnShootingRangeTriggerExit;
        stoppingDis = agent.stoppingDistance;
    }

    protected override void Update()
    {
        //starts zombie basic update 
        base.Update();
       //if the zombie start to attack it will chase the human and attack them at full speed 
        if (playerInRange && isAttacking)
        {
            facePlayer();
            agent.stoppingDistance = 1;
        }
        if (playerInRange && !isAttacking && inShootingRange)
        {
            agent.stoppingDistance = stoppingDis;
            facePlayer();
            startShooting();

        }

        

        
    }

 
    void startShooting()
    {
        if (!isShooting)
        {
            StartCoroutine(Shoot());
        }
    }

    private IEnumerator Shoot()
    {
        //will keep shooting till player is in eiter attacking range or  leave shooting range
        isShooting = true;
        myAnimator.SetTrigger("Shoot");

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void createBullet()
    {
        Instantiate(bullet, shootPos.position, transform.rotation);
    }

    private void OnShootingRangeTriggerEnter(Collider other)
    {
        // if player is in range  sets it true 
        if (other.CompareTag("Player"))
        {
           inShootingRange = true;
        }
    }

    private void OnShootingRangeTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inShootingRange = false;
        }
    }
    protected override void OnAttackRangeTriggerEnter(Collider other)
    {
        base.OnAttackRangeTriggerEnter(other);

        if (other.CompareTag("Player"))
        {
            inShootingRange = false;
        }

    }

    protected override void OnAttackRangeTriggerExit(Collider other)
    {
        base.OnAttackRangeTriggerExit(other);
        if (other.CompareTag("Player"))
        {
            inShootingRange = true;
        }

    }

    public void swordMeleeColOn()
    {
        meleeCol.enabled = true;
    }
    public void swordMeleeColOff()
    {
        meleeCol.enabled = false;
    }


}
