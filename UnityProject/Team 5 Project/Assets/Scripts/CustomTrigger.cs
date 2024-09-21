using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CustomTrigger : MonoBehaviour
{
    public event System.Action<Collider> EnteredTrigger;
    public event System.Action<Collider> ExitTrigger;
    public event System.Action<Collider> StayTrigger;

    void OnTriggerEnter(Collider other)
    {
        EnteredTrigger?.Invoke(other); // Trigger the event for other scripts
    }

    void OnTriggerExit(Collider other)
    {
        ExitTrigger?.Invoke(other); // Trigger the event for other script
    }
    void OnTriggerStay(Collider other)
    {
        StayTrigger?.Invoke(other); // Trigger the event for other scripts
    }


}
