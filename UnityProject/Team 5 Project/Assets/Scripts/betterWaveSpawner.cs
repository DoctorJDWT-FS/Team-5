using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class betterWaveSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] zombies;
    [Range(0,10)][SerializeField] int[] zombieWeights;
    [SerializeField] GameObject[] spawnPoints;
    [Range(1, 10)][SerializeField] int timeBetweenSpawns;
    [Range(1, 10)][SerializeField] int sizeOfSpawns;
    [Range(1, 10)][SerializeField] int startingSize;
    [Range(1, 10)][SerializeField] int spawnIncrease;

    private bool isSpawning;
    private int[] assignedWeights;
    private int totalWeight;
    private int numToSpawn;
    private int currentSpawned;
    private int waveNumber;

    void Start()
    {
        if (zombieWeights.Length > zombies.Length)
        {
            Debug.Log("There are more zombie weights than there are zombies, the extra weights will be ignored.");
        }
        else if (zombies.Length > zombieWeights.Length)
        {
            Debug.Log("There are more zombies than there are weights, please make the lists equal lengths.");
        }
        else
        {
            assignedWeights = new int[zombieWeights.Length];
            for (int i = 0; i < zombieWeights.Length; i++)
            {
                totalWeight += zombieWeights[i];
                assignedWeights[i] = totalWeight;
            }
        }
        if (spawnPoints.Length == 0)
        {
            Debug.Log("No spawnpoints set!");
        }
        numToSpawn = startingSize;
    }
    private void Update()
    {
        if (!isSpawning && gameManager.instance.enemyCount == 0)
        {
            currentSpawned = 0;
            StartCoroutine(spawn());
            if (waveNumber != 0)
            {
                numToSpawn += spawnIncrease;
            }
            waveNumber++;
        }
    }
    IEnumerator spawn()
    {
        isSpawning = true;
        int correctedSpawn = sizeOfSpawns;
        if ((numToSpawn - currentSpawned) < sizeOfSpawns) 
        {
            correctedSpawn = numToSpawn - currentSpawned;
        }
        for (int i = 0; i < correctedSpawn; i++)
        {
            int zombieRand = Random.Range(0, totalWeight);
            //Debug.Log("Random weight chosen: " + zombieRand);
            for (int j = 0; j < assignedWeights.Length; j++)
            {
                if (assignedWeights[j] >= zombieRand)
                {
                    int spawnRand = Random.Range(0, spawnPoints.Length - 1);
                    //Debug.Log("Spawning zombie #" + j + " at spawn point #" + spawnRand);
                    Instantiate(zombies[j], spawnPoints[spawnRand].transform.position, spawnPoints[spawnRand].transform.rotation);
                    currentSpawned++;
                    break;
                }
            }
        }
        yield return new WaitForSeconds(timeBetweenSpawns);
        if (currentSpawned < numToSpawn)
        {
            StartCoroutine(spawn());
        }
        else
        {
            isSpawning = false;
        }
    }
}
