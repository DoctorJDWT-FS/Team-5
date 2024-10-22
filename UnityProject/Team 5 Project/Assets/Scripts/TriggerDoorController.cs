using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TriggerDoorController : MonoBehaviour
{
    [SerializeField] private Animator myDoor = null;
    [SerializeField] private string doorOpenAnimationName; // Specify the door open animation in Inspector

    private bool isPlayerInZone = false;
    private bool hasDoorOpened = false;
    private iWallet playerWallet;

    // Reference to the UI Text for "Press E to open" message
    private GameObject pressEMessage;
    private TMP_Text pressEText;

    // Reference to the UI Text for "Cost:" message
    private GameObject costMessage;

    // Reference to the UI Text for the cost of the door
    private TMP_Text doorCostStatUI;

    // Set how much the door should cost. If free, set to 0
    [SerializeField] public int doorCost;

    [SerializeField] private GameObject[] disableList;
    [SerializeField] private GameObject[] enableList;
    [SerializeField] protected AudioSource doorSounds;
    [SerializeField] protected AudioClip openDoor;
    [SerializeField] protected bool isLaser;
    [SerializeField] protected bool ignoreTutorial;
    [SerializeField] protected int laserPersistTime;

    private void Awake()
    {
        playerWallet = FindObjectOfType<iWallet>();
        GameObject doorcostobject = GameObject.Find("DoorCostStat");
        doorCostStatUI = doorcostobject.GetComponent<TMP_Text>();
        costMessage = GameObject.Find("Cost Message");
        pressEMessage = GameObject.Find("Press E Message");
        pressEText = GameObject.Find("PressEtoOpen").GetComponent<TMP_Text>();
    }

    private void Start()
    {
        costMessage.SetActive(false);
        pressEMessage.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isLaser && other.CompareTag("Player") && !hasDoorOpened)
        {
            pressEText.text = "Press E to activate!";
        }
        if (!isLaser && other.CompareTag("Player") && !hasDoorOpened)
        {
            pressEText.text = "Press E to open!";
        }
        if (other.CompareTag("Player") && !hasDoorOpened)
        {
            isPlayerInZone = true;
            doorCostStatUI.text = "" + doorCost; // Set the cost UI equal to the cost of the door
            if (doorCost > 0) costMessage.SetActive(true); // Show the cost if the cost is more than 0
            pressEMessage.SetActive(true); // Show the "Press E to open" message
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            costMessage.SetActive(false); // Hide the cost
            pressEMessage.SetActive(false); // Hide the "Press E to open" message

        }
    }

    private void Update()
    {
        if (isPlayerInZone && Input.GetKeyDown(KeyCode.E) && !hasDoorOpened && playerWallet.Credits >= doorCost)
        {
            //if the tutorial is false then this plays  
            if (!FindObjectOfType<tutorialManager>().TutorialMode())
            {
                OpenDoor();
            }
            else
            {  
               if(FindObjectOfType<tutorialManager>().DoorMissionON())
                {
                    FindObjectOfType<tutorialManager>().Doorbrought();
                    OpenDoor();
                }

            }
            if (ignoreTutorial)
            {
                OpenDoor();
            }
        }
    }

    private void OpenDoor()
    {
        playerWallet.SpendCredits(doorCost);
        if (doorSounds != null)
        {
            doorSounds.clip = openDoor;
            doorSounds.Play();
        }
        if (myDoor != null)
        {
            myDoor.Play(doorOpenAnimationName, 0, 0.0f); // Play the specified animation
        }
        hasDoorOpened = true;
        costMessage.SetActive(false); // Hide the cost
        pressEMessage.SetActive(false); // Hide the message after the door is opened
        if (disableList != null)
        {
            for (int i = 0; i < disableList.Length; i++)
            {
                disableList[i].SetActive(false);
            }
        }
        if (enableList != null)
        {
            for (int i = 0; i < enableList.Length; i++)
            {
                enableList[i].SetActive(true);
            }
        }
        if (isLaser)
        {
            StartCoroutine(laser());
        }
    }

    IEnumerator laser()
    {
        yield return new WaitForSeconds(laserPersistTime);
        hasDoorOpened = false;
        for (int i = 0; i < enableList.Length; ++i)
        {
            enableList[i].SetActive(false);
        }
    }
}
