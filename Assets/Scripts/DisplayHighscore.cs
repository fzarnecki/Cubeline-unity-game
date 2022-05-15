using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayHighscore : MonoBehaviour
{
    [SerializeField] private Text pointsText;

    /***/

    // No real need for optimizing with coroutine since its only when game is not actively running (pause)
    void Update()
    {
        float temp;
        if (PointsManager.Highscore() > PointsManager.Points()) temp = Mathf.RoundToInt(PointsManager.highscore);
        else temp = Mathf.RoundToInt(PointsManager.Points());

        pointsText.text = temp.ToString();
    }
}
