using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damage : MonoBehaviour
{
    [SerializeField] enum damageType { bullet,enemyBullet,DroneBullet, stationary}
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;  

    [SerializeField] int damageAmount;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;

    [SerializeField] GameObject bloodEffectvfx; 
    [Range(0, 1)][SerializeField] float bleedChance = 0.5f;


    // Start is called before the first frame update
    void Start()
    {

        if(type == damageType.bullet)
        {
            rb.velocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }
        if(type == damageType.enemyBullet)
        {
            rb.velocity = (gameManager.instance.player.transform.position - (transform.position - new Vector3(0, 1, 0))).normalized * speed;
            Destroy(gameObject, destroyTime);
        }
        if(type == damageType.DroneBullet)
        {
            rb.velocity = (transform.forward - new Vector3(0, .1f, 0)).normalized * speed;
            Destroy(gameObject, destroyTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.isTrigger)
        {
            return;
        }

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null && other.CompareTag("Zombie"))
        {
            dmg.takeDamage(damageAmount);
            if (Random.value <= bleedChance)
            {
                //grabs the closest point on the shot position
                Vector3 hitPoint = other.ClosestPointOnBounds(transform.position);
                GameObject bloodEffect = Instantiate(bloodEffectvfx, hitPoint, Quaternion.identity);
                //attaches to body of zombie
                bloodEffect.transform.SetParent(other.transform);
                Destroy(bloodEffect, 0.5f); 
            }
        }
        Destroy(gameObject);
    }
}
