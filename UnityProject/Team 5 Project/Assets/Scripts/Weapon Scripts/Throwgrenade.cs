using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Throwgrenade : MonoBehaviour
{
    [Header("----- Scripts -----")]
    [SerializeField] playerController playerScript;
    [SerializeField] cameraController cameraScript;
    [SerializeField] PlayerSettings playerSettings;
    [SerializeField] basicZombieAI zombieScript;

    [Header("----- Grenade Items -----")]
    [SerializeField] Transform grenadeThrowPos;
    [SerializeField] public grenadeStats currentGrenadeStats;
    [SerializeField] public List<grenadeStats> grenadeStatTypes = new List<grenadeStats>();
    public int currentGrenadeIndex = 0;
    [SerializeField] GameObject grenadeLandingPrefab;

    [Header("----- Settings -----")]
    [SerializeField] float destroyTime = 3f;
    [SerializeField] float minThrowAngle = -0.3f;
    [SerializeField] float minDistanceFromPlayer = 2.75f;
    [SerializeField] LayerMask groundMask;

    private GameObject grenadeLanding;
    private bool isHoldingGrenade;

    private void Start()
    {
        grenadeLanding = Instantiate(grenadeLandingPrefab);
        grenadeLanding.SetActive(false);

        currentGrenadeStats = grenadeStatTypes[currentGrenadeIndex];
    }
    public void startHoldingGrenade(grenadeStats grenadeType)
    {
        isHoldingGrenade = true;
        grenadeLanding.SetActive(true);
        currentGrenadeStats = grenadeType;
    }
    public void stopHoldingGrenade()
    {
        isHoldingGrenade = false;
        grenadeLanding.SetActive(false);
        throwGrenade();
    }
    private void Update()
    {
        if (Input.GetKey(playerSettings.grenade))
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                currentGrenadeIndex++;
                if (currentGrenadeIndex >= grenadeStatTypes.Count)
                {
                    currentGrenadeIndex = 0;
                }
                currentGrenadeStats = grenadeStatTypes[currentGrenadeIndex];
                gameManager.instance.UpdateGrenadeImages(currentGrenadeIndex);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                currentGrenadeIndex--;
                if (currentGrenadeIndex < 0)
                {
                    currentGrenadeIndex = grenadeStatTypes.Count - 1;
                }
                currentGrenadeStats = grenadeStatTypes[currentGrenadeIndex];
                gameManager.instance.UpdateGrenadeImages(currentGrenadeIndex);
            }

            startHoldingGrenade(grenadeStatTypes[currentGrenadeIndex]);
        }

        if (isHoldingGrenade)
        {
            updateGrenadeLandingPos();
        }
    }

    public void updateGrenadeLandingPos()
    {
        Ray ray = new Ray(cameraScript.transform.position, cameraScript.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, currentGrenadeStats.throwForce, groundMask))
        {
            float distanceToHit = Vector3.Distance(hit.point, grenadeThrowPos.position);
            if (distanceToHit >= minDistanceFromPlayer)
            {
                grenadeLanding.transform.position = hit.point;
            }
            else
            {
                grenadeLanding.transform.position = grenadeThrowPos.position + ray.direction * minDistanceFromPlayer;
            }
        }
        else
        {
            Vector3 maxRangePoint = grenadeThrowPos.position + ray.direction * currentGrenadeStats.throwForce;
            Ray groundRay = new Ray(maxRangePoint, Vector3.down);
            RaycastHit groundHit;

            if (Physics.Raycast(groundRay, out groundHit, Mathf.Infinity, groundMask))
            {
                grenadeLanding.transform.position = groundHit.point;
            }
            else
            {
                grenadeLanding.transform.position = maxRangePoint;
            }
        }
    }

    public void throwGrenade()
    {
        if (currentGrenadeStats.grenadeType == grenadeStats.GrenadeType.Fire && gameManager.instance.curFireGrenades <= 0)
        {
            Debug.Log("No Fire grenades available to throw!");
            return;
        }
        else if (currentGrenadeStats.grenadeType == grenadeStats.GrenadeType.Ice && gameManager.instance.curIceGrenades <= 0)
        {
            Debug.Log("No Ice grenades available to throw!");
            return;
        }
        else if (currentGrenadeStats.grenadeType == grenadeStats.GrenadeType.EMP && gameManager.instance.curEMPGrenades <= 0)
        {
            Debug.Log("No EMP grenades available to throw!");
            return;
        }

        grenadeStats grenadeToThrow = currentGrenadeStats.Clone();
        GameObject grenade = Instantiate(grenadeToThrow.grenadeModel, grenadeThrowPos.position, Quaternion.identity);
        Rigidbody grenadeRb = grenade.GetComponent<Rigidbody>();

        if (grenadeRb != null)
        {
            Vector3 throwDirection = cameraScript.transform.forward;
            grenadeRb.AddForce(throwDirection * grenadeToThrow.throwForce, ForceMode.VelocityChange);
        }

        StartCoroutine(HandleExplosionAfterGrenadeDestruction(grenade, grenadeToThrow));

        switch (currentGrenadeIndex)
        {
            case 0:
                gameManager.instance.curFireGrenades--;
                gameManager.instance.UpdateGrenadeCountDisplay();
                break;
            case 1:
                gameManager.instance.curIceGrenades--;
                gameManager.instance.UpdateGrenadeCountDisplay();
                break;
            case 2:
                gameManager.instance.curEMPGrenades--;
                gameManager.instance.UpdateGrenadeCountDisplay();
                break;
        }

        Debug.Log(grenadeToThrow.grenadeType + " Grenade Thrown");
    }


    IEnumerator HandleExplosionAfterGrenadeDestruction(GameObject grenade, grenadeStats grenadeStats)
    {
        yield return new WaitForSeconds(destroyTime);
        Vector3 explosionPosition = grenade.transform.position;

        Destroy(grenade);

        GameObject explosion = Instantiate(grenadeStats.explosionEffect, explosionPosition, Quaternion.identity);

        if (grenadeStats.explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(grenadeStats.explosionSound, explosionPosition, grenadeStats.explosionVolume);
        }

        Collider[] hitColliders = Physics.OverlapSphere(explosionPosition, grenadeStats.explosionRadius);
        foreach (var hitCollider in hitColliders)
        {
            IDamage target = hitCollider.GetComponent<IDamage>();
            if (target != null)
            {
                target.takeDamage((int)grenadeStats.explosionDamage);
            }
        }

        Destroy(explosion, 2f);

        yield return new WaitForSeconds(.5f);

        GameObject floorEffect = Instantiate(grenadeStats.floorEffectStyle, explosionPosition, Quaternion.identity);

        switch (currentGrenadeIndex)
        {
            case 0:
                DealDamageOverTime(explosionPosition, grenadeStats.effectDuration, currentGrenadeStats.floorEffect);
                break;

            case 1:
                ApplySlowEffect(explosionPosition, grenadeStats.effectDuration, currentGrenadeStats.floorEffect);
                break;

            case 2:
                ApplyStunEffect(explosionPosition, grenadeStats.effectDuration);
                break;
        }

        Destroy(floorEffect, grenadeStats.effectDuration);
    }

    private void DealDamageOverTime(Vector3 position, float duration, float damagePerSecond)
    {
        StartCoroutine(DamageCoroutine(position, duration, damagePerSecond));
    }

    private IEnumerator DamageCoroutine(Vector3 position, float duration, float damagePerSecond)
    {
        float endTime = Time.time + duration;

        while (Time.time < endTime)
        {
            Collider[] hitColliders = Physics.OverlapSphere(position, 1f);
            foreach (var hitCollider in hitColliders)
            {
                IDamage target = hitCollider.GetComponent<IDamage>();
                if (target != null)
                {
                    target.takeDamage((int)damagePerSecond);
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private void ApplySlowEffect(Vector3 position, float duration, float slowAmount)
    {
        Collider[] hitColliders = Physics.OverlapSphere(position, currentGrenadeStats.explosionRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                playerScript.applySlow(slowAmount, duration); 
            }
            else if (hitCollider.CompareTag("Zombie"))
            {
                var enemy = hitCollider.GetComponent<basicZombieAI>();
                if (enemy != null)
                {
                    enemy.applySlow(slowAmount, duration);
                }
            }
        }
    }

    private void ApplyStunEffect(Vector3 position, float duration)
    {
        Collider[] hitColliders = Physics.OverlapSphere(position, currentGrenadeStats.explosionRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Zombie"))
            {
                var enemy = hitCollider.GetComponent<basicZombieAI>();
                if (enemy != null)
                {
                    enemy.applyStun(duration);
                }
            }
        }
    }
}