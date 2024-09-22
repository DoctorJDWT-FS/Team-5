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


    [Header("----- Camera Items -----")]
    [SerializeField] int sensitivity = 600;
    [SerializeField] bool invertY = false;

    [Header("----- Audio -----")]
    [SerializeField] AudioClip backgroundMusic;

    [Header("----- KeyBindingsText -----")]
    [SerializeField] TMP_Text sprintKeyText;
    [SerializeField] TMP_Text slideKeyText;
    [SerializeField] TMP_Text dashKeyText;
    [SerializeField] TMP_Text jumpKeyText;
    [SerializeField] TMP_Text moveForwardKeyText;
    [SerializeField] TMP_Text moveBackwardKeyText;
    [SerializeField] TMP_Text strafeLeftKeyText;
    [SerializeField] TMP_Text strafeRightKeyText;
    [SerializeField] TMP_Text enterKeyText;
    [SerializeField] TMP_Text exitKeyText;
    [SerializeField] TMP_Text upKeyText;
    [SerializeField] TMP_Text downKeyText;
    [SerializeField] TMP_Text leftKeyText;
    [SerializeField] TMP_Text rightKeyText;
    [SerializeField] TMP_Text shootKeyText;
    [SerializeField] TMP_Text aimKeyText;
    [SerializeField] TMP_Text reloadKeyText;
    [SerializeField] TMP_Text interactKeyText;
    [SerializeField] TMP_Text punchKeyText;
    [SerializeField] TMP_Text grenadeKeyText;
    [SerializeField] TMP_Text displayPlayerStatsKeyText;


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

    public void UpdateKeybindings()
    {
        sprintKeyText.text = PlayerPrefs.GetString("sprint");
        slideKeyText.text = PlayerPrefs.GetString("slide");
        dashKeyText.text = PlayerPrefs.GetString("dash");
        jumpKeyText.text = PlayerPrefs.GetString("jump");
        moveForwardKeyText.text = PlayerPrefs.GetString("moveForward");
        moveBackwardKeyText.text = PlayerPrefs.GetString("moveBackwards");
        strafeLeftKeyText.text = PlayerPrefs.GetString("strafeLeft");
        strafeRightKeyText.text = PlayerPrefs.GetString("strafeRight");
        enterKeyText.text = PlayerPrefs.GetString("enter");
        exitKeyText.text = PlayerPrefs.GetString("exit");
        upKeyText.text = PlayerPrefs.GetString("up");
        downKeyText.text = PlayerPrefs.GetString("down");
        leftKeyText.text = PlayerPrefs.GetString("left");
        rightKeyText.text = PlayerPrefs.GetString("right");
        shootKeyText.text = PlayerPrefs.GetString("shoot");
        aimKeyText.text = PlayerPrefs.GetString("aim");
        reloadKeyText.text = PlayerPrefs.GetString("reload");
        interactKeyText.text = PlayerPrefs.GetString("interact");
        punchKeyText.text = PlayerPrefs.GetString("punch");
        grenadeKeyText.text = PlayerPrefs.GetString("grenade");
        displayPlayerStatsKeyText.text = PlayerPrefs.GetString("displayPlayerStats");
    }
}