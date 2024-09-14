using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class helperBot : MonoBehaviour
{
    [Header("---- Model Items ----")]
    [SerializeField] NavMeshAgent agent;

    [Header("---- Custom Trigger ----")]
    [SerializeField] protected CustomTrigger AttackStayTrigger;
    [SerializeField] protected CustomTrigger PlayerRangeTrigger;
    [SerializeField] private int attackDistance;
    [SerializeField] private int playerDistance;

    [Header("----- Shooting Info -----")]
    [SerializeField] private int retargetZombieCD;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform shootPos;
    [SerializeField] private float shootRate;
    [SerializeField] private int faceZombieSpeed;

    basicZombieAI currentTarget = null;
    Vector3 target;

    bool playerinRange;
    bool isShooting;
    bool retarget;
    

    Vector3 zombieDir;
   
   void Start()
    {
        AttackStayTrigger.StayTrigger += OnAttackStayTriggerEnter;
        AttackStayTrigger.ExitTrigger += OnAttackStayTriggerExit;
        PlayerRangeTrigger.EnteredTrigger += OnPlayerRangeTriggerEnter;
        PlayerRangeTrigger.ExitTrigger += OnPlayerRangeTriggerExit;
        gameManager.instance.SetDrone(this);

    }
    private void Update()
    {
        agent.stoppingDistance = playerinRange ? attackDistance : playerDistance;
        if (!playerinRange)
        {
            agent.SetDestination(gameManager.instance.player.transform.position);
        }
        else if (playerinRange && currentTarget!= null) 
        {
            faceZombie();
            agent.SetDestination(target);
            if (!isShooting)
            {
                StartCoroutine(Shoot());
            }
        }


    }
    private void OnAttackStayTriggerEnter(Collider other)
    {
       
        if (other.CompareTag("Zombie") && currentTarget == null)
        {
           
            currentTarget = other.GetComponent<basicZombieAI>();
        }

        if(currentTarget != null)
        {
           
            if (currentTarget.isEliminated())
            {
                currentTarget = null;
                Debug.Log("Target died");
            }
            else
            {
                target = currentTarget.transform.position;
            }
            
        }
    }

    private void OnAttackStayTriggerExit(Collider other)
    {
        if (other.CompareTag("Zombie"))
        {
            if (other.CompareTag("Zombie") && currentTarget != null && other.GetComponent<basicZombieAI>() == currentTarget)
            {
                currentTarget = null;
                Debug.Log("Zombie left attack range.");
            }
        }

    }
    private void OnPlayerRangeTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerinRange = true;
        }
    }
    private void OnPlayerRangeTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerinRange = false;
        }
    }
    public void createBullet()
    {
        Instantiate(bullet, shootPos.position, transform.rotation);
    }

    protected virtual void faceZombie()
    {
        if (currentTarget != null)
        {
            zombieDir = currentTarget.transform.position - shootPos.position;
            Quaternion rot = Quaternion.LookRotation(zombieDir);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceZombieSpeed);
        }

    }
    private IEnumerator Shoot()
    {
        isShooting = true;
        createBullet();
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void SetTarget (basicZombieAI newtarget)
    {
        if (!retarget)
        { 
            currentTarget = newtarget;
            StartCoroutine(newTarget());
        }
    }

    private IEnumerator newTarget()
    {
        retarget = true;
        yield return new WaitForSeconds(retargetZombieCD);
        retarget = false;
    }
}
