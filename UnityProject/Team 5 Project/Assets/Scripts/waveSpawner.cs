using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;

public class waveSpawner : MonoBehaviour
{
    public GameObject[] enemyTypes;
    public spawnPoint[] spawnPoints;

    [SerializeField] int startingEnemyAmount;
    [SerializeField] int enemyIncreasePerWave;
    [SerializeField] int timeBetweenWaves;
    [SerializeField] int maxEnemies;

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
            if (spawnPoints[randomSpawner].playerInRange == false && maxEnemies > gameManager.instance.enemyCount)
            {
                //replaced enemy0 to random
                spawnPoints[randomSpawner].spawn(enemyTypes[randomEnemyType]);
                i++;
            }
            else
            {
                yield return new WaitForSeconds(0);
            }
        }
        yield return new WaitForSeconds(timeBetweenWaves);
        spawnAmount += enemyIncreasePerWave;
        spawning = false;
    }
}
