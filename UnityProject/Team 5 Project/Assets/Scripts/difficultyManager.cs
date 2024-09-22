using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;

public class difficultyManager : MonoBehaviour
{
    [SerializeField, Range(1,3)] public int difficulty;
    // 1 - Easy
    // 2 - Medium
    // 3 - Hard
    private GameObject waveManager;
    private countDown counter;
    private betterWaveSpawner waveSpawner;
    private GameObject countdown;

    // Start is called before the first frame update
    private void Awake()
    {
        difficulty = GameObject.Find("Difficulty Storage").GetComponent<difficultyStorage>().difficulty;
        Destroy(GameObject.Find("Difficulty Storage"));
        countdown = GameObject.Find("countdown");
        counter = countdown.GetComponent<countDown>();
        waveManager = GameObject.Find("Wave Manager");
        waveSpawner = waveManager.GetComponent<betterWaveSpawner>();
    }
    void Start()
    {
        countdown.SetActive(false);
        waveManager.SetActive(false);
        double difficultyMod = 1 + ((difficulty - 1.0) * 0.5);
        waveSpawner.timeBetweenWaves = (int)(waveSpawner.timeBetweenWaves / difficultyMod);
        waveSpawner.timeBetweenSpawns = (int)(waveSpawner.timeBetweenSpawns / difficultyMod);
        waveSpawner.sizeOfSpawns = (int)(waveSpawner.sizeOfSpawns * difficultyMod);
        waveSpawner.spawnIncrease = (int)(waveSpawner.spawnIncrease * difficultyMod);
        waveSpawner.startingSize = (int)(waveSpawner.startingSize * difficultyMod);

        counter.startingTime = (float)(counter.startingTime * difficultyMod);

        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        for (int i = 0; i < doors.Length; i++)
        {
            doors[i].GetComponent<TriggerDoorController>().doorCost = (int)(doors[i].GetComponent<TriggerDoorController>().doorCost * difficultyMod);
        }
    }
}
