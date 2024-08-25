using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zombieMelee : MonoBehaviour
{
    // Start is called before the first frame update
    //grabes the zombie parent 
    [SerializeField] basicZombieAI parent;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        IDamage dmg = other.GetComponent<IDamage>();

        if (other.CompareTag("Player") && dmg != null)
        {
            //grabs that zombie hit damage
            dmg.takeDamage(parent.getMeleeDmg());
        }
    }
}
