using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickUp : MonoBehaviour
{
    [SerializeField] enum pickUpItem {Sheild, Health, Ammo};
    [SerializeField] pickUpItem type;
    [SerializeField] private healthKitStats medkit;

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
            Destroy(gameObject);
        }
        
    }
}
