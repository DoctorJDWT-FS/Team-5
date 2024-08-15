using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damage : MonoBehaviour
{
    [SerializeField] enum damageType { bullet, stationary}
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;  

    [SerializeField] int damageAmount;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;

    // Start is called before the first frame update
    void Start()
    {
        if(type == damageType.bullet)
        {
            rb.velocity = transform.forward* speed;
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

        if (dmg != null )
        {
            dmg.takeDamage(damageAmount);
        }

        Destroy(gameObject);
    }
}
