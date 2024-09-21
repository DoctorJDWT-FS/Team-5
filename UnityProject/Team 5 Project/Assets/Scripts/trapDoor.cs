using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trapDoor : MonoBehaviour
{
    [SerializeField] GameObject[] enableList;
    private Animator animator;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.GetComponent<Animator>().SetBool("Closed", true);
        }
        for (int i = 0; i < enableList.Length; i++)
        {
            enableList[i].SetActive(true);
        }
    }
}
