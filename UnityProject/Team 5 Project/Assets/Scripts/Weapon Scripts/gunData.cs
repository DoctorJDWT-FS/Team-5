using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Gun",menuName ="Weapon/Gun")]

public class gunData : ScriptableObject
{
    [Header("Info")]
        public new string name;
    [Header("Shooting")]
        public float damage;
        public float maxDistance;
        public float accuracySpread;

    [Header("Reloading")]
        public int currentAmmo;
        public int magSize;
        public float fireRate;
        public float reloadTime;
    [HideInInspector]
        public bool reloading;
}
