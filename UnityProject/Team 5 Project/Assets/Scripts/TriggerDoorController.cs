using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDoorController : MonoBehaviour
{
    [SerializeField] private Animator myDoor = null;
    [SerializeField] private string doorOpenAnimationName; // Specify the door open animation in Inspector

    private bool isPlayerInZone = false;
    private bool hasDoorOpened = false;

    // Reference to the UI Text for "Press E to open" message
    [SerializeField] private GameObject pressEMessage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasDoorOpened)
        {
            isPlayerInZone = true;
            pressEMessage.SetActive(true); // Show the "Press E to open" message
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            pressEMessage.SetActive(false); // Hide the "Press E to open" message
        }
    }

    private void Update()
    {
        if (isPlayerInZone && Input.GetKeyDown(KeyCode.E) && !hasDoorOpened)
        {
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        myDoor.Play(doorOpenAnimationName, 0, 0.0f); // Play the specified animation
        hasDoorOpened = true;
        pressEMessage.SetActive(false); // Hide the message after the door is opened
    }
}
