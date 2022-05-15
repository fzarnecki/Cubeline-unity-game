using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineMover : MonoBehaviour
{
    [Header("Line components of the road + their positioners")]
    [SerializeField] private GameObject[] lines;
    [SerializeField] private GameObject[] lineParents;
    [SerializeField] private GameObject[] positioners;

    [Tooltip("Speed of line changing x position")]
    public float moveSpeed = 9f;
    private float initialMoveSpeed;

    // Controlling bool
    private bool move = true;

    [Header("Other working scripts")]
    [SerializeField] BuildManager buildManager;
    [SerializeField] PowerUpManager powerUpManager;

    /***/

    void Awake()
    {
        initialMoveSpeed = moveSpeed;
    }

    void Update()
    {
        if (move)
        {
            MoveLines();
            CheckAndChangeOrder();
        }
    }

    private void MoveLines()
    {
        for (int i = 0; i < positioners.Length; i++)
            positioners[i].transform.Translate(-Time.deltaTime * moveSpeed, 0, 0);
    }

    private void CheckAndChangeOrder()
    {
        if (lines[0].transform.localScale.x + positioners[0].transform.position.x < 0f)
        {
            // Moving 1st line to last pos
            Vector3 newPos = positioners[positioners.Length - 1].transform.position;
            newPos.x += lines[0].transform.localScale.x;
            positioners[0].transform.position = newPos;

            // Shifting all positioners to the left (first element is last one)
            for (int i = 0; i < positioners.Length - 1; i++)
                Swap(positioners, i, i + 1);
            //Shifting all line parents to the left
            for (int i = 0; i < lineParents.Length - 1; i++)
                Swap(lineParents, i, i + 1);
            // Shifting all lines to the left (first element is last one) -> to enable generating obstacles
            for (int i = 0; i < lines.Length - 1; i++)
                Swap(lines, i, i + 1);
             
            // Generating position for next obstacle
            buildManager.GenerateNextObstaclePosition();
            // Choosing next obstacle
            buildManager.ChooseNextObstacle();
            // Recreating last part of the line
            var lastLineGen = lines[lines.Length - 1].GetComponentInChildren<ObstacleGenerator>();
            lastLineGen.Recreate();

            // Deleting previous power up & indicating it in manager
            int childCount = lineParents[lineParents.Length - 1].transform.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                var child = lineParents[lineParents.Length - 1].transform.GetChild(i);
                if (powerUpManager.CheckIfPowerUp(child.tag))
                    Destroy(child.gameObject);
            }
            // Some chance to spawn powerup at LineParent
            powerUpManager.SpawnRandomPowerUp(lineParents[lineParents.Length - 1].transform, lastLineGen);
        }
    }

    public static void Swap(GameObject[] arr, int pos1, int pos2)
    {
        GameObject temp = arr[pos1];
        arr[pos1] = arr[pos2];
        arr[pos2] = temp;
    }

    // Changed from multiplying to just changing to fix the bug with points when changing world & dying
    public void ChangeMoveSpeed(float ratio)
    {
        if (ratio == initialMoveSpeed)
            moveSpeed = initialMoveSpeed;
        else
            moveSpeed = initialMoveSpeed / 10;
    }

    public void DisableMovement() { move = false; }
    public void EnableMovement() { move = true; }
    public float GetInitMoveSpeed() { return initialMoveSpeed; }
    public float GetMoveSpeed() { return moveSpeed; }
    public bool IsMoving() { return move; }

    public void TurnOffComponents() { foreach (GameObject l in lineParents) { l.active = false; } }
    public void TurnOnComponents() { foreach (GameObject l in lineParents) { l.active = true; } }

    /***/

    /*private void CheckAndChangeOrderv2()
    {
        if (lines[1].transform.position.x + positioners[0].transform.position.x < 0)
        {

            // Moving 1st line to last pos
            lines[0].transform.position = positioners[lines.Length - 1].transform.position;

            // Swaping elements, so that 1st element is on last position and each another shifted 1 'to the left'
            for (int i = 0; i < lines.Length - 1; i++)
                Swap(lines, i, i + 1);
        }
    }*/
}