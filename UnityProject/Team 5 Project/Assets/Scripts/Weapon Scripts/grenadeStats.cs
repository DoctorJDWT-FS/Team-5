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

    public bool damagesPlayer;
    public bool affectsEnemiesOnly;
    public grenadeStats Clone()
    {
        return new grenadeStats
        {
            grenadeModel = this.grenadeModel,

            explosionDamage = this.explosionDamage,
            explosionRadius = this.explosionRadius,
            throwForce = this.throwForce,
            floorEffect = this.floorEffect,

            effectDuration = this.effectDuration,
            explosionEffect = this.explosionEffect,
            floorEffectStyle = this.floorEffectStyle,
            explosionSound = this.explosionSound,
            explosionVolume = this.explosionVolume,

            damagesPlayer = this.damagesPlayer,
            affectsEnemiesOnly = this.affectsEnemiesOnly,
};
    }

}
