using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteraactionController : MonoBehaviour
{
    [Header("---- KEY Button ----")]
    [SerializeField] KeyCode keyButton;
    [Header("---- Object Info ----")]
    [SerializeField] string parameterName;

    [Header("---- Interaction Text Info ----")]// Animator component controlling the door
    public Text interactionText = null; // UI Text to display "Press E to open"

    [Header("---- Second Interaction Text Info ----")]
    [SerializeField] bool SecondText;
    public Text interactionText2 = null;//if there is a second state after interaction 

    private bool playerInRange = false;
    private Text activeText;
    private Animator myAnimator;
    private bool currentstate;

    void Start()
    {
        // Hide the interaction text initially
        interactionText.gameObject.SetActive(false);
        myAnimator = GetComponent<Animator>();
        activeText = interactionText;
        currentstate = myAnimator.GetBool(parameterName);
        if (SecondText)
        {
            interactionText2.gameObject.SetActive(false);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           activeText.gameObject.SetActive(true);
        }
    }
    void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            playerInRange = false;
           activeText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(keyButton))
        {
            // flips current state of parameter
            currentstate = !currentstate;
            myAnimator.SetBool(parameterName, currentstate);
            if (SecondText)
            {
                activeText.gameObject.SetActive(false);
                activeText = interactionText2;
                activeText.gameObject.SetActive(true);
            }
            
        }
    }

}
