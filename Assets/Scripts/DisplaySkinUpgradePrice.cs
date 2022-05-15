using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplaySkinUpgradePrice : MonoBehaviour
{
    [Header("Define whether Skin or Upgrade")]
    [SerializeField] private bool skin;
    [SerializeField] private bool upgrade;

    [Header("Required components")]
    [SerializeField] private BuyChooseSkinUpgrade skinUpgradeManager;
    [SerializeField] TextMeshProUGUI text;


    void Update()
    {
        if (skin)
        {
            if (skinUpgradeManager.IsSkinBought())
                text.text = "Choose";
            else
                text.text = (skinUpgradeManager.CurrentSkinPrice().ToString() + " C");
        }
        else if (upgrade)
        {
            if (skinUpgradeManager.IsUpgradeBought())
                text.text = "Choose";
            else
                text.text = (skinUpgradeManager.CurrentUpgradePrice().ToString() + " C");
        }
    }
}
