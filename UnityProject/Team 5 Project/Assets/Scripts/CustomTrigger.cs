using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    public event System.Action<Collider> EnteredTrigger;
    public event System.Action<Collider> ExitTrigger;

    void OnTriggerEnter(Collider other)
    {
        EnteredTrigger?.Invoke(other);
    }
    void OnTriggerExit(Collider other)
    {
        ExitTrigger?.Invoke(other); 
    }
}
