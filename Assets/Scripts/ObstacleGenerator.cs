using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    [Tooltip("Collider of the line")]
    [SerializeField] BoxCollider boxCollider;

    [Header("Distances from line edges where cannot spawn")]
    [SerializeField] float minDistFromLeft = 2f;
    [SerializeField] float minDistFromRight = 2f;

    [Header("Y coordinate adjusts")]
    [SerializeField] float[] yAdjusts;

    [Header("Obstacles to choose from")]
    [SerializeField] GameObject[] obstacles;

    [Header("Other working scripts")]
    [SerializeField] BuildManager buildManager;
    [SerializeField] CoinManager coinManager;

    // Adjusts for coin placement
    float yCoinAdj = 1f;
    float xCoinAdj = 2.5f;
    // Keeping x pos of last spawned obstacle (used e.g. in powerUp spawning)
    float lastXPos;


    /***/

    void Start()
    {
        Recreate();
    }

    public void Recreate()
    {
        DestroyChildren();
        SpawnObstacle();
    }

    public void ClearLine()
    {
        DestroyChildren();
    }

    private void DestroyChildren()
    {
        int childCount = transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    private void SpawnObstacle()    // Spawning only one obstacle due to line being short
    {
        // Clamping generated position to fit in the wanted space (in case to big)
        float xPos = Mathf.Clamp(boxCollider.bounds.min.x + buildManager.GetNextXPos(), 
                                    boxCollider.bounds.min.x + minDistFromLeft, 
                                    boxCollider.bounds.max.x - minDistFromRight);
        lastXPos = xPos;


        if (!buildManager.IsSpawnObstacle()) return;


        // Setting prefab to random obstacle from the array
        GameObject obstaclePrefab = obstacles[buildManager.GetNextObstacle()];

        // Instantiating at line position, but moved to chosen xPos + setting it up as a child to this game object
        float yAdj = yAdjusts[buildManager.GetNextObstacle()];
        GameObject newObstacle = Instantiate(obstaclePrefab,
                                                new Vector3(xPos, transform.position.y + yAdj, transform.position.z),
                                                Quaternion.identity,
                                                transform);

        // Spawning coin
        GameObject newCoin = Instantiate(coinManager.GetCoinPrefab(),
                                         new Vector3(xPos + xCoinAdj, transform.position.y + yCoinAdj, transform.position.z),
                                         Quaternion.identity,
                                         transform);
    }

    public void SpawnCoinsOnWholeLine(float gap)
    {
        float pos = boxCollider.bounds.min.x;
        while (pos < boxCollider.bounds.max.x)
        {
            GameObject c = Instantiate(coinManager.GetCoinPrefab(),
                                       new Vector3(pos, transform.position.y + yCoinAdj, transform.position.z),
                                       Quaternion.identity,
                                       transform);
            c.GetComponent<Animator>().SetTrigger("Spawn");
            pos += gap;
        }
    }

    public BoxCollider GetBoxCollider() { return boxCollider; }
    public float GetLastXPos() { return lastXPos; }
}
