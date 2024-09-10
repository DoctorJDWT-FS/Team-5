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
   

    public void Start()
    {
        playerWallet = FindObjectOfType<iWallet>();
    }
    public void StartButton()
    {
        //gameManager.instance.UpdateSliderValue(mainSlider.value);
        //gameManager.instance.toggleInvertY();
        SceneManager.LoadScene(1);
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
        player.currentGun.fireRate += player.currentGun.fireRateOrig/50;
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