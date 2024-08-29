using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnPoint : MonoBehaviour
{
    public bool playerInRange;

    public bool Spawn(GameObject enemy)
    {
        if (!playerInRange)
        {
            // Adjust spawn position: setting y to 0, or adjust based on ground level.
            Vector3 spawnPosition = new Vector3(transform.position.x, 0, transform.position.z);
            Instantiate(enemy, spawnPosition, Quaternion.identity);
            return true;
        }
        else
        {
            return false;
        }
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
