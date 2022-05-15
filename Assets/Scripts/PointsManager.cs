using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsManager : MonoBehaviour
{

    public static int highscore;

    public static int points = 0;
    public static bool earn = false;

    [Header("Working scripts")]
    [SerializeField] BuildManager buildManager;
    [SerializeField] TimeManager timeManager;

    // In case of power up the multiplier may change and influence points growth
    private int multiplier = 1;


    private void Start()
    {
        highscore = PlayerPrefs.GetInt("highscore", 0);

        StartCoroutine(EarningCo());
    }


    private IEnumerator EarningCo()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            if (PointsManager.earn)
            {
                if (PointsManager.points > 50000)
                    PointsManager.points += 250 * multiplier;
                else
                    PointsManager.points += (1 + PointsManager.points / 200) * multiplier;
            }

            if (PointsManager.Points() > buildManager.GetFirstSpawnChange() && buildManager.CheckSpawning())
                buildManager.UpdateSpawning();

            if (PointsManager.Points() > timeManager.GetFirstTimeChange() && timeManager.CheckTime())
                timeManager.UpdateTime();
        }
    }

    public static void EnableEarning()
    {
        PointsManager.earn = true;
    }

    public static void DisableEarning()
    {
        PointsManager.earn = false;
    }

    public static void UpdateHighscore()
    {
        if (PointsManager.Points() > PointsManager.Highscore())
        {
            PointsManager.highscore = PointsManager.Points();
            PlayerPrefs.SetInt("highscore", PointsManager.Highscore());
        }

    }

    public static void ResetPoints()
    {
        PointsManager.points = 0;
    }

    public static IEnumerator CrazyEarningCo(int targetPoints)
    {
        while(PointsManager.Points() < targetPoints)
        {
            PointsManager.points += 1 + PointsManager.Points() / 5;
            yield return new WaitForSeconds(0.2f);
        }
    }

    public static int Points() { return PointsManager.points;}
    public static int Highscore() { return highscore; }

    public void SetMultiplier(int m) { multiplier = m; }
    public void ResetMultiplier() { multiplier = 1; }
}
