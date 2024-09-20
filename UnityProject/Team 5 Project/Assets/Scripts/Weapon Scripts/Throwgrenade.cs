using UnityEngine;

public class Throwgrenade : MonoBehaviour
{
    [Header("----- Scripts -----")]
    [SerializeField] playerController playerScript;
    [SerializeField] GameObject grenadePrefab;
    [SerializeField] Transform grenadeThrowPos;
    [SerializeField] cameraController cameraScript;
    [SerializeField] GameObject grenadeLandingPrefab;

    [Header("----- Grenade Items -----")]
    [SerializeField] float throwForce = 10f;
    [SerializeField] float destroyTime = 3f;
    [SerializeField] float minThrowAngle = -0.3f;
    [SerializeField] float maxThrowRange = 10f;
    [SerializeField] float minDistanceFromPlayer = 2.75f;  
    [SerializeField] LayerMask groundMask;

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

        if (Physics.Raycast(ray, out hit, maxThrowRange, groundMask))
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
            Vector3 maxRangePoint = grenadeThrowPos.position + ray.direction * maxThrowRange;
            grenadeLanding.transform.position = maxRangePoint;
        }
    }

    public void throwGrenade()
    {
        GameObject grenade = Instantiate(grenadePrefab, grenadeThrowPos.position, grenadeThrowPos.rotation);
        Rigidbody grenadeRb = grenade.GetComponent<Rigidbody>();

        if (grenadeRb != null)
        {
            Vector3 throwDirection = cameraScript.transform.forward;

            if (throwDirection.y < minThrowAngle)
            {
                throwDirection.y = minThrowAngle;
            }

            grenadeRb.AddForce(throwDirection * throwForce, ForceMode.VelocityChange);
        }

        Destroy(grenade, destroyTime);
        Debug.Log("Grenade Thrown");
    }
}
