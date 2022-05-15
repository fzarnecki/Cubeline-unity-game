using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    /* Time stuff */
    private bool checkTime = true;
    // Point milestones for changes of time speed + corresponding speeds
    private int[]      timeChanges     = { 75, 200, 600, 1200, 4000, 10000, 20000, 40000, 80000, 160000, 320000, 640000};
    private float[]    timeSpeeds      = { .75f, .8f, .85f, .9f, .95f, 1f, 1.05f, 1.1f, 1.15f, 1.2f, 1.25f, 1.3f, 1.35f };
    // Used to track current change (from the ones above) to know what to go back to when speed up (from power up)
    private float currentGeneralTimeSpeed;
    // Tracks whether the speed up is present - blocks updating the speed
    private bool isSpeedUp = false;
    // Speed at what speed up (slow down) will occur
    private float timeChangeSpeed = 0.4f;
    // Tracks whether time speed is being changed
    private bool timeIsChanging = false;

    [Header("Other scripts")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private WorldsManager worldsManager;
    [SerializeField] private PlayerController player;
    [SerializeField] private InGameNotifications inGameNotifications;

    /***/

    void Start()
    {
        Time.timeScale = timeSpeeds[0];
        currentGeneralTimeSpeed = Time.timeScale;
        SetCheckTime(true);
    }

    public void UpdateTime()
    {
        for (int i = timeChanges.Length; i >= 1; i--)
        {
            if (i == timeChanges.Length)
            {
                if (CheckAndSetTime(i)) { SetCheckTime(false); return; }
            }
            else
            {
                if (CheckAndSetTime(i)) return;
            }
        }
    }

    private bool CheckAndSetTime(int change)
    {
        if (GetTimeSpeed() != timeSpeeds[change] && PointsManager.Points() > timeChanges[change - 1])
        {
            if(!IsSpeedUp())
                SetTimeSpeed(timeSpeeds[change]);
            currentGeneralTimeSpeed = timeSpeeds[change];  // to track general time speed, e.g. for powerup purposes
            StartCoroutine(inGameNotifications.DisplaySpeedUp());
        }

        if (GetTimeSpeed() == timeSpeeds[change]) return true;
        else return false;
    }

    public void SpeedTimeUp(float speed)
    {
        SetIsSpeedUp(true);     // indicating start of the speedup/slowdown process to prevent regular time checks&changes from interrupting
        StartCoroutine(SetTimeSpeedGradually(speed));
    }
    public void TimeSpeedToNormal()
    {
        StartCoroutine(SetTimeSpeedGradually(0));
    }

    private IEnumerator SetTimeSpeedGradually(float speed)
    {
        if (IsTimeChanging())
            yield return new WaitUntil(() => !IsTimeChanging());

        speed += currentGeneralTimeSpeed;   // adding the current speed to speed bonus to get target speed

        if (Time.timeScale < speed)
        {
            SetTimeIsChanging(true);
            while (Time.timeScale < speed)
            {
                Time.timeScale += timeChangeSpeed * Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }
            Time.timeScale = speed;
            SetTimeIsChanging(false);
        }
        else
        {
            SetTimeIsChanging(true);
            while (Time.timeScale > speed)
            {
                Time.timeScale -= timeChangeSpeed * Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }
            Time.timeScale = speed;
            SetTimeIsChanging(false);
            SetIsSpeedUp(false);    // indicating the speedup/slowdown process has finished, normal time changes can be again
            UpdateTime();
        }   
    }

    public int GetFirstTimeChange() { return timeChanges[0]; }
    private void SetCheckTime(bool b) { checkTime = b; }
    public bool CheckTime() { return checkTime; }
    public float GetTimeSpeed() { return Time.timeScale; }
    private void SetTimeSpeed(float speed) { Time.timeScale = speed; }

    public bool IsSpeedUp() { return isSpeedUp; }
    public void SetIsSpeedUp(bool b) { isSpeedUp = b; }
    public bool IsTimeChanging() { return timeIsChanging; }
    public void SetTimeIsChanging(bool b) { timeIsChanging = b; }
    public bool IsTimeBackToNormal() { return Time.timeScale <= currentGeneralTimeSpeed + timeChangeSpeed; }


    public IEnumerator PauseTime(float s) { yield return new WaitForSeconds(s); Time.timeScale = 0f; }
    /***/

    /*public IEnumerator InitialSpeedUp()
    {
        player.DisableJump();
        worldsManager.DisableWorldChange();

        while (GetTimeSpeed() < initialSpeedUpSpeed)
        {
            Time.timeScale += 0.01f;
            yield return 0;
            if (gameManager.IsHeadstart()) { SetTimeSpeed(timeSpeeds[0]); yield break; }
        }

        while (GetTimeSpeed() > timeSpeeds[0])
        {
            Time.timeScale -= 0.01f;
            yield return 0;
            if (gameManager.IsHeadstart()) { SetTimeSpeed(timeSpeeds[0]); yield break; }
        }

        SetTimeSpeed(timeSpeeds[0]);

        worldsManager.EnableWorldChange();
        player.EnableJump();
    }*/
}
