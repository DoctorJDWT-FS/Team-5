using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class countDown : MonoBehaviour
{
    [SerializeField] float startingTime;
    [SerializeField] TMP_Text textTimer;

    float currentTime;
    bool won;

    // Start is called before the first frame update
    void Start()
    {
        currentTime = startingTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (!won)
        {
            currentTime -= Time.deltaTime;

            textTimer.text = (float)(Math.Floor(currentTime * 10) / 10) + " ";

            if (currentTime <= 0)
            {
                won = true;
                gameManager.instance.youWin();
            }
        }
    }
}
