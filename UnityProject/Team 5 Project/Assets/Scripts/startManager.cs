using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.VisualScripting;


public class startManager : MonoBehaviour
{
    public static startManager instance;


    [Header("----- Menu Items -----")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject startLabel;
    [SerializeField] GameObject Keybindings;
    [SerializeField] GameObject settingsLabel;
    [SerializeField] GameObject creditsLabel;
    [SerializeField] GameObject loadingScreen;
    [SerializeField] GameObject difficulties;


    [Header("----- Camera Items -----")]
    [SerializeField] int sensitivity = 600;
    [SerializeField] bool invertY = false;

    [Header("----- Audio -----")]
    [SerializeField] AudioClip backgroundMusic;



    int MenuPosition;
    private Image currentHighlight;

    void Awake()
    {
        instance = this;
        
    }

    private void Start()
    {
        AudioManager.instance.PlayMainMenuMusic();
    }


    public void openSettingsMenu()
    {
        startLabel.SetActive(false);
        settingsLabel.SetActive(true);
    }

    public void closeSettingsMenu()
    {
        startLabel.SetActive(true);
        settingsLabel.SetActive(false);
    }

    public void openCredits()
    {
        startLabel.SetActive(false);
        creditsLabel.SetActive(true);
    }
    public void closeCredits()
    {
        startLabel.SetActive(true);
        creditsLabel.SetActive(false);
    }

    public void toggleInvertY()
    {
        invertY = !invertY;
        PlayerPrefs.SetInt("InvertY", invertY ? 1 : 0);
        PlayerPrefs.Save();

    }
    public void UpdateSliderValue(float sens)
    {
        sensitivity = Mathf.RoundToInt(sens);
        PlayerPrefs.SetInt("Sensitivity", sensitivity);
        PlayerPrefs.Save();
    }
    public void loadSettings()
    {
        invertY = PlayerPrefs.GetInt("InvertY", 0) == 1;
        int sensitivity = PlayerPrefs.GetInt("Sensitivity", 600);
    }
    public void OpenChangeKeybindings()
    {
        settingsLabel.SetActive(false);
        Keybindings.SetActive(true);
    }

    public void CloseChangeKeyBindings()
    {
        Keybindings.SetActive(false);
        settingsLabel.SetActive(true);
    }
    public void OpenDifficulties()
    {
        difficulties.SetActive(true);
        startLabel.SetActive(false);
    }
    public void startGame()
    {
        StartCoroutine(load());
    }
   IEnumerator load()
    {
        loadingScreen.SetActive(true);
        startLabel.SetActive(false);
        yield return new WaitForSeconds(.5f);
        AudioManager.instance.StopMusic();
        SceneManager.LoadScene(1);
        AudioManager.instance.PlayGameplayMusic();
    }


}