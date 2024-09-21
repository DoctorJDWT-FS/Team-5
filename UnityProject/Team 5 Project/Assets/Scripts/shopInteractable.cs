using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shopInteractable : MonoBehaviour
{
    public static shopInteractable instance;
    public GameObject interact;
    public GameObject shopCanvas;
    public GameObject UIComplete;
    public GameObject upgradeItems;
    public GameObject weaponItems;
    public GameObject powerupItems;
    public List<GameObject> oneTimeButtons;
    public GameObject dronePosition;



    private bool isPlayerInRange = false;
    public bool shopOpen;
    public bool isPaused;

    private iWallet playerWallet;

    void Awake()
    {
        playerWallet = FindObjectOfType<iWallet>();
        instance = this;
    }
    // Update is called once per frame
    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            shopInteraction(true);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        // Check if the player has entered the trigger zone
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            interact.gameObject.SetActive(true); // Show the "Press E to Interact" message
            gameManager.instance.setDroneSpawn(dronePosition);
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
    public void shopInteraction(bool open)
    {
        if (!open)
        {
            gameManager.instance.stateUnpause();
            isPaused = false;
            shopOpen = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            interact.SetActive(true);
            shopCanvas.SetActive(false);
            upgradeItems.SetActive(false);
            weaponItems.SetActive(false);
            gameManager.instance.menuActive = null;
        }
        else
        {
            isPaused = true;
            shopOpen = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            interact.SetActive(false);
            shopCanvas.SetActive(true);
            upgradeItems.SetActive(true);
            weaponItems.SetActive(false);
            gameManager.instance.menuActive = shopCanvas;
            gameManager.instance.statePause();
        }
    }
}
