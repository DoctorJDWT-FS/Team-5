using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class difficultyManager : MonoBehaviour
{
    [SerializeField, Range(1,3)] public int difficulty;
    // 1 - Easy
    // 2 - Medium
    // 3 - Hard
    private GameObject waveManager;
    private betterWaveSpawner waveSpawner;

    // Start is called before the first frame update
    void Start()
    {
        double difficultyMod = 1 + ((difficulty - 1.0) * 0.5);
        waveManager = GameObject.Find("Wave Manager");
        waveSpawner = waveManager.GetComponent<betterWaveSpawner>();
        waveSpawner.timeBetweenWaves = (int)(waveSpawner.timeBetweenWaves / difficultyMod);
        waveSpawner.timeBetweenSpawns = (int)(waveSpawner.timeBetweenSpawns / difficultyMod);
        waveSpawner.sizeOfSpawns = (int)(waveSpawner.sizeOfSpawns * difficultyMod);
        waveSpawner.spawnIncrease = (int)(waveSpawner.spawnIncrease * difficultyMod);
        waveSpawner.startingSize = (int)(waveSpawner.startingSize * difficultyMod);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
