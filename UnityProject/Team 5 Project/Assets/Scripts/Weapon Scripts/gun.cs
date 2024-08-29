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
        else
        {
            Debug.Log("No magazines left!");
        }

        reloading = false;
    }

    private bool canShoot() => !reloading && timeSinceLastShot > 1f / (fireRate / 60f);

    public void Shoot()
    {
        Debug.Log("Shot Gun!");
        Debug.Log("Current Ammo Before: " + currentAmmo);

        if (currentAmmo > 0)
        {
            if (canShoot())
            {
                if (Physics.Raycast(muzzle.position, muzzle.forward, out RaycastHit hitInfo, maxDistance))
                {
                    Debug.Log(hitInfo.transform.name);
                }
                else
                {
                    Debug.Log("No hit detected");
                }

                GameObject bulletInstance = Instantiate(bullet, muzzle.position, muzzle.rotation);
                currentAmmo--;
                timeSinceLastShot = 0;
                onGunShot();
            }
        }
        else
        {
            Debug.Log("Out of ammo, reload needed.");
        }
    }

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;
        Debug.DrawRay(muzzle.position, muzzle.forward * maxDistance, Color.red);
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
        else
        {
            Debug.LogWarning("Audio source or clip is not assigned.");
        }
    }

    IEnumerator flashMuzzle()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        muzzleFlash.SetActive(false);
    }
}
