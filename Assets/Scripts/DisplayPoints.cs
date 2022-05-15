using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayPoints : MonoBehaviour
{
    [Header("Text object to display the points to")]
    [SerializeField] private Text pointsText;

    private float divider = 50;

    /***/

    private void Update()
    {
        float temp = Mathf.RoundToInt(PointsManager.Points());
        pointsText.text = temp.ToString();
    }

    // Displaying the points in a form of gradual increase
    public IEnumerator DisplayPointsCountFromZero()
    {
        float tempPts = 0;
        float toAdd = PointsManager.Points() / divider;  // adding a number based on achieved points to stabilize nr of additions throughout the whole coroutine
        Text ptsText = this.GetComponent<Text>();
        SFXPlayer sfxPlayer = FindObjectOfType<SFXPlayer>();

        if (PointsManager.Points() < divider)
        {
            ptsText.text = PointsManager.Points().ToString();   // if achieved points <divider then we would be adding 0 till the end of the execution, so simply putting the number in place
        }
        else
        {
            // Actual addition loop
            while (tempPts < PointsManager.Points())
            {
                if (PointsManager.Points() - tempPts < toAdd)
                {
                    tempPts = Mathf.RoundToInt(PointsManager.Points());
                    ptsText.text = tempPts.ToString();
                    yield return new WaitForSeconds(Time.deltaTime);
                }

                tempPts += toAdd;
                tempPts = Mathf.RoundToInt(tempPts);
                ptsText.text = tempPts.ToString();
                sfxPlayer.PlayPointCountingSFX();
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }
    }
}
