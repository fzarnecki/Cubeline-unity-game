using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    [Header("Obstacle spawning stuff")]
    public float spawnSpaceLength = 8f;
    private float nextXPos;
    private bool posGenerated = false; // not to generate another immediately, but after all 3 worlds spawned obstacle
    private bool spawnObstacle = true;
    private int spaceBetweenObstacles = 3;
    private int obstacleDistanceHelper = 0;
    private int nextObstacle;
    private bool obstacleChosen = false;
    private bool checkSpawning = true;
    private int firstSpawnChange = 250;
    private int secondSpawnChange = 2000;

    [Header("Other working scripts")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private InGameNotifications inGameNotifications;

    /***/

    void Start()
    {
        obstacleDistanceHelper = 0;     // not reset, because the method sets to 1 and it would shift the initial obstacle, which we don't want at start
        SetSpaceBetweenObstacles(3);
        SetCheckSpawning(true);
    }


    public void GenerateNextObstaclePosition()
    {
        if (!IsPosGenerated())
        {
            // Determining spacing
            if (GetSpaceBetweenObstacles() == 3)
            {
                if (obstacleDistanceHelper % 2 == 1)
                    SetSpawnObstacle(true);
                else
                    SetSpawnObstacle(false);
            }
            else if (GetSpaceBetweenObstacles() == 2)
            {
                int helper = obstacleDistanceHelper % 3;
                if (helper == 0 || helper == 1)
                    SetSpawnObstacle(true);
                else
                    SetSpawnObstacle(false);
            }
            else if (GetSpaceBetweenObstacles() == 1)
            {
                SetSpawnObstacle(true);
            }
            obstacleDistanceHelper++;

            // Generating position
            nextXPos = UnityEngine.Random.Range(0, spawnSpaceLength);
            SetPosGenerated(true);
            StartCoroutine(WaitAndResetGenerator());
        }
    }

    private IEnumerator WaitAndResetGenerator()
    {
        yield return new WaitForSeconds(1f);
        SetPosGenerated(false);
    }

    public void ChooseNextObstacle()
    {
        if (!IsObstacleChosen())
        {
            nextObstacle = UnityEngine.Random.Range(0, GameManager.nrOfWorlds);
            SetObstacleChosen(true);
            StartCoroutine(WaitAndResetObstacleChooser());
        }
    }

    private IEnumerator WaitAndResetObstacleChooser()
    {
        yield return new WaitForSeconds(1f);
        SetObstacleChosen(false);
    }

    public void UpdateSpawning()
    {
        if (PointsManager.Points() > secondSpawnChange)
        {
            StartCoroutine(inGameNotifications.DisplayObstacleUpgrade());
            ResetDistanceHelper();
            SetSpaceBetweenObstacles(1);
            SetCheckSpawning(false);
        }
        else if (GetSpaceBetweenObstacles() != 2 && PointsManager.Points() > firstSpawnChange)
        {
            StartCoroutine(inGameNotifications.DisplayObstacleUpgrade());
            ResetDistanceHelper();
            SetSpaceBetweenObstacles(2);
        }
    }

    public void ResetDistanceHelper() { obstacleDistanceHelper = 1; }
    public bool CheckSpawning() { return checkSpawning; }
    private void SetCheckSpawning(bool b) { checkSpawning = b; }
    private void SetSpaceBetweenObstacles(int space) { spaceBetweenObstacles = space; }
    private int GetSpaceBetweenObstacles() { return spaceBetweenObstacles; }
    public int GetFirstSpawnChange() { return firstSpawnChange; }
    public float GetNextXPos() { return nextXPos; }
    public int GetNextObstacle() { return nextObstacle; }

    private void SetSpawnObstacle(bool b) { spawnObstacle = b; }
    public bool IsSpawnObstacle() { return spawnObstacle; }

    private void SetPosGenerated(bool b) { posGenerated = b; }
    public bool IsPosGenerated() { return posGenerated; }

    private void SetObstacleChosen(bool b) { obstacleChosen = b; }
    public bool IsObstacleChosen() { return obstacleChosen; }
}
