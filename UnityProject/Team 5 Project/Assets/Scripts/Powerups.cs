using System.Collections;
using UnityEngine;

public class Powerups : MonoBehaviour
{
    public playerController player;

    public void ActivateHeavyPowerup()
    {
        if (player != null)
        {
            StartCoroutine(heavyPowerup());
        }
        else
        {
            Debug.LogWarning("Player reference is missing in Powerups script.");
        }
    }
    public IEnumerator heavyPowerup()
    {
        player.increaseMaxHealth(100);
        player.heavySlow();
        Debug.Log("before wait");
        yield return new WaitForSeconds(20f);
        Debug.Log("after wait");
        player.increaseMaxHealth(-100);
        player.heavyReturn();
    }
}
