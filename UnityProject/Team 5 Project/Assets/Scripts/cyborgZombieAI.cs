using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cyborgZombieAI : basicZombieAI
{
    // shooting stats
    [Header("----- Shooting Stats -----")]
    [SerializeField] private float shootRate;
    [SerializeField] private CustomTrigger shootRangeTrigger;
        
    //shooting info
    [Header("----- Shooting Info -----")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform shootPos;
    [SerializeField] private int facePlayerSpeed;
    [SerializeField] private Transform headPos;

    private bool isShooting;
    private Vector3 playerDir;
    protected override void Start()
    {
        //starts other trigger as well 
        base.Start();
        // added shooting trigger for zombie
        shootRangeTrigger.EnteredTrigger += OnShootingRangeTriggerEnter;
        shootRangeTrigger.ExitTrigger += OnShootingRangeTriggerExit;
    }

    protected override void Update()
    {
        //starts zombie basic update 
        base.Update();
        //if zombie is shooting  will aim at player and face them 
        if (isShooting)
        {
            playerDir = gameManager.instance.player.transform.position - headPos.position;
            facePlayer();
        }
    }

    // faces player  function
    private void facePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * facePlayerSpeed);
    }

    private IEnumerator Shoot()
    {
        //will keep shooting till player is in eiter attacking range or  leave shooting range
        while (isShooting)
        {
            Instantiate(bullet, shootPos.position, transform.rotation);
            yield return new WaitForSeconds(shootRate);
        }
    }


    private void OnShootingRangeTriggerEnter(Collider other)
    {
        // if player is in range  sets it true 
        if (other.CompareTag("Player"))
        {
            myAnimator.SetBool("Shoot", true);
            isShooting = true;
            StartCoroutine(Shoot());
        }
    }

    private void OnShootingRangeTriggerExit(Collider other)
    {
        // if player is in range  sets it true 
        if (other.CompareTag("Player"))
        {
            myAnimator.SetBool("Shoot", false);
            isShooting = false;
        }
    }

    protected override void OnAttackRangeTriggerEnter(Collider other)
    {
        // add fucntion if zombie enter attack range stop shooting 
        if (target != null && other.CompareTag("Player"))
        {
            isShooting = false;
            myAnimator.SetBool("Shoot", false);

        }
        //uses  base class function 
        base.OnAttackRangeTriggerEnter(other);

       
    }
    protected override void OnAttackRangeTriggerExit(Collider other)
    {
        //stop attacking function 
        base.OnAttackRangeTriggerExit(other);
        // start shooting function
        if (other.CompareTag("Player"))
        {
            myAnimator.SetBool("Shoot", true);
            isShooting = true;
        }

    }
}
