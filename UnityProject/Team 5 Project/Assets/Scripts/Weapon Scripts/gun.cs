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
    public float fireRate;
    public float reloadTime;

    [HideInInspector]
    public bool reloading;

    [Header("References")]
   // [SerializeField] private gunData gunData;
    [SerializeField] private Transform muzzle;
    [SerializeField] private GameObject bullet;


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
        if(!reloading && this.gameObject.activeSelf)
        {
            StartCoroutine(Reload());
        }
    }
    private IEnumerator Reload()
    {
        reloading = true;

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = magSize;

        reloading = false;
    }
    private bool canShoot() => !reloading && timeSinceLastShot > 1f / (fireRate / 60f);

    public void Shoot()
    {
        // Use this to test if player is shooting
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
    }

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;
        Debug.DrawRay(muzzle.position, muzzle.forward * maxDistance, Color.red);
    }

    private void onGunShot()
    {
        // Implement gunshot effects, sound, etc.

    }
}