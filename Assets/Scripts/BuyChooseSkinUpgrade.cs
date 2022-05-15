using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyChooseSkinUpgrade : MonoBehaviour
{
    [Header("Required variables")]
    [SerializeField] private int skinColorNumber = 11;
    [SerializeField] private int upgradeColorNumber = 3;

    [Header("Skin data")]
    [SerializeField] private int[] skinPrices = { };
    [SerializeField] private float movingSkinCost = 0.5f;   // fraction of given skins' cost

    [Header("Upgrade data")]
    [SerializeField] private int[] upgradePrices = { };

    [Header("Required components")]
    [SerializeField] private ScrollSnapRect skinScroller;
    [SerializeField] private ScrollSnapRect skinColorScroller;
    [SerializeField] private ScrollSnapRect upgradeScroller;
    [SerializeField] private ScrollSnapRect upgradeColorScroller;

    [Header("Other working scripts")]
    [SerializeField] private CoinManager coinManager;
    [SerializeField] private PlayerController player;
    [SerializeField] private ShopManager shopManager;

    /***/

    public void BuyChooseSkin()
    {
        // Determining which skin is player choosing
        int which = CurrentSkin();

        // The name depends on the possible movement of the skin
        string name = player.SkinName(which);

        // If skin owned choosing, otherwise buying
        int isBought = PlayerPrefs.GetInt(name, 0);
        if (isBought == 0)
        {
            BuySkin(which, name);
        }
        else if (isBought == 1)
        {
            ChooseSkin(which);
        }
    }

    public void BuyChooseUpgrade()
    {
        int which = CurrentUpgrade();
        int isBought = PlayerPrefs.GetInt(player.UpgradeName(which), 0);
        if (isBought == 0)
        {
            BuyUpgrade(which);
        }
        else if (isBought == 1)
        {
            ChooseUpgrade(which);
        }
    }


    private void BuySkin(int which, string name)
    {
        // Price depends on skin movement
        int price = GivenSkinPrice(which);

        // Checking whether player has enough resources
        if (coinManager.GetCoins() < price) { StartCoroutine(shopManager.SpawnNotEnoughCoinsPlayerCustomisation()); return; }

        // Buying the skin (converting the price to int to avoid fractions / float above since we are multiplying by fraction)
        coinManager.DecreaseCoins(price);
        PlayerPrefs.SetInt(name, 1);
        ChooseSkin(which);
    }

    private void ChooseSkin(int which)
    {
        // Changing the skin & indicating it in player prefs
        PlayerPrefs.SetInt("CurrentSkin", which);
        player.ChangeSkin(which);

        // If moving - saving the info to player prefs
        player.AdjustMovingSkinPrefs();
    }


    private void BuyUpgrade(int which)
    {
        if (coinManager.GetCoins() < upgradePrices[which]) { StartCoroutine(shopManager.SpawnNotEnoughCoinsPlayerCustomisation()); return; }

        PlayerPrefs.SetInt(player.UpgradeName(which), 1);
        coinManager.DecreaseCoins(upgradePrices[which]);
        ChooseUpgrade(which);
    }

    private void ChooseUpgrade(int which)
    {
        PlayerPrefs.SetInt("CurrentUpgrade", which);
        player.ChangeUpgrade(which);
    }


    // Displays the change during shopping to see the actual effect
    public void ChangeSkin() { player.ChangeSkin(CurrentSkin()); }
    public void ChangeUpgrade() { player.ChangeUpgrade(CurrentUpgrade()); }
    public IEnumerator ChangeWhenOpeningShopCo() { yield return new WaitUntil(() => skinScroller.isActiveAndEnabled == true); ChangeSkin(); ChangeUpgrade(); }
    
    // Returns currently chosen skin/upgrade
    private int CurrentSkin()
    {
        return skinScroller.GetNearestPage() * skinColorNumber + skinColorScroller.GetNearestPage();
    }
    private int CurrentUpgrade()
    {
        if (upgradeScroller.GetNearestPage() == 0) return 0;
        else
            return upgradeScroller.GetNearestPage() * upgradeColorNumber + upgradeColorScroller.GetNearestPage() - (upgradeColorNumber - 1);
    }

    public int CurrentSkinPrice() { return GivenSkinPrice(CurrentSkin()); }
    public int CurrentUpgradePrice() { return upgradePrices[CurrentUpgrade()]; }
    
    public int GivenSkinPrice(int which)
    {
        float price;
        if (player.IsSkinMoving())
            price = skinPrices[which] + skinPrices[which] * movingSkinCost;
        else
            price = skinPrices[which];

        return (int)price;
    }

    public bool IsSkinBought() { if (PlayerPrefs.GetInt(player.SkinName(CurrentSkin())) == 1) return true; else return false; }
    public bool IsUpgradeBought() { if (PlayerPrefs.GetInt(player.UpgradeName(CurrentUpgrade())) == 1) return true; else return false; }
}
