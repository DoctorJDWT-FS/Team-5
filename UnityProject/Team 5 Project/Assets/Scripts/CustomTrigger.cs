using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomTrigger : MonoBehaviour
{
    public event System.Action<Collider> EnteredTrigger;
    public event System.Action<Collider> ExitTrigger;

    public Animator doorAnimator; // Animator component controlling the door
    public Text interactionText; // UI Text to display "Press E to open"

    private bool playerInRange = false;

    void Start()
    {
        // Hide the interaction text initially
        interactionText.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        EnteredTrigger?.Invoke(other); // Trigger the event for other scripts

        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            interactionText.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        ExitTrigger?.Invoke(other); // Trigger the event for other scripts

        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            interactionText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // Toggle door open/close animation
            bool isOpen = doorAnimator.GetBool("isOpen");
            doorAnimator.SetBool("isOpen", !isOpen);
        }
    }
}
