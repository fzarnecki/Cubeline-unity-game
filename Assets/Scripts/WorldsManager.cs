using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldsManager : MonoBehaviour
{
    // Initial positions of each line to define 'reachpoints'
    private Vector3[] initialPositions;
    
    [Tooltip("All lines on which player plays")]
    public GameObject[] worlds;
    private int nrOfPartsInLine = 4;

    [Tooltip("Seconds to wait before another world change")]
    [SerializeField] private float waitBeforeChange = 0.1f;
    [Tooltip("Duration of world change animation")]
    [SerializeField] private float changeAnimationDuration = 0.75f;
    // Controlling changes of the lines, not to override
    private bool canChange = true;
    private bool jumpFinished = true;

    [Tooltip("Player customisation line game object")]
    [SerializeField] private GameObject customLine;

    [Header("Other working scripts")]
    [SerializeField] private PlayerController player;
    [SerializeField] private SFXPlayer sfxPlayer;
    [SerializeField] private PowerUpManager powerUpManager;
    [SerializeField] private GameManager gameManager;

    /***/

    void Start()
    {
        // Caching initial positions of the lines(worlds)
        initialPositions = new Vector3[worlds.Length];
        for (int i = 0; i < worlds.Length; i++)
            initialPositions[i] = worlds[i].transform.position;
    }

    public void ExchangeWorlds()    // on touch
    {
        if (CanChange() && IsJumpFinished())
        {
            StartCoroutine(ExchangeWorldsCo());
            GameManager.ChangeWorld();
        }
    }

    public IEnumerator ExchangeWorldsCo()
    {
        SetCanChange(false);
        player.DisableJump();

        // Disabling & Enabling player collision
        StartCoroutine(ResetPlayerCollisionCo());

        GameManager.SlowDownMovement();
        TriggerWorldChangeAnimations();

        // Trigerring change sfx
        sfxPlayer.PlayChangeSFX();

        yield return new WaitForSeconds(changeAnimationDuration);
        //yield return new WaitUntil(() => !IsChangeAnimationPlaying());

        // Changing worlds positions
        worlds[0].transform.position = initialPositions[2];
        worlds[1].transform.position = initialPositions[0];
        worlds[2].transform.position = initialPositions[1];

        // Updating 'worlds' array
        for (int i = 0; i < worlds.Length - 1; i++)
            LineMover.Swap(worlds, i, i + 1);
        
        if (!gameManager.GameEnded())
            GameManager.SpeedUpMovement();

        // Enabling jump
        player.EnableJump();
        // Waiting given nr of seconds before can change world again
        yield return new WaitForSeconds(waitBeforeChange);
        SetCanChange(true);
    }

    public IEnumerator ResetPlayerCollisionCo()
    {
        player.SetColliding(false);
        yield return new WaitForSeconds(changeAnimationDuration / 2);
        player.SetColliding(true);
    }

    private void TriggerWorldChangeAnimations()
    {
        // Trigerring player animation
        player.GetComponent<Animator>().SetTrigger("WorldChange");
        player.DisableJump();
        
        // Trigerring worlds animations
        worlds[0].GetComponent<Animator>().SetTrigger("FirstToLast");
        for (int i = 1; i < worlds.Length; i++)
            worlds[i].GetComponent<Animator>().SetTrigger("Regular");
    }

    public void TriggerRespawnAnimations()
    {
        // Trigerring player animation
        player.GetComponent<Animator>().SetTrigger("Respawn");
        player.DisableJump();

        // Trigerring worlds animations
        for (int i = 0; i < worlds.Length; i++)
            worlds[i].GetComponent<Animator>().SetTrigger("Respawn");
    }

    private bool CanChange() { return canChange; }
    public void SetCanChange(bool b) { canChange = b; }
    private bool IsJumpFinished() { return jumpFinished; }
    public void SetJumpFinished(bool b) { jumpFinished = b; }
    
    public void ClearChosenLine(int line)
    {
        ObstacleGenerator[] children = worlds[line].GetComponentsInChildren<ObstacleGenerator>();
        for (int i = 0; i < nrOfPartsInLine; i++)
        {
            children[i].ClearLine();
        }
    }

    public IEnumerator ClearFirstObstacle()
    {
        yield return 0;
        for (int i = GameManager.nrOfWorlds - 1; i >= 0; i--)
        {
            ObstacleGenerator[] children = worlds[i].GetComponentsInChildren<ObstacleGenerator>();
            children[0].ClearLine();
            children[1].ClearLine();
        }
    }
    
    public void ChangeLineForCustomisation() { foreach (GameObject g in worlds) { g.active = false; } customLine.active = true; }
    public void ChangeLineAfterCustomisation() { customLine.active = false; foreach (GameObject g in worlds) { g.active = true; } }

    /***/

    /*public IEnumerator ClearEverything()
    {
        yield return 0;

        int limit = 4;
        for (int i = 0; i < GameManager.nrOfWorlds; i++)
        {
            ObstacleGenerator[] children = worlds[i].GetComponentsInChildren<ObstacleGenerator>();
            for (int j = 0; j < limit; j++)
            {
                children[j].ClearLine();
            }
        }
    }*/

    //private void TurnOffWorldsComponents() { foreach (GameObject w in worlds) { w.GetComponent<LineMover>().TurnOffComponents(); } }
    //private void TurnOnWorldsComponents() { foreach (GameObject w in worlds) { w.GetComponent<LineMover>().TurnOnComponents(); } }
}
