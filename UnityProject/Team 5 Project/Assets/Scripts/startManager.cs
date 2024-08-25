using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class startManager : MonoBehaviour
{
    public static startManager instance;


    [Header("----- Menu Items -----")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject startLabel;
    [SerializeField] GameObject settingsLabel;

    [Header("----- Camera Items -----")]
    [SerializeField] int sensitivity = 600;
    [SerializeField] bool invertY = false;


    public bool isPaused;


    void Awake()
    {
        instance = this;
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

    public void openSettingsMenu()
    {
        startLabel.SetActive(false);
        settingsLabel.SetActive(true);

        isPaused = true;
        Time.timeScale = 0;
    }

    public void closeSettingsMenu()
    {
        startLabel.SetActive(true);
        settingsLabel.SetActive(false);

        isPaused = false;
        Time.timeScale = 1;
    }

    public void toggleInvertY()
    {
        invertY = !invertY;
        PlayerPrefs.SetInt("InvertY", invertY ? 1 : 0);
        
        Debug.Log(PlayerPrefs.GetInt("InvertY") + "hello");
        PlayerPrefs.Save();

    }
    public void UpdateSliderValue(float sens)
    {
        sensitivity = Mathf.RoundToInt(sens);
        PlayerPrefs.SetInt("Sensitivity", sensitivity);
        
        Debug.Log(PlayerPrefs.GetInt("Sensitivity") + "hello");
        PlayerPrefs.Save();
    }
    public void loadSettings()
    {
        invertY = PlayerPrefs.GetInt("InvertY", 0) == 1;
        int sensitivity = PlayerPrefs.GetInt("Sensitivity", 600);


    }

}