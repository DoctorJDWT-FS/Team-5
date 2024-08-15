using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;

public class waveSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] enemyTypes;
    public spawnPoint[] spawnPoints;

    [SerializeField] int startingEnemyAmount;
    [SerializeField] int enemyIncreasePerWave;
    [SerializeField] int timeBetweenWaves;

    int spawnAmount;
    bool spawning;

    void Start()
    {
        spawnAmount = startingEnemyAmount;
    }
    void Update()
    {
        if (!spawning)
        {
            spawning = true;
            StartCoroutine(spawnWave());

        }
    }
    IEnumerator spawnWave()
    {
        for (int i = 0; i < spawnAmount;)
        {
            int randomSpawner = Random.Range(0, spawnPoints.Length);
            //spawn random for enemy type 
            int randomEnemyType = Random.Range(0, 3);
            if (spawnPoints[randomSpawner].playerInRange == false)
            {
                //replaced enemy0 to random
                spawnPoints[randomSpawner].spawn(enemyTypes[randomEnemyType]);
                i++;
            }
        }
        yield return new WaitForSeconds(timeBetweenWaves);
        spawnAmount += enemyIncreasePerWave;
        spawning = false;
    }
}
