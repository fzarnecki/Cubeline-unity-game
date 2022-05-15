using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerUpManager : MonoBehaviour
{
    private int nrOfPowerUps = 4;

    [Header("Power up prefabs")]
    [SerializeField] private GameObject[] powerUps;

    [Header("Power up effects")]
    [SerializeField] private PowerUpEffect invincibilityEffect;
    [SerializeField] private PowerUpEffect doublePointsEffect;
    [SerializeField] private PowerUpEffect moneyRainEffect;
    [SerializeField] private PowerUpEffect deflectionEffect;

    [Header("Power ups durations (in seconds)")]
    private float[] invincibilityDurations = { 6, 7, 8, 9, 10 };
    private float[] invincibilitySpeeds = { 0.3f, 0.5f, 0.6f, 0.8f, 1f };
    private float[] doublePointsDurations = { 6, 7, 8, 9, 10 };
    private float[] moneyRainGaps = { 5, 4, 3, 2, 1 };
    private float moneyRainEffectDuration = 5;
    private float[] deflectionDurations = { 45, 90, 180, 0, 0 };        // 0 -> permanent

    [Header("Other needed scripts")]
    [SerializeField] private PlayerController player;
    [SerializeField] private BuildManager buildManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PointsManager pointsManager;
    [SerializeField] private CoinManager coinManager;
    [SerializeField] private TimeManager timeManager;

    [Header("Distance of powerup from obstacle")]
    [SerializeField] private float xPowerUpAdj = 5f;

    // Upgrades levels names
    private string invincibilityLevelName = "InvincibilityLevel";
    private string doublePointsLevelName = "DoublePointsLevel";
    private string moneyRainLevelName = "MoneyRainLevel";
    private string deflectionLevelName = "DeflectionLevel";

    // Tags for switch (need to be constant, cannot use the array)
    private const string invincibilityTag = "PowerUpInv";
    private const string doublePointsTag = "PowerUp2xP";
    private const string moneyRainTag = "PowerUpMR";
    private const string deflectionTag = "PowerUpDef";

    private string[] powerUpTags;

    // Controlling bools
    private bool isInvincibility;
    private bool isDoublePoints;
    private bool isMoneyRain;
    private bool isDeflection;

    [Header("Spawn chance stuff")]
    [SerializeField] private TextMeshProUGUI spawnChancePriceText;
    [SerializeField] private Slider spawnChanceSlider;
    private string spawnChancePrefsName = "SpawnChance";
    private int spawnChance;
    private int spawnChanceDefaultValue = 11;
    private int spawnChanceLevels = 4;
    private int[] spawnChancePrices = { 100, 1000, 3000, 10000 };
    private int coinsSpawnedBorder = 100;

    //
    private float timeLeft;

    /***/

    private void Start()
    {
        // Loading spawn chance value + updating price text & slider in shop
        LoadSpawnChance();

        // Getting powerup tags automatically to reduce string error possibility
        powerUpTags = new string[powerUps.Length];
        for (int i = 0; i < powerUps.Length; i++)
        {
            powerUpTags[i] = powerUps[i].tag;
        }

        // Turning powerUps off, in case some were left turned on
        TurnPowerUpsOff();
    }

    public void SpawnRandomPowerUp(Transform trans, ObstacleGenerator gen)
    {
        // Leaving when its not possible to spawn a powerup
        if (powerUps.Length < 1 || gameManager.IsHeadstart() || gameManager.IsHeadstartButtonActive())
            return;

        // 1 in 'spawnChance' chance to spawn a powerUp
        var r = Random.Range(0, spawnChance);
        if (r != 1) return;

        // Choosing power up
        var random = Random.Range(0, powerUps.Length);
        // If there are too many coins and moneyRain chosen again there is a need to omit this spawn, to prevent framerate drop
        if (random == 2 && !CheckIfMoneyRainPossible()) // moneyRain is as 3rd powerup => 2 used
            return;
        // Spawning
        var t = powerUps[random].transform;
        Instantiate(powerUps[random], 
                    new Vector3(gen.GetLastXPos() + xPowerUpAdj, t.position.y, gen.transform.position.z), 
                    t.rotation,
                    trans);
    }

    public void PickedUpPowerUp(string tag)
    {
        switch (tag)
        {
            case invincibilityTag:  StartCoroutine(InvincibilityCo());   break;
            case doublePointsTag:   StartCoroutine(DoublePointsCo());    break;
            case moneyRainTag:      StartCoroutine(MoneyRainCo());       break;
            case deflectionTag:     StartCoroutine(DeflectionCo());      break; 
            default: break;
        }
    }


    private IEnumerator InvincibilityCo()
    {
        int level = CurrentLevel(InvincibilityLevelName());
        var duration = invincibilityDurations[level] * (invincibilitySpeeds[level] + timeManager.GetTimeSpeed());
        float additionalSafeTime = 0.5f;

        if (GetInvincibility())
        {
            invincibilityEffect.MaxSlider(duration);
            yield break;
        }
        else if (IsPowerUp())
            yield break;

        SetInvincibility(true);
        player.SetCollisionWithObstacles(false);
        timeManager.SpeedTimeUp(invincibilitySpeeds[level]);
        invincibilityEffect.gameObject.SetActive(true);
        StartCoroutine(invincibilityEffect.ResetEffectCo(duration));

        yield return new WaitForSeconds(duration);
        yield return new WaitUntil(() => invincibilityEffect.IsSliderZero());   // waiting till slider has reached the end
        timeManager.TimeSpeedToNormal();    // trigerring time change
        yield return new WaitUntil(() => timeManager.IsTimeBackToNormal());     // not to make player collide due to too high time speed
        yield return new WaitForSeconds(additionalSafeTime);    // additonal safetime to prevent unfair collision

        player.SetCollisionWithObstacles(true);
        SetInvincibility(false);
        yield return new WaitUntil(()=> invincibilityEffect.GetParticleSystem().particleCount == 0);    // waiting for all particles to disappear before disactivating object
        invincibilityEffect.gameObject.SetActive(false);
    }

    private IEnumerator DoublePointsCo()
    {
        var level = CurrentLevel(DoublePointsLevelName());
        var duration = doublePointsDurations[level];

        if (GetDoublePoints())
        {
            doublePointsEffect.MaxSlider(duration);
            yield break;
        }
        else if (IsPowerUp())
            yield break;

        if (level == 4)
            pointsManager.SetMultiplier(3);
        else
            pointsManager.SetMultiplier(2);

        SetDoublePoints(true);
        doublePointsEffect.gameObject.SetActive(true);
        StartCoroutine(doublePointsEffect.ResetEffectCo(duration));

        yield return new WaitForSeconds(duration);
        yield return new WaitUntil(() => doublePointsEffect.IsSliderZero());

        pointsManager.ResetMultiplier();
        SetDoublePoints(false);

        yield return new WaitUntil(() => doublePointsEffect.GetParticleSystem().particleCount == 0);
        doublePointsEffect.gameObject.SetActive(false);
    }

    private IEnumerator MoneyRainCo()
    {
        var level = CurrentLevel(MoneyRainLevelName());
        
        // Spawning coins
        ObstacleGenerator[] generators = FindObjectsOfType<ObstacleGenerator>();
        foreach(ObstacleGenerator g in generators)
        {
            g.SpawnCoinsOnWholeLine(moneyRainGaps[level]);
        }

        SetMoneyRain(true);
        moneyRainEffect.gameObject.SetActive(true);
        moneyRainEffect.GetParticleSystem().emissionRate = moneyRainEffect.GetInitialEmission();

        // countdown
        var time = moneyRainEffectDuration;
        while (time > 0)
        {
            if (gameManager.GetPause()) yield return new WaitUntil(() => !gameManager.GetPause());       // pause
            if (gameManager.GameEnded()) yield return new WaitUntil(() => !gameManager.GameEnded());        // lost game

            time--;
            yield return new WaitForSeconds(1f);
        }

        moneyRainEffect.GetParticleSystem().emissionRate = 0;
        SetMoneyRain(false);
        yield return new WaitUntil(() => moneyRainEffect.GetParticleSystem().particleCount == 0);
        moneyRainEffect.gameObject.SetActive(false);
    }

    private IEnumerator DeflectionCo()
    {
        var level = CurrentLevel(DeflectionLevelName());
        var duration = deflectionDurations[level];

        if (GetDeflection())    // resetting current duration + clearing line
        {
            timeLeft = duration;
            if (level == 4)
                FindObjectOfType<WorldsManager>().ClearChosenLine(0);
            yield break;
        }

        player.SetDeflection(true);
        SetDeflection(true);
        deflectionEffect.gameObject.SetActive(true);
        deflectionEffect.GetParticleSystem().emissionRate = deflectionEffect.GetInitialEmission();

        // 3 & 4 -> permanent deflection, 4 -> also clearing obstacles on current line
        if (level == 4)
        {
            FindObjectOfType<WorldsManager>().ClearChosenLine(0);
            yield break;
        }
        else if (level == 3)
            yield break;

        // countdown
        timeLeft = duration;
        while (timeLeft > 0)
        {
            if (gameManager.GetPause()) yield return new WaitUntil(() => !gameManager.GetPause());       // pause
            if (gameManager.GameEnded()) yield return new WaitUntil(() => !gameManager.GameEnded());        // lost game
            if (!player.GetDeflection()) yield break;   // player already used the deflection

            timeLeft--;
            yield return new WaitForSeconds(1f);
        }

        deflectionEffect.GetParticleSystem().emissionRate = 0;
        yield return new WaitUntil(() => deflectionEffect.GetParticleSystem().particleCount == 0);
        TurnDeflectionEffectOff();
    }
    public void TurnDeflectionEffectOff()
    {
        player.SetDeflection(false);
        deflectionEffect.gameObject.SetActive(false);
        SetDeflection(false);
    }


    public void SetInvincibility(bool b) { isInvincibility = b; }
    public void SetDoublePoints(bool b) { isDoublePoints = b; }
    public void SetMoneyRain(bool b) { isMoneyRain = b; }
    public void SetDeflection(bool b) { isDeflection = b; }

    public bool GetInvincibility() { return isInvincibility; }
    public bool GetDoublePoints() { return isDoublePoints; }
    public bool GetMoneyRain() { return isMoneyRain; }
    public bool GetDeflection() { return isDeflection; }

    private bool IsPowerUp() { return (isInvincibility || isDoublePoints); }

    private int CurrentLevel(string powerUp) { return PlayerPrefs.GetInt(powerUp, 0); }

    // Checks if given string is a power up tag, used in player when colliding
    public bool CheckIfPowerUp(string tag)
    {
        switch (tag)
        {
            case invincibilityTag:
            case doublePointsTag:
            case moneyRainTag:
            case deflectionTag: return true;
            default: break;
        }
        return false;
    }

    public int GetNrOfPowerUps() { return nrOfPowerUps; }

    public void SetPowerUpEffectsActive(bool b)
    {
        PowerUpEffect[] effects = FindObjectsOfType<PowerUpEffect>();
        foreach(PowerUpEffect e in effects)
        {
            if (b)
                e.gameObject.transform.localScale = new Vector3(1, 1, 1);
            else
                e.gameObject.transform.localScale = new Vector3(0, 0, 0);
        }
    }

    public string InvincibilityLevelName() { return invincibilityLevelName; }
    public string DoublePointsLevelName() { return doublePointsLevelName; }
    public string MoneyRainLevelName() { return moneyRainLevelName; }
    public string DeflectionLevelName() { return deflectionLevelName; }

    // Spawn chance methods
    private void LoadSpawnChance()
    {
        spawnChance = PlayerPrefs.GetInt(spawnChancePrefsName, spawnChanceDefaultValue);
        UpdateSpawnChanceStuff();
    }

    public void IncreaseSpawnChance()   // button
    {
        // Returning if maxed out
        if (PlayerPrefs.GetInt(spawnChancePrefsName, spawnChanceDefaultValue) - spawnChanceDefaultValue >= spawnChanceLevels) return;

        // Checking if enough coins
        int level = Mathf.Abs(PlayerPrefs.GetInt(spawnChancePrefsName, spawnChanceDefaultValue) - spawnChanceDefaultValue);
        if (coinManager.GetCoins() < spawnChancePrices[level])
            return;
        else
        {
            coinManager.DecreaseCoins(spawnChancePrices[level]);
            spawnChance--;  // the smaller the more often there will be powerUp spawned
            PlayerPrefs.SetInt("SpawnChance", spawnChance);
            UpdateSpawnChanceStuff();
        }
    }

    private void UpdateSpawnChanceStuff()
    {
        // Price text
        int level = Mathf.Abs(PlayerPrefs.GetInt(spawnChancePrefsName, spawnChanceDefaultValue) - spawnChanceDefaultValue);
        if (level == spawnChanceLevels)
            spawnChancePriceText.text = "Max";
        else if (level < spawnChanceLevels) 
            spawnChancePriceText.text = spawnChancePrices[level].ToString();

        // Slider
        spawnChanceSlider.value = level;
    }

    private bool CheckIfMoneyRainPossible()
    {
        // Finding all coins spawned and checking whether the amount is greater than border established 
        // (it is so that max 2 moneyRains can be spawned at once - fraerate drops if more)
        var coinsSpawned = GameObject.FindGameObjectsWithTag("Coin").Length;
        if (coinsSpawned > coinsSpawnedBorder)
            return false;
        else
            return true;
    }

    private void TurnPowerUpsOff()
    {
        invincibilityEffect.gameObject.SetActive(false);
        doublePointsEffect.gameObject.SetActive(false);
        moneyRainEffect.gameObject.SetActive(false);
        deflectionEffect.gameObject.SetActive(false);
    }

    /***/

    /*private IEnumerator PreviousDoubleMoneyCo()
    {
        var level = CurrentLevel("DoubleMoneyLevel");
        var duration = doubleMoneyDurations[level];

        if (GetDoubleMoney())
        {
            doubleMoneyEffect.MaxSlider(duration);
            yield break;
        }
        else if (IsPowerUp())
            yield break;

        if (level == 4)
            coinManager.SetMultiplier(5);
        else
            coinManager.SetMultiplier(2 + level / 2);

        SetDoubleMoney(true);
        doubleMoneyEffect.gameObject.SetActive(true);
        StartCoroutine(doubleMoneyEffect.ResetEffectCo(duration));

        yield return new WaitForSeconds(duration);
        yield return new WaitUntil(() => doubleMoneyEffect.IsSliderZero());

        coinManager.ResetMultiplier();
        SetDoubleMoney(false);

        yield return new WaitUntil(() => doubleMoneyEffect.GetParticleSystem().particleCount == 0);
        doubleMoneyEffect.gameObject.SetActive(false);
    }*/


    /*public void TriggerDisappearAnimations()
    {
        foreach (string t in powerUpTags)
        {
            var powerUps = GameObject.FindGameObjectsWithTag(t);
            foreach (GameObject o in powerUps)
            {
                o.GetComponent<Animator>().SetTrigger("Disappear");
            }
        }
    }

    public void TriggerAppearAnimations()
    {
        foreach (string t in powerUpTags)
        {
            var powerUps = GameObject.FindGameObjectsWithTag(t);
            foreach (GameObject o in powerUps)
            {
                o.GetComponent<Animator>().SetTrigger("Appear");
            }
        }
    }*/


    //float speed = FindObjectOfType<LineMover>().GetMoveSpeed() + level*3;   // current speed + level * 2
    //StartCoroutine(gameManager.ChangeMoveSpeedCo(speed, false, false, 1));

    //StartCoroutine(gameManager.ChangeMoveSpeedCo(1, false, false, 1));
}
