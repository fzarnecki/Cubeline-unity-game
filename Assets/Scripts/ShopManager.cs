using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("Prices of given usables")]
    [SerializeField] int freeRespawnCost = 400;
    [SerializeField] int headstartCost = 600;

    [Header("Text objects to display prices to")]
    [SerializeField] private TextMeshProUGUI freeRespawnCostText;
    [SerializeField] private TextMeshProUGUI headstartCostText;

    [Header("Not Enough Coins objects to display")]
    [SerializeField] private GameObject notEnoughCoinsUsables;
    [SerializeField] private GameObject notEnoughCoinsPowerUps;
    [SerializeField] private GameObject notEnoughCoinsPlayerCustomisation;

    [Header("Other working scripts")]
    [SerializeField] GameManager gameManager;
    [SerializeField] CoinManager coinManager;

    /***/

    void Start()
    {
        freeRespawnCostText.text = freeRespawnCost.ToString();
        headstartCostText.text = headstartCost.ToString();
    }

    public void PurchaseFreeRespawn()
    {
        if (coinManager.GetCoins() < freeRespawnCost)
        {
            StartCoroutine(SpawnNotEnoughCoinsUsableCo());
            return;
        }
        else
        {
            gameManager.IncreaseFreeRespawn();
            coinManager.DecreaseCoins(freeRespawnCost);
        }
    }

    public void PurchaseHeadstart()
    {
        if (coinManager.GetCoins() < headstartCost)
        {
            StartCoroutine(SpawnNotEnoughCoinsUsableCo());
            return;
        }
        else
        {
            gameManager.IncreaseHeadstart();
            coinManager.DecreaseCoins(headstartCost);
        }
    }

    public IEnumerator SpawnNotEnoughCoinsUsableCo()
    {
        notEnoughCoinsUsables.SetActive(true);
        var animator = notEnoughCoinsUsables.GetComponent<Animator>();
        animator.SetTrigger("Entry");
        yield return new WaitUntil(() => !IsAnimationPlaying(animator));
        notEnoughCoinsUsables.SetActive(false);
    }

    public IEnumerator SpawnNotEnoughCoinsPowerUps()
    {
        notEnoughCoinsPowerUps.SetActive(true);
        var animator = notEnoughCoinsPowerUps.GetComponent<Animator>();
        animator.SetTrigger("Entry");
        yield return new WaitUntil(() => !IsAnimationPlaying(animator));
        notEnoughCoinsPowerUps.SetActive(false);
    }

    public IEnumerator SpawnNotEnoughCoinsPlayerCustomisation()
    {
        notEnoughCoinsPlayerCustomisation.SetActive(true);
        var animator = notEnoughCoinsPlayerCustomisation.GetComponent<Animator>();
        animator.SetTrigger("Entry");
        yield return new WaitUntil(() => !IsAnimationPlaying(animator));
        notEnoughCoinsPlayerCustomisation.SetActive(false);
    }

    private bool IsAnimationPlaying(Animator a) { return a.GetCurrentAnimatorStateInfo(0).length > a.GetCurrentAnimatorStateInfo(0).normalizedTime; }
}
