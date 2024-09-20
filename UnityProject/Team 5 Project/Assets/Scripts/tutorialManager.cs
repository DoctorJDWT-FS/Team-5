using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Security.Cryptography;

public class tutorialManager : MonoBehaviour
{

    [SerializeField] TMP_Text currentMission;
    List<string> objectives = new List<string>
    {
        "Press W,A,S,D to Move Around",
        "Press Left Shift to Sprint",
         "Press Left Control to Dash",
         "Press Space to Jump",
         "Press V to Punch",
         "Click Left mouse to Shoot",
         "Buy a health upgrade at the store",
         "Open a Door"

    };
    int currentObjective;
    bool purcheseUpgrade = false;
    bool doorOpen = false;
    bool inTutorialMode = true;
    bool doorMission = false;
    // Start is called before the first frame update

    void Start()
    {
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
                    ShootMission();
                    break;
                case 6:
                    StoreMission();
                    break;
                case 7:
                    DoorMission();
                    break;

            }
        }
    }

    void movementMission()
    {
        if (Input.GetKey(KeyCode.W)|| Input.GetKey(KeyCode.A)|| Input.GetKey(KeyCode.S)|| Input.GetKey(KeyCode.D))
        {
            MissionComplete();
        }
    }
    void SprintMission()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            MissionComplete();
        }
    }

    void DashMission()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            MissionComplete();
        }
    }
    void JumpMission()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            MissionComplete();
        }
    }

    void MeleeMission()
    {
        if (Input.GetKey(KeyCode.V))
        {
            MissionComplete();
        }
    }
    void ShootMission()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            MissionComplete();
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
    public bool TutorialMode()
    {
        return inTutorialMode;
    }
}
