using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class buttonFunctions : MonoBehaviour
{
    public playerController player;
    public Powerups powerupScript;
    public Slider mainSlider;
    private iWallet playerWallet;
    private int x = 0, y = 0, z = 0;


    public void Start()
    {
        playerWallet = FindObjectOfType<iWallet>();
    }
    public void StartButton()
    {
        startManager.instance.startGame();
    }

    public void resume()
    {
        gameManager.instance.stateUnpause();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateUnpause();
    }
    public void respawn()
    {
        gameManager.instance.playerScript.spawnPlayer();
        gameManager.instance.stateUnpause();
    }
    public void back()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex == 0)
            startManager.instance.closeSettingsMenu();
    }
    public void settings()
    {
        Debug.Log("Settings");

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex == 0)
            startManager.instance.openSettingsMenu();
        else
            gameManager.instance.openSettingsMenu();
    }

    public void ChangeKeybindings()
    {
        Debug.Log("Settings");

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex == 0)
            startManager.instance.ChangeKeybindings();
    }
    public void onToggleChange()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex == 0)
            startManager.instance.toggleInvertY();
        else
            gameManager.instance.toggleInvertY();
    }
    public void OnSliderValueChanged()
    {
        if (mainSlider != null)
        {
            float sliderVal = mainSlider.value;
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            if (currentSceneIndex == 0)
                startManager.instance.UpdateSliderValue(sliderVal);
            else
                gameManager.instance.UpdateSliderValue(sliderVal);
        }
        else
        {
            Debug.LogWarning("mainSlider is not assigned.");
        }
    }
    public void healthUp()
    {
        playerWallet.SpendCredits(50);
        FindObjectOfType<tutorialManager>().Upgradebrought();
        gameManager.instance.playerScript.increaseMaxHealth(5);
        
    }
    public void shieldUp()
    {
        playerWallet.SpendCredits(50);
        gameManager.instance.playerScript.increaseMaxShield(5);
    }
    public void ammoUp()
    {
        playerWallet.SpendCredits(25);
        player.currentGun.magSize += 5;
    }
    public void fireRateUp()
    {
        playerWallet.SpendCredits(25);
        player.currentGun.fireRate += player.currentGun.fireRateOrig / 50;
    }
    public void buyRifle() 
    {
        if (z == 0)
        {
            playerWallet.SpendCredits(100);
            Debug.Log("Rifle Bought");
            z++;
            shopInteractable.instance.oneTimeButtons[0].SetActive(false);
            shopInteractable.instance.oneTimeButtons[3].SetActive(true);
        }
        else
        {
            Debug.Log("Already Bought");
            shopInteractable.instance.oneTimeButtons[0].SetActive(false);
            shopInteractable.instance.oneTimeButtons[3].SetActive(true);
        }
    }

    public void buyShotgun() 
    {
        if (x == 0)
        {
            playerWallet.SpendCredits(150);
            Debug.Log("Shotgun Bought");
            x++;
            shopInteractable.instance.oneTimeButtons[1].SetActive(false);
            shopInteractable.instance.oneTimeButtons[4].SetActive(true);
        }
        else
        {
            Debug.Log("Already Bought");
            shopInteractable.instance.oneTimeButtons[1].SetActive(false);
            shopInteractable.instance.oneTimeButtons[4].SetActive(true);
        }
    }

    public void buyLauncher()
    {
        if (y == 0)
        {
            playerWallet.SpendCredits(250);
            Debug.Log("Grenade Launcher Bought");
            y++;
            shopInteractable.instance.oneTimeButtons[2].SetActive(false);
            shopInteractable.instance.oneTimeButtons[5].SetActive(true);
        }
        else { 
            Debug.Log("Already Bought");
            shopInteractable.instance.oneTimeButtons[2].SetActive(false);
            shopInteractable.instance.oneTimeButtons[5].SetActive(true);
        }
    }
    public void buyGrenade(int kind)
    {
        if (kind == 1)
        {
            playerWallet.SpendCredits(100);
            Debug.Log("Fire Grenade Bought");
        }
        else if (kind == 2)
        {
            playerWallet.SpendCredits(100);
            Debug.Log("Ice Grenade Bought");
        }
        else
        {
            playerWallet.SpendCredits(100);
            
            Debug.Log("EMP Grenade Bought");
        }
    }
    public void SpawnDrone()
    {
        if (gameManager.instance.Drone == null)
        {
            if (playerWallet.SpendCredits(500))
            {
                Instantiate(gameManager.instance.GetDrone(), gameManager.instance.GetDroneSpawn().transform.position, transform.rotation);
            }
        }
    }
    public void heavyPowerup()
    {
        powerupScript.ActivateHeavyPowerup();
    }
    public void forceFieldPowerup()
    {

    }
    public void glassCannonPowerup()
    {

    }
    public void tankPowerup()
    {

    }
    public void exitShop()
    {
        shopInteractable.instance.shopInteraction(false);
    }

    public void shopMenuUpgrades()
    {
        gameManager.instance.shopMenu(1);
    }
    public void shopMenuWeapons()
    {
        gameManager.instance.shopMenu(2);
    }
    public void shopMenuPowerup()
    {
        gameManager.instance.shopMenu(3);
    }
    public void showCredits()
    {
        startManager.instance.openCredits();
    }
    public void backFromCredits()
    {
        startManager.instance.closeCredits();
    }
    public void quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
    }
}