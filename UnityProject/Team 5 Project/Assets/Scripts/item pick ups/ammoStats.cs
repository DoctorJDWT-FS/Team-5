using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ammoStats : ScriptableObject
{
    [Range(1, 10)] public int ammoRecover;
    [Range(1, 10)] public int magRecover;


    void Update()
    {
       

    }
}
