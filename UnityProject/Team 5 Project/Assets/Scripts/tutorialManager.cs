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
    [SerializeField] PlayerSettings playerSettings;

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
        buttonFuncScript = FindObjectOfType<buttonFunctions>();
        playerSettings = FindObjectOfType<PlayerSettings>();
        //grabs the script from the player settng and adust the button 
        objectives.Add($"Press {playerSettings.moveForward}, {playerSettings.strafeLeft}, {playerSettings.moveBackward}, {playerSettings.strafeRight} to Move Around");
        objectives.Add($"Press {playerSettings.sprint} to Sprint");
        objectives.Add($"Press {playerSettings.dash} to Dash");
        objectives.Add($"Press {playerSettings.jump} to Jump");
        objectives.Add($"Press {playerSettings.punch} to Punch");
        objectives.Add($"Press {playerSettings.grenade} to throw a grenade");
        objectives.Add($"Click {playerSettings.shoot} to Shoot");
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
        if (Input.GetKey(playerSettings.moveForward) || Input.GetKey(playerSettings.strafeLeft) || Input.GetKey(playerSettings.moveBackward) || Input.GetKey(playerSettings.strafeRight))
        {
            MissionComplete();
        }
    }
    void SprintMission()
    {
        if (Input.GetKey(playerSettings.sprint))
        {
            MissionComplete();
        }
    }

    void DashMission()
    {
        if (Input.GetKey(playerSettings.dash))
        {
            MissionComplete();
        }
    }
    void JumpMission()
    {
        if (Input.GetKey(playerSettings.jump))
        {
            MissionComplete();
        }
    }

    void MeleeMission()
    {
        if (Input.GetKey(playerSettings.punch))
        {
            MissionComplete();
        }
    }
    void GrenadeMission()
    {
        if (Input.GetKey(playerSettings.grenade))
        {
            MissionComplete();
        }
    }
    void ShootMission()
    {
        if (Input.GetKey(playerSettings.shoot))
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
        }
    }

    void MissionComplete()
    {
        currentObjective++;

        if (currentObjective < objectives.Count)
        {
            currentMission.text = objectives[currentObjective];
        }
        else
        {
            currentMission.text = "FIND AN EXIT";
            inTutorialMode = false;
            buttonFuncScript.TutorialDone();
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