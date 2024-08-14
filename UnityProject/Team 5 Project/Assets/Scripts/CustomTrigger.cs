using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CustomTrigger : MonoBehaviour
{
    public event System.Action<Collider> EnteredTrigger;
    public event System.Action<Collider> ExitTrigger;

    void OnTriggerEnter(Collider other)
    {
        EnteredTrigger?.Invoke(other); // Trigger the event for other scripts

       
    }

    void OnTriggerExit(Collider other)
    {
        ExitTrigger?.Invoke(other); // Trigger the event for other scripts

    }

   
}
