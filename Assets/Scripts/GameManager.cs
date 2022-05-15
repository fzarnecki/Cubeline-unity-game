using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour
{
    [Header("Game stuff")]
    private int freeRespawn;
    private string freeRespawnPrefsName = "freeRespawn";
    private bool gameEnded;
    public static float respawnDuration = 3f;
    private int respawnTracker = 0;
    public float fadeSpeed = 5f;
    private bool isPause = false;
    private bool firstTime = false; // if its 1st time => delays start, used in StartGame

    // When changing world, incrementing the 'currentWorld' to indicate change and then getting mod from number of worlds
    // Therefore -> 0 - first world, 1 - second world, 2 - third world
    public static int nrOfWorlds = 3;
    public static int currentWorld = 0;

    [Header("Headstart stuff")]
    private int headstart;
    private string headstartPrefsName = "headstart";
    private bool isHeadstart = false;
    private RigidbodyConstraints originalConstraints;
    [SerializeField] private GameObject headstartButton;
    [SerializeField] private float headstartDisplayTime = 7f;
    [SerializeField] private float headstartDuration = 3f;
    [SerializeField] private float headstartSpeed = 200f;
    [SerializeField] private int headstartPoints = 1000;
    [SerializeField] private float headstartButtonDespawnDuration = 1f;
    
    [Header("Music stuff")]
    public AudioSource musicPlayer;
    private float musicVolumeLower = .25f;
    private float musicVolumeHigher = .9f;
    [SerializeField] SFXPlayer sfxPlayer;

    [Header("Canvas")]
    [SerializeField] GameObject menuCanvas;
    [SerializeField] GameObject gameCanvas;
    [SerializeField] GameObject loseCanvas;
    [SerializeField] GameObject pauseCanvas;
    [SerializeField] GameObject shopCanvas;
    [SerializeField] GameObject playerCustomisationCanvas;
    [SerializeField] GameObject usablesCanvas;
    [SerializeField] GameObject powerUpCanvas;
    [SerializeField] GameObject worldCanvas;
    private GameObject currentCanvas;
    private bool isAction = false;

    [Tooltip("Text displayed with ad respawn option")]
    [SerializeField] private GameObject respawnText;
    [Tooltip("Button displayed with ad respawn option")]
    [SerializeField] private GameObject respawnButton;
    [Tooltip("Button displayed when player has respawn purchased")]
    [SerializeField] private GameObject freeRespawnButton;
    [Tooltip("Text displayed when game is over over (no more respawns)")]
    [SerializeField] private GameObject goodluckText;
    
    [Header("Other working scripts")]
    [SerializeField] PlayerController player;
    [SerializeField] WorldsManager worldsManager;
    [SerializeField] SceneFader sceneFader;
    [SerializeField] TimeManager timeManager;
    [SerializeField] ToggleImageChanger toggleManager;
    [SerializeField] BuyChooseSkinUpgrade skinUpgradeManager;
    [SerializeField] PowerUpManager powerUpManager;
    [SerializeField] IntroGuide introGuide;

    /***/

    private void Start()
    {
        SetResolution();
        SetHeadstart(false);
        DisableMovement();
        PointsManager.ResetPoints();
        SetGameEnded(false);
        GameManager.currentWorld = 0;
        LoadFreeRespawns();
        LoadHeadstarts();
        PrepareCanvas();
        MuteMusic();
        StartCoroutine(BringVolumeUp(musicVolumeLower));
        StartCoroutine(worldsManager.ClearFirstObstacle());
        //StartCoroutine(sceneFader.FadeCanvasIn(menuCanvas, fadeSpeed));
    }

    private void SetResolution() { Screen.SetResolution(720, 1280, true); }

    public void StartGame() { StartCoroutine(StartGameCo()); }  // button
    private IEnumerator StartGameCo() // coroutine to enable wait for 1st play check
    {
        SetFirstTime(true); // set to true to enable check in introGuide
        introGuide.CheckIfFirstPlay();  // will change firstTime to false if its whether not 1st time or intro at 1st time has finished

        yield return new WaitUntil(() => !IsFirstTime());   // requires firstTime to be false (cannot start if intro is being displayed)

        StartCoroutine(DisplayHeadstart()); // Initial headstart button
        player.EnableCollision();
        player.StartPlayer();   // Going to idle state
        StartCoroutine(FadeMenuAndStart()); // Changing canvas & starting movement
        StartCoroutine(BringVolumeUp(musicVolumeHigher));
    }

    public void Pause() // button
    {
        if (isAction) return;   // To prevent multiple canvas actions at once - could be really unpleasant

        SetPause(true); // Indicating game pause
        StartCoroutine(sceneFader.FadeUnfadeCo(gameCanvas, pauseCanvas, fadeSpeed));  // Changing canvas
        DisableMovement();
        StartCoroutine(TurnVolumeDown(musicVolumeLower));
    }

    public void Resume() // button
    {
        if (isAction) return;

        SetPause(false);
        StartCoroutine(sceneFader.FadeUnfadeCo(pauseCanvas, gameCanvas, fadeSpeed));
        StartCoroutine(EnableMovementWithDelay(0.3f));  // Delay to let canvas fade out
        StartCoroutine(BringVolumeUp(musicVolumeHigher));
    }

    public void RespawnOrRetry() // button
    {
        StartCoroutine(StopAndFadeLoseScreen());
    }

    public void Respawn() // button
    {
        if (isAction) return;

        // In case of watched ad ;)
        StartCoroutine(FadeLoseAndRespawn());
        IndicateRespawn();
    }

    public void FreeRespawn() // button
    {
        if (isAction) return;

        StartCoroutine(FadeLoseAndRespawn());
        DecreaseFreeRespawn();
        // IndicateRespawn(); <- freeRespawn not gonna be counted to the limiting
    }

    public void Retry() // button
    {
        if (isAction) return;

        StartCoroutine(sceneFader.FadeOutAndReloadCo());
        StartCoroutine(sceneFader.FadeCanvasOutCo(currentCanvas, fadeSpeed));
        StartCoroutine(TurnVolumeDown(0f));
    }

    public void OpenShop() // button
    {
        if (isAction) return;
        StartCoroutine(sceneFader.FadeUnfadeCo(currentCanvas, shopCanvas, fadeSpeed));
    }
    public void ReturnFromShop() // button
    {
        if (isAction) return;
        StartCoroutine(sceneFader.FadeUnfadeCo(shopCanvas, pauseCanvas, fadeSpeed));
    }

    public void OpenUsables() // button
    {
        if (isAction) return;
        StartCoroutine(sceneFader.FadeUnfadeCo(currentCanvas, usablesCanvas, fadeSpeed));
    }
    public void ReturnFromUsables() // button
    {
        if (isAction) return;
        StartCoroutine(sceneFader.FadeUnfadeCo(currentCanvas, shopCanvas, fadeSpeed));
    }

    public void OpenPowerUps() // button
    {
        if (isAction) return;
        StartCoroutine(sceneFader.FadeUnfadeCo(currentCanvas, powerUpCanvas, fadeSpeed));
    }
    public void ReturnFromPowerUps() // button
    {
        if (isAction) return;
        StartCoroutine(sceneFader.FadeUnfadeCo(currentCanvas, shopCanvas, fadeSpeed));
    }

    public void OpenPlayerCustomisation() // button
    {
        if (isAction) return;
        player.TriggerIdleState();
        toggleManager.TurnToggleOff();  // off so that first animation is idle
        powerUpManager.SetPowerUpEffectsActive(false);
        worldsManager.ChangeLineForCustomisation();
        StartCoroutine(skinUpgradeManager.ChangeWhenOpeningShopCo());
        StartCoroutine(sceneFader.FadeUnfadeCo(shopCanvas, playerCustomisationCanvas, fadeSpeed));
    }
    public void ReturnFromPlayerCustomisation() // button
    {
        if (isAction) return;
        player.TriggerIdleState();  // going back to idle state not to mess up game
        worldsManager.ChangeLineAfterCustomisation();
        powerUpManager.SetPowerUpEffectsActive(true);
        player.ApplySkinAndUpgrade();
        StartCoroutine(sceneFader.FadeUnfadeCo(playerCustomisationCanvas, shopCanvas, fadeSpeed));
    }

    public void QuitGame() // button
    {
        if (isAction) return;

        Application.Quit();
    }

    private IEnumerator StopAndFadeLoseScreen()
    {
        //SetAction(true);
        sfxPlayer.PlayFailSFX();
        gameEnded = true;
        DisableMovement();
        
        gameCanvas.SetActive(false);
        loseCanvas.SetActive(true);
        SetCurrentCanvas(loseCanvas);

        // Turning on appropriate button, depending on nr of free respawns
        if (respawnTracker >= 2)
        {
            respawnText.SetActive(false);
            respawnButton.SetActive(false);
            freeRespawnButton.SetActive(false);

            goodluckText.SetActive(true);
        }
        else
        {
            goodluckText.SetActive(false);
            freeRespawnButton.SetActive(false);
            respawnText.SetActive(true);
            respawnButton.SetActive(true);

            /*if (GetFreeRespawns() > 0)    Not using free respawns anymore
            {
                goodluckText.SetActive(false);
                respawnText.SetActive(false);
                respawnButton.SetActive(false);
                freeRespawnButton.SetActive(true);
            }
            else
            {
                goodluckText.SetActive(false);
                freeRespawnButton.SetActive(false);
                respawnText.SetActive(true);
                respawnButton.SetActive(true);
            }*/
        }

        // Setting lose canvas alpha to 0 to fade in slowly
        CanvasGroup group = loseCanvas.GetComponent<CanvasGroup>();
        group.alpha = 0;
        // Fading in lose canvas
        while (group.alpha < 1)
        {
            group.alpha += Time.deltaTime * fadeSpeed;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        StartCoroutine(TurnVolumeDown(musicVolumeLower));
        StartCoroutine(FindObjectOfType<DisplayPoints>().DisplayPointsCountFromZero());
    }

    private IEnumerator FadeMenuAndStart()
    {
        SetAction(true);
        // Fading out menu canvas
        if (menuCanvas.active)  // can be already disabled if intro guide was displayed
        {
            CanvasGroup group = menuCanvas.GetComponent<CanvasGroup>();
            while (group.alpha > 0)
            {
                group.alpha -= Time.deltaTime * fadeSpeed;
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }
        
        menuCanvas.SetActive(false);
        gameCanvas.SetActive(true);
        SetCurrentCanvas(gameCanvas);

        // Fading in game canvas
        CanvasGroup group2 = gameCanvas.GetComponent<CanvasGroup>();
        group2.alpha = 0;
        while (group2.alpha < 1)
        {
            group2.alpha += Time.deltaTime * fadeSpeed;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        // Reseting global stuff
        SetGameEnded(false);
        GameManager.currentWorld = 0;

        EnableMovement();   //line
        SetAction(false);
    }

    private IEnumerator FadeLoseAndRespawn()
    {
        worldsManager.SetCanChange(false);

        // Fading out lose canvas
        CanvasGroup group = loseCanvas.GetComponent<CanvasGroup>();
        while (group.alpha > 0)
        {
            group.alpha -= Time.deltaTime * fadeSpeed;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        
        loseCanvas.SetActive(false);
        
        TriggerRespawnAnimations();
        sfxPlayer.PlayRespawnSFX();
        yield return new WaitForSeconds(GameManager.respawnDuration * 0.66f);
        ClearAllLines();
        yield return new WaitForSeconds(GameManager.respawnDuration * 0.34f);
        
        gameCanvas.SetActive(true);
        SetCurrentCanvas(gameCanvas);

        gameEnded = false;
        EnableMovement();   //line
        GameManager.SpeedUpMovement();
        player.EnableJump();
        worldsManager.SetCanChange(true);
        StartCoroutine(BringVolumeUp(musicVolumeHigher));
    }

    private void TriggerRespawnAnimations()
    {
        worldsManager.TriggerRespawnAnimations();
    }

    private void ClearAllLines()
    {
        for (int i=0; i<GameManager.nrOfWorlds; i++)
            worldsManager.ClearChosenLine(i);
    }

    public static void DisableMovement()
    {
        LineMover[] mvrs = FindObjectsOfType<LineMover>();
        foreach (LineMover l in mvrs)
            l.DisableMovement();
        
        PointsManager.DisableEarning();
    }

    public static void EnableMovement()
    {
        LineMover[] mvrs = FindObjectsOfType<LineMover>();
        foreach (LineMover l in mvrs)
            l.EnableMovement();

        PointsManager.EnableEarning();
    }

    private IEnumerator EnableMovementWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        EnableMovement();
    }

    // Ratio added in slow down must be a reverse of the one used in the speed up method
    public static void SlowDownMovement()
    {
        LineMover[] mvrs = FindObjectsOfType<LineMover>();
        foreach (LineMover l in mvrs)
            l.ChangeMoveSpeed(l.GetInitMoveSpeed() / 10);

        PointsManager.DisableEarning();
    }
    public static void SpeedUpMovement()
    {
        LineMover[] mvrs = FindObjectsOfType<LineMover>();
        foreach (LineMover l in mvrs)
            l.ChangeMoveSpeed(l.GetInitMoveSpeed());

        PointsManager.EnableEarning();
    }

    public IEnumerator ChangeMoveSpeedCo(float speed, bool headstart, bool immediateSpeedUp, float changeRate)
    {
        LineMover[] mvrs = FindObjectsOfType<LineMover>();
        
        if (immediateSpeedUp)
        {
            foreach (LineMover l in mvrs)
                l.moveSpeed = speed;

            yield break;
        }

        if (speed != 1 && mvrs[0].GetMoveSpeed() < speed)
        {
            while (mvrs[0].GetMoveSpeed() < speed)
            {
                foreach (LineMover l in mvrs)
                    l.moveSpeed += changeRate;
                yield return new WaitForSeconds(Time.deltaTime);

                if (!headstart && IsHeadstart()) yield break;
            }
        }
        else if (speed == 1)
        {
            while (mvrs[0].GetMoveSpeed() > mvrs[0].GetInitMoveSpeed())
            {
                foreach (LineMover l in mvrs)
                    l.moveSpeed -= changeRate;
                yield return new WaitForSeconds(Time.deltaTime);

                if (!headstart && IsHeadstart()) yield break;
            }
            foreach (LineMover l in mvrs)
                l.ChangeMoveSpeed(l.GetInitMoveSpeed());
        }
    }
    
    private IEnumerator TurnVolumeDown(float targetVolume)
    {
        while (musicPlayer.volume > targetVolume)
        {
            musicPlayer.volume -= 0.02f;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    private IEnumerator BringVolumeUp(float targetVolume)
    {
        while (musicPlayer.volume < targetVolume)
        {
            musicPlayer.volume += 0.02f;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    private void MuteMusic() { musicPlayer.volume = 0; }

    private void IndicateRespawn() { respawnTracker++; }

    private void LoadFreeRespawns() { freeRespawn = PlayerPrefs.GetInt(freeRespawnPrefsName, 0); }

    public void IncreaseFreeRespawn()
    {
        freeRespawn++;
        PlayerPrefs.SetInt(freeRespawnPrefsName, freeRespawn);
    }
    public void DecreaseFreeRespawn()
    {
        freeRespawn--;
        PlayerPrefs.SetInt(freeRespawnPrefsName, freeRespawn);
    }

    public int GetFreeRespawns() { return freeRespawn; }

    private void LoadHeadstarts() { headstart = PlayerPrefs.GetInt(headstartPrefsName, 0); }

    public void IncreaseHeadstart()
    {
        headstart++;
        PlayerPrefs.SetInt(headstartPrefsName, headstart);
    }
    public void DecreaseHeadstart()
    {
        headstart--;
        PlayerPrefs.SetInt(headstartPrefsName, headstart);
    }

    public int GetHeadstarts() { return headstart; }

    private IEnumerator DisplayHeadstart()
    {
        if (headstart < 1)
        {
            headstartButton.SetActive(false);
            yield break;
        }
        else
        {
            headstartButton.SetActive(true);
            yield return new WaitForSeconds(headstartDisplayTime);
            if (!isHeadstart)
            {
                headstartButton.GetComponent<Animator>().SetTrigger("Despawn");
                yield return new WaitForSeconds(headstartButtonDespawnDuration);
                headstartButton.SetActive(false);
            }
        }
    }

    private void SetHeadstart(bool b) { isHeadstart = b; }

    public void StartHeadstart() { if (headstart > 0) { DecreaseHeadstart(); StartCoroutine(Headstart()); } }
    public IEnumerator Headstart()
    {
        // Indicating headstart occurence
        SetHeadstart(true);
        // Trigerring players animation
        player.GetAnimator().SetTrigger("Headstart");
        // Disabling touch controls
        gameCanvas.SetActive(false);
        // Changing player state to prevent unwanted events (collision, animation interuption)
        player.DisableJump();
        player.SetCollisionWithObstacles(false);
        worldsManager.SetCanChange(false);
        // Trigerring quick point gain
        StartCoroutine(PointsManager.CrazyEarningCo(headstartPoints));

        // Wait resulting from animation look
        yield return new WaitForSeconds(1f);

        // Speed up
        StartCoroutine(ChangeMoveSpeedCo(headstartSpeed, true, true, 0f));

        // Waiting for the animation
        yield return new WaitForSeconds(headstartDuration);

        // Disabing now needless button
        headstartButton.SetActive(false);
        // Slow down
        StartCoroutine(ChangeMoveSpeedCo(1, true, false, 3f));
        // Indicating headstart has ended
        SetHeadstart(false);
        
        // Waiting for speed to be back to initial one
        LineMover l = FindObjectOfType<LineMover>();
        yield return new WaitUntil(() => l.GetMoveSpeed() == l.GetInitMoveSpeed());

        // Setting player state back to normal (changed from triggers in animation)
        player.EnableCollisionWithObstaclesWithDelay();
        player.EnableJump();
        worldsManager.SetCanChange(true);

        // Enabling player touch controls
        gameCanvas.SetActive(true);
    }

    private void PrepareCanvas()
    {
        worldCanvas.SetActive(true);
        menuCanvas.SetActive(true);
        gameCanvas.SetActive(false);
        loseCanvas.SetActive(false);
        pauseCanvas.SetActive(false);
        shopCanvas.SetActive(false);
        playerCustomisationCanvas.SetActive(false);
        powerUpCanvas.SetActive(false);
        usablesCanvas.SetActive(false);
    }

    public void SetCurrentCanvas(GameObject c) { currentCanvas = c; }
    public GameObject GetCurrentCanvas() { return currentCanvas; }

    public bool IsHeadstart() { return isHeadstart; }
    public bool IsHeadstartButtonActive() { return headstartButton.active; }
    public void SetAction(bool b) { isAction = b; }
    public bool IsAction() { return isAction; }

    public void SetPause(bool b) { isPause = b; }
    public bool GetPause() { return isPause; }

    public void SetGameEnded(bool b) { gameEnded = b; }
    public bool GameEnded() { return gameEnded; }

    public bool IsFirstTime() { return firstTime; }
    public void SetFirstTime(bool b) { firstTime = b; }

    public void SetMenuCanvasActive(bool b) { menuCanvas.SetActive(b); }


    // Method used in WorldsManager to indicate change
    public static void ChangeWorld()
    {
        GameManager.currentWorld++;
        GameManager.currentWorld %= GameManager.nrOfWorlds;
    }
}
