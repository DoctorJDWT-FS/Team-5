using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class grenadeStats : ScriptableObject
{
    public enum GrenadeType { Fire, Ice, EMP }
    public GrenadeType grenadeType;

    public GameObject grenadeModel;

    public float explosionDamage;
    public float explosionRadius;
    public float throwForce;

    public float effectDuration;
    public GameObject explosionEffect;
    public GameObject floorEffect;
    public AudioClip explosionSound;
    public float explosionVolume;

    public bool damagesPlayer;
    public bool affectsEnemiesOnly;
}
