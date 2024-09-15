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
    public AudioSource levelMusic;

    [Header("----- item Drop rate-----")]
    [SerializeField] GameObject[] Items;
    [Range(0, 100)][SerializeField] int dropRate;

    [Header("----- Text -----")]
    [SerializeField] TMP_Text enemyCountText;
    [SerializeField] TMP_Text creditCountText;

    //player info
    [Header("----- Player -----")]
    public playerController playerScript;
    public cameraController cameraScript;
    public iWallet playerWallet;

    [Header("----- Drone -----")]
    public helperBot Drone = null;
    public GameObject DroneModel ;
    public GameObject DroneSpawnPoint;

    [Header("----- other -----")]
    public GameObject player;
    public GameObject playerSpawnPos;
    public GameObject checkpointGet;
    public GameObject flashDamageScreen;
    public GameObject flashShieldDamageScreen;
    public Image PlayerHPBar;
    public Image playerShieldBar;
    public Image playerCreditCount;
    public TMP_Text currentObjective;

    public bool invertY;
    public bool isPaused;


    public int enemyCount;

    void Awake()
    {
        instance = this;
        levelMusic.Play();

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        playerWallet = player.GetComponent<iWallet>();
        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos");
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

        UpdateCreditDisplay();

    }

    public void UpdateCreditDisplay()
    {
        if (playerWallet != null && creditCountText != null)
        {
            creditCountText.text = playerWallet.Credits.ToString();
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
    public void openSettingsMenu()
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
            //statePause();
            //menuActive = menuWin;
            //menuActive.SetActive(isPaused);
        }
    }
    public void spawnItemDrop(Vector3 spawnPosition)
    {
        // Drop chance (dont change)
        int itemChanceDrop = Random.Range(0, 100);

        // Check if the item should be dropped based on the drop rate
        if (itemChanceDrop <= dropRate)
        {
            // Randomly select an item to drop
            int itemAcquired = Random.Range(0, Items.Length);

            Vector3 adjustedPosition = spawnPosition + new Vector3(0, 1, 0);
            // Instantiate the item at the provided spawn position
            Instantiate(Items[itemAcquired], adjustedPosition, Quaternion.identity);
        }
        else return;

    }
    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(isPaused);
    }
    public void youWin()
    {
        statePause();
        menuActive = menuWin;
        menuActive.SetActive(isPaused);
    }

    public void toggleInvertY()
    {
        invertY = !invertY; 
        if (cameraScript != null)
            cameraScript.invertY = invertY;
        PlayerPrefs.SetInt("InvertY", invertY ? 1 : 0);
        PlayerPrefs.Save();
    }
    public void UpdateSliderValue(float sens)
    {
        int sensitivity = Mathf.RoundToInt(sens);
        if (cameraScript != null)
            cameraScript.SetSensitivity(sensitivity);
        PlayerPrefs.SetInt("Sensitivity", sensitivity);
        PlayerPrefs.Save();
    }
    public void loadSettings()
    {
        invertY = PlayerPrefs.GetInt("InvertY", 0) == 1;
        if (cameraScript != null)
            cameraScript.invertY = invertY;


        int sensitivity = PlayerPrefs.GetInt("Sensitivity", 600);
        if (cameraScript != null)
            cameraScript.SetSensitivity(sensitivity);
    }


    public void SetDrone(helperBot bot)
    {
        Drone = bot;
    }
    public GameObject GetDrone()
    {
        return DroneModel;
    }
    public GameObject GetDroneSpawn()
    {
        return DroneSpawnPoint;
    }
}