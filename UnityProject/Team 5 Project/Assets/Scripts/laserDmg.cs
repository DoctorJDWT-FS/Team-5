using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laserDmg : MonoBehaviour
{
    [SerializeField] int laserDamage;
    private void OnTriggerEnter(Collider other)
    {
        IDamage dmg = other.GetComponent<IDamage>();
        if (other.CompareTag("Zombie"))
        {
            dmg.takeDamage(laserDamage);
        }
    }
}
