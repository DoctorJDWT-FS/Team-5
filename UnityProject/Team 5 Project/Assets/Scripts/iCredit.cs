using UnityEngine;

public class iCredit : MonoBehaviour
{
    [SerializeField] private int creditValue = 10;

    public void AwardCredits()
    {
        // Find the player's wallet
        iWallet playerWallet = FindObjectOfType<iWallet>();

        if (playerWallet != null)
        {
            // Add credits to the player's wallet
            playerWallet.AddCredits(creditValue);
        }
    }
}
