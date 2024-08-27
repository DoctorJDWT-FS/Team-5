using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnPoint : MonoBehaviour
{
    public bool playerInRange;

    public bool spawn(GameObject enemy)
    {
        if (!playerInRange)
        {
            Instantiate(enemy, gameObject.transform);
            return true;
        }
        else return false;
    }
    private void OnTriggerEnter(Collider other)
    {
        playerInRange = true;
    }
    private void OnTriggerExit(Collider other)
    {
        playerInRange = false;
    }
}
