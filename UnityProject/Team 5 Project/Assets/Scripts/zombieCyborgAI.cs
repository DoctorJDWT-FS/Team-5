using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class zombieCyborgAI : ZombieAI
{
    [Header("----- Shooting Stats -----")]
    [SerializeField] float shootRate;
    [SerializeField] CustomTrigger shootRangeTrigger;
   
    [Header("----- Shooting Info -----")]
    [SerializeField] GameObject bullet;
    [SerializeField] int viewAngle;
    [SerializeField] Transform shootPos;
    [SerializeField] int facePlayerSpeed;


    bool isShooting;
    Vector3 playerDir;
    float angleToPlayer;

    void Start()
    {
        //sets trigger
        shootRangeTrigger.EnteredTrigger += OnShootingRangeTriggerEnter;
        shootRangeTrigger.ExitTrigger += OnShootingRangeTriggerExit;

    }

    void facePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * facePlayerSpeed);
    }

     private void OnShootingRangeTriggerEnter(Collider other)
    {

        

    }
    private void OnShootingRangeTriggerExit(Collider other)
    {



    }
    IEnumerator shoot()
    {
        isShooting = true;
        Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(shootRate);
        isShooting = false;

    }

}
