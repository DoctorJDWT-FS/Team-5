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
    [SerializeField] public GameObject menuActive;
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
    [SerializeField] TMP_Text playerHealthText;
    [SerializeField] TMP_Text playerSheildText;

    //player info
    [Header("----- Player -----")]
    public playerController playerScript;
    public cameraController cameraScript;
    public iWallet playerWallet;

    [Header("----- Reticle -----")]
    [SerializeField] private Image reticleImage; 
    [SerializeField] private Color defaultColor = Color.white;  
    [SerializeField] private Color zombieTargetColor = Color.red;  
    [SerializeField] private float rayDistance = 100f;

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

    [Header("----- Tutorial -----")]
    public buttonFunctions buttonTutorial;

    [Header("----- Grenades -----")]
    public int maxFireGrenades = 9;
    public int maxIceGrenades = 9;
    public int maxEMPGrenades = 9;

    public int curFireGrenades = 0;
    public int curIceGrenades = 0;
    public int curEMPGrenades = 0;

    [SerializeField] public Image fireGrenade, iceGrenade, empGrenade;


    public bool invertY;
    public bool isPaused;

    public int playerHp;
    public int playerShield;
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

        HandleReticleRaycast();

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

    private void HandleReticleRaycast()
    {
        if (playerScript.isDead)
            return;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.collider.CompareTag("Zombie"))
            {
                reticleImage.color = zombieTargetColor;
            }
            else
            {
                reticleImage.color = defaultColor;
            }
        }
        else
        {
            reticleImage.color = defaultColor;
        }
    }

    public void UpdateCreditDisplay()
    {
        if (playerWallet != null && creditCountText != null)
        {
            creditCountText.text = playerWallet.Credits.ToString();
        }
    }

    public void UpdateHealthCount()
    { 
        if (gameManager.instance.playerScript.HP >= 0)
        {
            playerHealthText.text = gameManager.instance.playerScript.HP.ToString();
        }
    }

    public void UpdateSheildCount()
    {
        playerSheildText.text = gameManager.instance.playerScript.shield.ToString();
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
        menuActive.SetActive(false);
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
        menuActive.SetActive(true);
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
    public void shopMenu(int menu)
    {
        if (menu == 1)
        {
            shopInteractable.instance.upgradeItems.SetActive(true);
            shopInteractable.instance.weaponItems.SetActive(false);
            shopInteractable.instance.powerupItems.SetActive(false);
        }
        else if (menu == 2)
        {
            shopInteractable.instance.upgradeItems.SetActive(false);
            shopInteractable.instance.weaponItems.SetActive(true);
            shopInteractable.instance.powerupItems.SetActive(false);
        }
        else
        {
            shopInteractable.instance.upgradeItems.SetActive(false);
            shopInteractable.instance.weaponItems.SetActive(false);
            shopInteractable.instance.powerupItems.SetActive(true);
        }
    }

    public void UpdateGrenadeImages(int currentGrenadeIndex)
    {
        fireGrenade.gameObject.SetActive(currentGrenadeIndex == 0);
        iceGrenade.gameObject.SetActive(currentGrenadeIndex == 1);
        empGrenade.gameObject.SetActive(currentGrenadeIndex == 2);
    }

    public void UpdateGrenadeCountDisplay()
    {
        fireGrenade.transform.GetChild(0).GetComponent<TMP_Text>().text = curFireGrenades.ToString();
        iceGrenade.transform.GetChild(0).GetComponent<TMP_Text>().text = curIceGrenades.ToString();
        empGrenade.transform.GetChild(0).GetComponent<TMP_Text>().text = curEMPGrenades.ToString();
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
    public void setDroneSpawn(GameObject DroneSpawn)
    {
        DroneSpawnPoint = DroneSpawn;
    }
}