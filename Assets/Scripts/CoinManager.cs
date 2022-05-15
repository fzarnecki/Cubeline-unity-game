using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    // Players amount of coins + string to access them
    private int coins;
    private string coinsPrefsName = "coins";

    [Header("Coin spawning components")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private float coinYAdjust = 1f;
    [SerializeField] private float coinXAdjust = 2f;

    // Value added for each coin picked up
    private int coinValue = 10;

    /***/

    void Start()
    {
        coins = PlayerPrefs.GetInt(coinsPrefsName, 0);
    }
    

    public void IncreaseCoins(int amount)
    {
        coins += amount;
        PlayerPrefs.SetInt(coinsPrefsName, coins);
    }

    public void DecreaseCoins(int amount)
    {
        coins -= amount;
        PlayerPrefs.SetInt(coinsPrefsName, coins);
    }

    public int GetCoins() { return coins; }
    public void DestroyCoin() { Destroy(gameObject); }
    public GameObject GetCoinPrefab() { return coinPrefab; }

    public int GetCoinValue() { return coinValue; }
}
