using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayFreeRespawns : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] Text freeRespawnText;

    /***/

    // No real need for optimizing with coroutine since its only when game is not actively running (pause)
    void Update()
    {
        freeRespawnText.text = gameManager.GetFreeRespawns().ToString();
    }
}
