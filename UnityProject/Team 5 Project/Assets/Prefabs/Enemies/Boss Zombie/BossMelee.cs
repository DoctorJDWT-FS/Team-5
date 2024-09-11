using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMelee : MonoBehaviour
{
    // Reference to the boss parent script
    [SerializeField] private BossZombieAI parent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        IDamage dmg = other.GetComponent<IDamage>();

        if (other.CompareTag("Player") && dmg != null)
        {
            Debug.Log("Melee hit player"); // Check if this prints in the console
            dmg.takeDamage(parent.GetAttackDamage());
        }
    }

}
