using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    int enemyCount;
    //player info
    public GameObject player;
    //public playerController playerScript;
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        // playerScript = player.GetComponent<playerController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updateGameGoal(int amount)
    {
        enemyCount += amount;
    }
}