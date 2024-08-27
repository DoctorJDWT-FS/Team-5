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
    [SerializeField] private GameObject pressEMessage;

    // Reference to the UI Text for "Cost:" message
    [SerializeField] private GameObject costMessage;

    // Reference to the UI Text for the cost of the door
    [SerializeField] private TMP_Text doorCostStatUI;

    // Set how much the door should cost. If free, set to 0
    [SerializeField] private int doorCost;

    private void Start()
    {
        playerWallet = FindObjectOfType<iWallet>();
    }

    private void OnTriggerEnter(Collider other)
    {
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
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        playerWallet.SpendCredits(doorCost);
        myDoor.Play(doorOpenAnimationName, 0, 0.0f); // Play the specified animation
        hasDoorOpened = true;
        costMessage.SetActive(false); // Hide the cost
        pressEMessage.SetActive(false); // Hide the message after the door is opened
    }
}
