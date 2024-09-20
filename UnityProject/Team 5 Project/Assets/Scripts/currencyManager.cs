using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class currencyManager : MonoBehaviour
{
    [SerializeField] int startingCurrency;
    //this code will allow zombies to accese this wallet at all times
    public static currencyManager instance;
    public int currentCurrency;

    // Event for updating the UI or notifying other systems
    public delegate void OnCurrencyChanged(int newAmount);
    public event OnCurrencyChanged currencyChangedEvent;
    [SerializeField] private AudioSource currencyAudio;

    // Start is called before the first frame update
    void Start()
    {
        currentCurrency = startingCurrency;
        NotifyCurrencyChanged();
    }

    public void AddCurrency(int amount)
    {
        currentCurrency += amount;
        NotifyCurrencyChanged();
    }

    public bool SpendCurrency(int amount)
    {
        if (currentCurrency >= amount)
        {
            currentCurrency -= amount;
            NotifyCurrencyChanged();
            return true; // Transaction successful
        }
        else
        {
            return false; // Not enough currency
        }
    }

    public int GetCurrentCurrency()
    {
        return currentCurrency;
    }

    // Notify UI or other systems that the currency has changed
    private void NotifyCurrencyChanged()
    {
        if (currencyChangedEvent != null)
        {
            currencyChangedEvent(currentCurrency);
        }
    }
}
