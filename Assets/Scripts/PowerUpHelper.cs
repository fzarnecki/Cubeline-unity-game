using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerUpHelper : MonoBehaviour
{
    [Header("Prices of given power ups")]
    private int[] invincibilityPrices = { 0, 100, 1000, 2500, 10000 };
    private int[] doublePointsPrices = { 0, 100, 500, 1500, 10000 };
    private int[] moneyRainPrices = { 0, 100, 1000, 2500, 10000 };
    private int[] deflectionPrices = { 0, 100, 500, 1500, 10000 };

    [Header("Sliders to visually show power up progression level")]
    [SerializeField] private Slider invincibilitySlider;
    [SerializeField] private Slider doublePointsSlider;
    [SerializeField] private Slider moneyRainSlider;
    [SerializeField] private Slider deflectionSlider;

    [Header("Text objects to display price to")]
    [SerializeField] private TextMeshProUGUI invincibilityPrice;
    [SerializeField] private TextMeshProUGUI doublePointsPrice;
    [SerializeField] private TextMeshProUGUI moneyRainPrice;
    [SerializeField] private TextMeshProUGUI deflectionPrice;

    [Header("Other working scripts")]
    [SerializeField] private PowerUpManager powerUpManager;
    [SerializeField] private CoinManager coinManager;
    [SerializeField] private ShopManager shopManager;

    /***/

    private void Start()
    {
        UpdateInfo();
    }

    private void UpdateInfo()
    {
        int level;

        level = PlayerPrefs.GetInt(powerUpManager.InvincibilityLevelName());
        invincibilitySlider.value = level;
        if (level < powerUpManager.GetNrOfPowerUps())
            invincibilityPrice.text = invincibilityPrices[level + 1].ToString();
        else
            invincibilityPrice.text = "Max";

        level = PlayerPrefs.GetInt(powerUpManager.DoublePointsLevelName());
        doublePointsSlider.value = level;
        if (level < powerUpManager.GetNrOfPowerUps())
            doublePointsPrice.text = doublePointsPrices[level + 1].ToString();
        else
            doublePointsPrice.text = "Max";

        level = PlayerPrefs.GetInt(powerUpManager.MoneyRainLevelName());
        moneyRainSlider.value = level;
        if (level < powerUpManager.GetNrOfPowerUps())
            moneyRainPrice.text = moneyRainPrices[level + 1].ToString();
        else
            moneyRainPrice.text = "Max";

        level = PlayerPrefs.GetInt(powerUpManager.DeflectionLevelName());
        deflectionSlider.value = level;
        if (level < powerUpManager.GetNrOfPowerUps())
            deflectionPrice.text = deflectionPrices[level + 1].ToString();
        else
            deflectionPrice.text = "Max";
    }

    public void UpgradeInvincibility()
    {
        var level = PlayerPrefs.GetInt(powerUpManager.InvincibilityLevelName());

        if (level >= powerUpManager.GetNrOfPowerUps())   // maxed out
            return;
        else if (coinManager.GetCoins() < invincibilityPrices[level + 1])   // too pricy
        {
            StartCoroutine(shopManager.SpawnNotEnoughCoinsPowerUps());
            return;
        }
        else
        {
            coinManager.DecreaseCoins(invincibilityPrices[level + 1]);
            PlayerPrefs.SetInt(powerUpManager.InvincibilityLevelName(), level + 1);
            UpdateInfo();
        }
    }
    public void UpgradeDoublePoints()
    {
        var level = PlayerPrefs.GetInt(powerUpManager.DoublePointsLevelName());

        if (level >= powerUpManager.GetNrOfPowerUps())   // maxed out
            return;
        else if (coinManager.GetCoins() < doublePointsPrices[level + 1])   // too pricy
        {
            StartCoroutine(shopManager.SpawnNotEnoughCoinsPowerUps());
            return;
        }
        else
        {
            coinManager.DecreaseCoins(doublePointsPrices[level + 1]);
            PlayerPrefs.SetInt(powerUpManager.DoublePointsLevelName(), level + 1);
            UpdateInfo();
        }
    }
    public void UpgradeMoneyRain()
    {
        var level = PlayerPrefs.GetInt(powerUpManager.MoneyRainLevelName());

        if (level >= powerUpManager.GetNrOfPowerUps())   // maxed out
            return;
        else if (coinManager.GetCoins() < moneyRainPrices[level + 1])   // too pricy
        {
            StartCoroutine(shopManager.SpawnNotEnoughCoinsPowerUps());
            return;
        }
        else
        {
            coinManager.DecreaseCoins(moneyRainPrices[level + 1]);
            PlayerPrefs.SetInt(powerUpManager.MoneyRainLevelName(), level + 1);
            UpdateInfo();
        }
    }
    public void UpgradeDeflection()
    {
        var level = PlayerPrefs.GetInt(powerUpManager.DeflectionLevelName());

        if (level >= powerUpManager.GetNrOfPowerUps())   // maxed out
            return;
        else if (coinManager.GetCoins() < deflectionPrices[level + 1])   // too pricy
        {
            StartCoroutine(shopManager.SpawnNotEnoughCoinsPowerUps());
            return;
        }
        else
        {
            coinManager.DecreaseCoins(deflectionPrices[level + 1]);
            PlayerPrefs.SetInt(powerUpManager.DeflectionLevelName(), level + 1);
            UpdateInfo();
        }
    }
}
