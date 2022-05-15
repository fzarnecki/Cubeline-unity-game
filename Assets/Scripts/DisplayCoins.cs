using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayCoins : MonoBehaviour
{
    [SerializeField] private CoinManager coinManager;   
    [SerializeField] private Text coinText;

    /***/
    
    // No real need for optimizing with coroutine since its only when game is not actively running (pause)
    void Update()
    {
        coinText.text = coinManager.GetCoins().ToString();
    }
}
