using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class buttonFunctions : MonoBehaviour
{
    public playerController player;
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
            Debug.Log("Rifle Bought");
            z++;
        }
        else
            Debug.Log("Already Bought");
    }

    public void buyShotgun() 
    {
        if (x == 0)
        {
            Debug.Log("Shotgun Bought");
            x++;
        }
        else
            Debug.Log("Already Bought");
    }

    public void buyLauncher() 
    {
        if (y == 0)
        {
            Debug.Log("Grenade Launcher Bought");
            y++;
        }
        else
            Debug.Log("Already Bought");
    }
    public void buyGrenade(int kind)
    {
        if (kind == 1)
        {
            Debug.Log("Fire Grenade Bought");
        }
        else if (kind == 2)
        {
            Debug.Log("Ice Grenade Bought");
        }
        else
        {
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
    public void exitShop()
    {
        shopInteractable.instance.shopInteraction(false);
    }
    public void shopMenuWeapons(){
        gameManager.instance.shopMenu(false);
    }
    public void shopMenuUpgrades()
    {
        gameManager.instance.shopMenu(true);
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