using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HeadShot : MonoBehaviour, IDamage 
{
    [Header("----- Enemy Stats -----")]
    [SerializeField] basicZombieAI parent;

    
    public void takeDamage(int damage)
    {
        Debug.Log("Head-Shot!");
        parent.takeDamage(damage*2);
       
    }
    
}
