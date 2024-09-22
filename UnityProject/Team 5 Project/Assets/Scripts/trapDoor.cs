using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trapDoor : MonoBehaviour
{
    [SerializeField] GameObject[] enableList;
    private Animator animator;
    private betterWaveSpawner waveSpawner;
    private GameObject waveManager;
    private bool activated;

    private void Awake()
    {
        waveManager = GameObject.Find("Wave Manager");
        waveSpawner = waveManager.GetComponent<betterWaveSpawner>();
    }
    private void Start()
    {
        waveManager.SetActive(false);
    }
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !activated)
        {
            activated = true;
            GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");
            for (int i = 0; i < zombies.Length; i++)
            {
                Destroy(zombies[i]);
                gameManager.instance.updateGameGoal(-1);
            }
            gameObject.GetComponent<Animator>().SetBool("Closed", true);
            for (int i = 0; i < enableList.Length; i++)
            {
                enableList[i].SetActive(true);
            }
            waveSpawner.isSpawning = false;
        }
    }
}
