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
            if (Input.GetAxis("Mouse ScrollWheel") > 0 && currentGrenadeIndex < grenadeStatTypes.Count - 1)
            {
                currentGrenadeIndex = (currentGrenadeIndex + 1) % grenadeStatTypes.Count;
                currentGrenadeStats = grenadeStatTypes[currentGrenadeIndex];
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0 && currentGrenadeIndex > 0)
            {
                currentGrenadeIndex = (currentGrenadeIndex - 1 + grenadeStatTypes.Count) % grenadeStatTypes.Count;
                currentGrenadeStats = grenadeStatTypes[currentGrenadeIndex];
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
        grenadeStats grenadeToThrow = currentGrenadeStats.Clone();
        GameObject grenade = Instantiate(grenadeToThrow.grenadeModel, grenadeThrowPos.position, Quaternion.identity);
        Rigidbody grenadeRb = grenade.GetComponent<Rigidbody>();

        if (grenadeRb != null)
        {
            Vector3 throwDirection = cameraScript.transform.forward;
            grenadeRb.AddForce(throwDirection * grenadeToThrow.throwForce, ForceMode.VelocityChange);
        }

        StartCoroutine(HandleExplosionAfterGrenadeDestruction(grenade, grenadeToThrow));

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

        if (grenadeStats.grenadeType == grenadeStats.GrenadeType.Fire) 
        {
            DealDamageOverTime(explosionPosition, grenadeStats.effectDuration, grenadeStats.floorEffect);
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
}
