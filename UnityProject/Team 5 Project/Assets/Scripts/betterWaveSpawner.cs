using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class betterWaveSpawner : MonoBehaviour
{
    [SerializeField] public GameObject[] zombies;
    [Range(0,10)][SerializeField] int[] zombieWeights;
    [Range(1, 10)][SerializeField] public int timeBetweenSpawns;
    [Range(1, 10)][SerializeField] public int timeBetweenWaves;
    [Range(1, 10)][SerializeField] public int sizeOfSpawns;
    [Range(1, 10)][SerializeField] public int startingSize;
    [Range(1, 10)][SerializeField] public int spawnIncrease;

    [SerializeField] TMP_Text waveCountText;
    [SerializeField] TMP_Text waveCountdownText;
    [SerializeField] GameObject waveCountdown;

    private GameObject[] spawnPoints;
    private bool isSpawning;
    private bool isCounting;
    private float currentTime;
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
        numToSpawn = startingSize;
    }
    private void Update()
    {
        if (!isSpawning && gameManager.instance.enemyCount == 0)
        {
            currentSpawned = 0;
            StartCoroutine(waveCountDown());
            if (waveNumber != 0)
            {
                numToSpawn += spawnIncrease;
            }
            waveNumber++;
            waveCountText.text = waveNumber.ToString();
        }
        if (isCounting)
        {
            currentTime -= Time.deltaTime;

            waveCountdownText.text = (float)(System.Math.Floor(currentTime * 10) / 10) + " ";
            if (currentTime <= 0.1)
            {
                isCounting = false;
            }
        }
    }
    IEnumerator spawn()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("Zombie spawn point");
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
                    int spawnRand = Random.Range(0, spawnPoints.Length);
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
    IEnumerator waveCountDown()
    {
        isSpawning = true;
        currentTime = timeBetweenWaves;
        isCounting = true;
        waveCountdown.SetActive(true);
        yield return new WaitForSeconds(timeBetweenWaves);
        waveCountdown.SetActive(false);
        StartCoroutine(spawn());
    }
}
