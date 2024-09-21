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
    }
    public IEnumerator heavyPowerup()
    {
        player.increaseMaxHealth(100);
        player.heavySlow();
        yield return new WaitForSeconds(20f);
        player.increaseMaxHealth(-100);
        player.heavyReturn();
    }
}
