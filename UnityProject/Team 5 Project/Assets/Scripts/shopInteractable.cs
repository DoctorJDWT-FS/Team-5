using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shopInteractable : MonoBehaviour
{
    public static shopInteractable instance;
    public GameObject interact;
    public GameObject shopCanvas;
    public GameObject UIComplete;


    private bool isPlayerInRange = false;
    private bool shopOpen;
    public bool isPaused;

    void Awake()
    {
        instance = this;
    }
        // Update is called once per frame
        void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            shopOpen = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            shopInteraction();
        }

        if (Input.GetButtonDown("Cancel") && shopOpen)
        {
            shopOpen = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            shopInteraction();

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player has entered the trigger zone
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            interact.gameObject.SetActive(true); // Show the "Press E to Interact" message
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the player has exited the trigger zone
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            interact.gameObject.SetActive(false); // Hide the "Press E to Interact" message
            shopCanvas.gameObject.SetActive(false);     // Close the shop UI if the player leaves the trigger zone
        }
    }
    public void shopInteraction()
    {
        if (!shopOpen)
        {
            isPaused = false;
            UIComplete.SetActive(true);
            interact.SetActive(true);
            shopCanvas.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            isPaused = true;
            UIComplete.SetActive(false);
            interact.SetActive(false);
            shopCanvas.SetActive(true);
            Time.timeScale = 0;
        }
    }
}
