using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class countDown : MonoBehaviour
{
    [SerializeField] public float startingTime;
    [SerializeField] TMP_Text textTimer;
    private GameObject escapeShuttle;
    public float currentTime;
    bool won;

    // Start is called before the first frame update
    private void Awake()
    {
        escapeShuttle = GameObject.Find("Escape Shuttle");
        escapeShuttle.GetComponent<Animator>().speed = 0;
    }
    void Start()
    {
        currentTime = startingTime;
        escapeShuttle.GetComponent<Animator>().speed = (1 / currentTime);

    }

    // Update is called once per frame
    void Update()
    {
        if (!won)
        {
            gameManager.instance.currentObjective.text = "Survive";
            currentTime -= Time.deltaTime;

            textTimer.text = (float)(Math.Floor(currentTime * 10) / 10) + " ";

            if (currentTime <= 0.1)
            {
                won = true;
                gameManager.instance.youWin();
            }
        }
    }
}
