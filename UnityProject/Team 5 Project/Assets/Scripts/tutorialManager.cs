using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Security.Cryptography;
using UnityEditor;

public class tutorialManager : MonoBehaviour
{

    [SerializeField] TMP_Text currentMission;
    [SerializeField] buttonFunctions buttonFuncScript;
    [SerializeField] public PlayerSettings playerSettings;
    [SerializeField] AudioSource audPlayer;
    [SerializeField] AudioClip taskSound;

    List<string> objectives = new List<string>();
    int currentObjective;
    bool purcheseUpgrade = false;
    bool doorOpen = false;
    bool inTutorialMode = true;
    bool doorMission = false;
    bool storeMission = false;

    // Start is called before the first frame update

    void Start()
    {
        
        playerSettings = FindObjectOfType<PlayerSettings>();
        //grabs the script from the player setting and adjust the button 
        objectives.Add("Press W, A, S, D to Move Around");
        objectives.Add("Press Left Shift to Sprint");
        objectives.Add("Press Left Control to Dash");
        objectives.Add("Press Space to Jump");
        objectives.Add("Press V to Punch");
        objectives.Add("Press G to throw a grenade");
        objectives.Add("Click Left Mouse Button to Shoot");
        objectives.Add("Buy a health upgrade at the store");
        objectives.Add("Open a Door");
        currentMission.text = objectives[currentObjective];
    }

    private void Update()
    {
        if (inTutorialMode)
        {


            switch (currentObjective)
            {
                case 0:
                    movementMission();
                    break;
                case 1:
                    SprintMission();
                    break;
                case 2:
                    DashMission();
                    break;
                case 3:
                    JumpMission();
                    break;
                case 4:
                    MeleeMission();
                    break;
                case 5:
                    GrenadeMission();
                    break;
                case 6:
                    ShootMission();
                    break;
                case 7:
                    StoreMission();
                    break;
                case 8:
                    DoorMission();
                    break;

            }
        }
    }

    void movementMission()
    {
        if (Input.GetKey(gameManager.instance.playerSettings.keys["moveForward"]) || Input.GetKey(gameManager.instance.playerSettings.keys["strafeLeft"]) || Input.GetKey(gameManager.instance.playerSettings.keys["moveBackwards"]) || Input.GetKey(gameManager.instance.playerSettings.keys["strafeRight"]))
        {
            MissionComplete();
        }
    }
    void SprintMission()
    {
        if (Input.GetKey(gameManager.instance.playerSettings.keys["sprint"]))
        {
            MissionComplete();
        }
    }

    void DashMission()
    {
        if (Input.GetKey(gameManager.instance.playerSettings.keys["dash"]))
        {
            MissionComplete();
        }
    }
    void JumpMission()
    {
        if (Input.GetKey(gameManager.instance.playerSettings.keys["jump"]))
        {
            MissionComplete();
        }
    }

    void MeleeMission()
    {
        if (Input.GetKey(gameManager.instance.playerSettings.keys["punch"]))
        {
            MissionComplete();
            gameManager.instance.curFireGrenades = gameManager.instance.maxFireGrenades;
            gameManager.instance.UpdateGrenadeCountDisplay();
        }
    }
    void GrenadeMission()
    {
        if (Input.GetKey(gameManager.instance.playerSettings.keys["grenade"]))
        {
            MissionComplete();
        }
    }
    void ShootMission()
    {
        if (Input.GetKey(gameManager.instance.playerSettings.keys["shoot"]))
        {
            MissionComplete();
            storeMission = true;
        }
    }

    void StoreMission()
    {
        if (purcheseUpgrade)
        {
            MissionComplete();
            doorMission = true;

        }
    }
    void DoorMission()
    {
        if (doorOpen)
        {
            MissionComplete();
            gameManager.instance.buttonTutorial.TutorialModeComplete();
        }
    }

    void MissionComplete()
    {
        audPlayer.PlayOneShot(taskSound);
        currentObjective++;

        if (currentObjective < objectives.Count)
        {
            currentMission.text = objectives[currentObjective];
        }
        else
        {
            currentMission.text = "FIND AN EXIT";
            inTutorialMode = false;
            
        }

    }
    public void Upgradebrought()
    {
        if (!inTutorialMode)
        {
            return;
        }
        else
        {
            purcheseUpgrade = true;
        }
    }
    public void Doorbrought()
    {
        if (!inTutorialMode)
        {
            return;
        }
        else
        {
            doorOpen = true;
        }
    }

    public bool DoorMissionON()
    {
        return doorMission;
    }
    public bool StoreMissionON()
    {
        return storeMission;
    }
    public bool TutorialMode()
    {
        return inTutorialMode;
    }
}