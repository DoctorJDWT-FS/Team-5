using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class healthKitStats : ScriptableObject
{
    [Range(1, 100)] public int healthGain;
  
}
