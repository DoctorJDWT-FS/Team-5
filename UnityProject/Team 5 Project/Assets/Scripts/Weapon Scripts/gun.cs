using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gun : MonoBehaviour
{
    [Header("Info")]
    public new string name;

    [Header("Type")]
    public WeaponType weaponType;

    [Header("Shooting")]
    public float maxDistance;
    public float accuracySpread;

    [Header("Stats")]
    public int currentAmmo;
    public int magSize;
    public int currentMagazines;  // Current number of magazines available
    public int maxMagazines;      // Maximum number of magazines the player can carry
    public float fireRate;
    public float fireRateOrig;
    public float reloadTime;

    [HideInInspector]
    public bool reloading;

    [Header("References")]
    [SerializeField] private Transform muzzle;
    [SerializeField] private GameObject bullet;
    [SerializeField] private AudioSource gunAudioSource;
    [SerializeField] private AudioClip shootClip;        
    [SerializeField] private AudioClip reloadClip;
    [SerializeField] GameObject muzzleFlash;

    float timeSinceLastShot;

    private void Start()
    {
        playerController.shootInput += Shoot;
        playerController.reloadInput += StartReload;
        fireRateOrig = fireRate;
    }

    private void OnDisable() => reloading = false;

    public enum WeaponType
    {
        Pistol,
        Rifle
    }

    public void StartReload()
    {
        if (!reloading && this.gameObject.activeSelf && currentAmmo < magSize && currentMagazines > 0)
        {
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        reloading = true;
        PlaySound(reloadClip);
        yield return new WaitForSeconds(reloadTime);

        if (currentMagazines > 0)
        {
            currentMagazines--;
            currentAmmo = magSize;
        }
        reloading = false;
    }

    private bool canShoot() => !reloading && timeSinceLastShot > 1f / (fireRate / 60f);

    public void Shoot()
    {
        if (currentAmmo > 0)
        {
            if (canShoot())
            {
                GameObject bulletInstance = Instantiate(bullet, muzzle.position, muzzle.rotation);
                currentAmmo--;
                timeSinceLastShot = 0;
                onGunShot();
            }
        }
    }

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;
    }

    private void onGunShot()
    {
        StartCoroutine(flashMuzzle());
        PlaySound(shootClip);
    }

    private void PlaySound(AudioClip clip)
    {
        if (gunAudioSource != null && clip != null)
        {
            gunAudioSource.clip = clip;
            gunAudioSource.Play();
        }
    }

    IEnumerator flashMuzzle()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        muzzleFlash.SetActive(false);
    }
}
