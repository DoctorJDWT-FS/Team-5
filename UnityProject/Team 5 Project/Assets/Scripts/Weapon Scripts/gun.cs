using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gun : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private gunData gunData;
    [SerializeField] private Transform muzzle;
    [SerializeField] private GameObject bullet;

    float timeSinceLastShot;

    private void Start()
    {
        playerController.shootInput += Shoot;
        playerController.reloadInput += StartReload;
    }

    public void StartReload()
    {
        if(!gunData.reloading)
        {
            StartCoroutine(Reload());
        }
    }
    private IEnumerator Reload()
    {
        gunData.reloading = true;

        yield return new WaitForSeconds(gunData.reloadTime);

        gunData.currentAmmo = gunData.magSize;

        gunData.reloading = false;
    }
    private bool canShoot() => !gunData.reloading && timeSinceLastShot > 1f / (gunData.fireRate / 60f);

    public void Shoot()
    {
        // Use this to test if player is shooting
         Debug.Log("Shot Gun!");
        Debug.Log("Current Ammo Before: " + gunData.currentAmmo);

        if (gunData.currentAmmo > 0)
        {
            if (canShoot())
            {
                if (Physics.Raycast(muzzle.position, muzzle.forward, out RaycastHit hitInfo, gunData.maxDistance))
                {
                    Debug.Log(hitInfo.transform.name);
                }
                else
                {
                    Debug.Log("No hit detected");
                }
                GameObject bulletInstance = Instantiate(bullet, muzzle.position, muzzle.rotation);

                gunData.currentAmmo--;
                timeSinceLastShot = 0;
                onGunShot();
            }
        }
    }

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;
        Debug.DrawRay(muzzle.position, muzzle.forward * gunData.maxDistance, Color.red);
    }

    private void onGunShot()
    {
        // Implement gunshot effects, sound, etc.

    }
}