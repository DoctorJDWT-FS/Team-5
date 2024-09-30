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
    public float floorEffect;

    public float effectDuration;
    public GameObject explosionEffect;
    public GameObject floorEffectStyle;
    public AudioClip explosionSound;
    public float explosionVolume;


    public grenadeStats Clone()
    {
        grenadeStats clonedStats = ScriptableObject.CreateInstance<grenadeStats>();

        clonedStats.grenadeModel = this.grenadeModel;
        clonedStats.explosionDamage = this.explosionDamage;
        clonedStats.explosionRadius = this.explosionRadius;
        clonedStats.throwForce = this.throwForce;
        clonedStats.floorEffect = this.floorEffect;

        clonedStats.effectDuration = this.effectDuration;
        clonedStats.explosionEffect = this.explosionEffect;
        clonedStats.floorEffectStyle = this.floorEffectStyle;
        clonedStats.explosionSound = this.explosionSound;
        clonedStats.explosionVolume = this.explosionVolume;

        return clonedStats;
    }



}
