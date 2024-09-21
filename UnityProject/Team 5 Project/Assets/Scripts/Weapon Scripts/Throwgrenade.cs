using System.Collections;
using UnityEngine;

public class Throwgrenade : MonoBehaviour
{
    [Header("----- Scripts -----")]
    [SerializeField] playerController playerScript;
    [SerializeField] cameraController cameraScript;

    [Header("----- Grenade Items -----")]
    [SerializeField] Transform grenadeThrowPos;
    [SerializeField] grenadeStats currentGrenadeStats;
    [SerializeField] GameObject grenadeLandingPrefab;

    [Header("----- Settings -----")]
    [SerializeField] float destroyTime = 3f;
    [SerializeField] float minThrowAngle = -0.3f;
    [SerializeField] float minDistanceFromPlayer = 2.75f;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float damageInterval = 0.5f;
    [SerializeField] int damagePerInterval = 5;

    private GameObject grenadeLanding;
    private bool isHoldingGrenade;

    private void Start()
    {
        grenadeLanding = Instantiate(grenadeLandingPrefab);
        grenadeLanding.SetActive(false);
    }

    public void startHoldingGrenade()
    {
        isHoldingGrenade = true;
        grenadeLanding.SetActive(true);
    }

    public void stopHoldingGrenade()
    {
        isHoldingGrenade = false;
        grenadeLanding.SetActive(false);
        throwGrenade();
    }

    private void Update()
    {
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
        GameObject grenade = Instantiate(currentGrenadeStats.grenadeModel, grenadeThrowPos.position, Quaternion.identity);
        Rigidbody grenadeRb = grenade.GetComponent<Rigidbody>();

        if (grenadeRb != null)
        {
            Vector3 throwDirection = cameraScript.transform.forward;

            grenadeRb.AddForce(throwDirection * currentGrenadeStats.throwForce, ForceMode.VelocityChange);
        }

        StartCoroutine(HandleExplosionAfterGrenadeDestruction(grenade));
    }


    IEnumerator HandleExplosionAfterGrenadeDestruction(GameObject grenade)
    {
        yield return new WaitForSeconds(destroyTime);
        Vector3 explosionPosition = grenade.transform.position;

        Destroy(grenade);

        GameObject explosion = Instantiate(currentGrenadeStats.explosionEffect, explosionPosition, Quaternion.identity);

        if (currentGrenadeStats.explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(currentGrenadeStats.explosionSound, explosionPosition, currentGrenadeStats.explosionVolume);
        }

        Collider[] hitColliders = Physics.OverlapSphere(explosionPosition, currentGrenadeStats.explosionRadius);
        foreach (var hitCollider in hitColliders)
        {
            IDamage target = hitCollider.GetComponent<IDamage>();
            if (target != null)
            {
                target.takeDamage((int)currentGrenadeStats.explosionDamage);
            }
        }

        Destroy(explosion, 2f);

        yield return new WaitForSeconds(0.5f);

        GameObject floorEffect = Instantiate(currentGrenadeStats.floorEffect, explosionPosition, Quaternion.identity);

        BoxCollider boxCollider = floorEffect.GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            boxCollider.enabled = true;
            StartCoroutine(HandleTriggerDamage(boxCollider));
        }

        Destroy(floorEffect, currentGrenadeStats.effectDuration);
    }

    IEnumerator HandleTriggerDamage(BoxCollider boxCollider)
    {
        float elapsedTime = 0;
        while (elapsedTime < currentGrenadeStats.effectDuration)
        {
            Collider[] hitColliders = Physics.OverlapBox(boxCollider.bounds.center, boxCollider.bounds.extents, boxCollider.transform.rotation);
            foreach (var hitCollider in hitColliders)
            {
                IDamage target = hitCollider.GetComponent<IDamage>();
                if (target != null)
                {
                    target.takeDamage(damagePerInterval);
                }
            }
            elapsedTime += damageInterval;
            yield return new WaitForSeconds(damageInterval);
        }
    }
}
