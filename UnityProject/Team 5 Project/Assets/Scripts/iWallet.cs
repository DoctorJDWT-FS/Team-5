using UnityEngine;

public class iWallet : MonoBehaviour
{
    [SerializeField] private int startingCredits = 0;
    [SerializeField] private int currentCredits;

    public int Credits { get; private set; }

    private void Start()
    {
        // Initialize credits with the starting value
        Credits = startingCredits;
        currentCredits = Credits; // Keep currentCredits in sync if needed
    }

    // Method allows other scripts to add credits to the player's wallet
    public void AddCredits(int amount)
    {
        Credits += amount;
        currentCredits = Credits; // Update currentCredits to reflect the change
        Debug.Log("Credits added: " + amount + ". Total Credits: " + Credits);

        // Update the credit display in the game manager
        if (gameManager.instance != null)
        {
            gameManager.instance.UpdateCreditDisplay();
        }
    }

    // Method to remove credits
    public bool SpendCredits(int amount)
    {
        if (Credits >= amount)
        {
            Credits -= amount;
            currentCredits = Credits; // Update currentCredits to reflect the change
            Debug.Log("Credits spent: " + amount + ". Remaining Credits: " + Credits);

            // Update the credit display in the game manager
            if (gameManager.instance != null)
            {
                gameManager.instance.UpdateCreditDisplay();
            }

            return true;
        }
        else
        {
            Debug.Log("Not enough credits!");
            return false;
        }
    }
}
