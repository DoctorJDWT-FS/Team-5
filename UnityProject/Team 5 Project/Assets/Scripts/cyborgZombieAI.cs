using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cyborgZombieAI : basicZombieAI
{
    // Start is called before the first frame update
    [Header("----- Shooting Stats -----")]
    [SerializeField] private float shootRate;
    [SerializeField] private CustomTrigger shootRangeTrigger;

    [Header("----- Shooting Info -----")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform shootPos;
    [SerializeField] private int facePlayerSpeed;
    [SerializeField] private Transform headPos;

    private bool isShooting;
    private Vector3 playerDir;
    protected override void Start()
    {
        base.Start();
        shootRangeTrigger.EnteredTrigger += OnShootingRangeTriggerEnter;
        shootRangeTrigger.ExitTrigger += OnShootingRangeTriggerExit;
    }

    protected override void Update()
    {
        base.Update();

        if (isShooting)
        {
            playerDir = gameManager.instance.player.transform.position - headPos.position;
            facePlayer();
        }
    }

    private void facePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * facePlayerSpeed);
    }

    private IEnumerator Shoot()
    {
        while (isShooting)
        {
            Instantiate(bullet, shootPos.position, transform.rotation);
            yield return new WaitForSeconds(shootRate);
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

}
