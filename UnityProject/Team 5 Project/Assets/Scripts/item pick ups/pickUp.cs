using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickUp : MonoBehaviour
{
    [SerializeField] enum pickUpItem {Shield, Health, Ammo};
    [SerializeField] pickUpItem type;
    [SerializeField] private healthKitStats medkit = null;
    [SerializeField] private shieldStats shieldPack = null;
    [SerializeField] private ammoStats ammoPack = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            if (type == pickUpItem.Health)
            {
                gameManager.instance.playerScript.addHealth(medkit.healthGain);
                
            }
            if(type == pickUpItem.Shield)
            {
                gameManager.instance.playerScript.addShield(shieldPack.shieldGain);
            }
            if (type == pickUpItem.Ammo)
            {
                gameManager.instance.playerScript.addAmmo(ammoPack.ammoRecover, ammoPack.magRecover);
            }


            Destroy(gameObject);
        }
        
    }
}
