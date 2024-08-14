using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkpoint : MonoBehaviour
{
    [SerializeField] Renderer model;

    Color colorigin;
    // Start is called before the first frame update
    void Start()
    {
        colorigin = model.material.color;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && gameManager.instance.playerSpawnPos.transform.position != this.transform.position)
        {
            gameManager.instance.playerSpawnPos.transform.position = transform.position;
            StartCoroutine(flashModel());
            StartCoroutine(flashCheckpoint());
        }
    }
    IEnumerator flashModel()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.3f);
        model.material.color = colorigin;
    }
    IEnumerator flashCheckpoint()
    {
        gameManager.instance.checkpointGet.SetActive(true);
        yield return new WaitForSeconds(1f);
        gameManager.instance.checkpointGet.SetActive(false);
    }
}
