using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;


    [Header("----- Menu Items -----")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuSettings;

    public bool isPaused;

    [Header("----- Text -----")]
    [SerializeField] TMP_Text enemyCountText;
    
    //player info
    [Header("----- Player -----")]
    public GameObject player;
    public playerController playerScript;
    public cameraController cameraScript;
    public Image PlayerHPBar;
    public GameObject flashDamageScreen;

    int enemyCount;
    public bool invertY;

    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(isPaused);
            }
            else if (menuActive == menuPause)
                stateUnpause();
        }
    }
    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(isPaused);
        menuActive = null;
    }
    public void OpenSettingsMenu()
    {
        menuActive.SetActive(false);
        menuActive = menuSettings;
        menuSettings.SetActive(true);
    }
    public void updateGameGoal(int amount)
    {
        enemyCount += amount;
        enemyCountText.text = enemyCount.ToString("F0");
        if (enemyCount <= 0)
        {
            // temp stuff if we do wave based if not then its not temp
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(isPaused);
        }
    }
    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(isPaused);
    }

    public void toggleInvertY()
    {
        invertY = !invertY; 
        if (cameraScript != null)
            cameraScript.invertY = invertY;
    }
    public void UpdateSliderValue(float sens)
    {
        int sensitivity = Mathf.RoundToInt(sens);
        if (cameraScript != null)
            cameraScript.SetSensitivity(sensitivity);
    }
}