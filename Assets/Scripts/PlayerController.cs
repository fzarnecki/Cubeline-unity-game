using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("Working variables")]
    private float secondsBetweenJumpAndChange = 0.6f; // <- to delay world changing what eliminates bugs (e.g. wrong color of stomp particles)
    private bool canCollide = true;
    private bool obstacleCollision = true;
    private bool isGrounded = true;
    private bool deflection = false;
    string obstacleTag = "Obstacle";
    string coinTag = "Coin";
    string powerUpTag = "PowerUp";
    private float safeTimeAfterHeadstart = 2f;

    [Tooltip("Scene spot light")]
    public Light light;

    [Header("Managers & refs")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private WorldsManager worldsManager;
    [SerializeField] private SFXPlayer sfxPlayer;
    [SerializeField] private VFXPlayer vfxPlayer;
    [SerializeField] private CoinManager coinManager;
    [SerializeField] private Animator animator;
    [SerializeField] private PowerUpManager powerUpManager;

    [Header("Skins & Upgrades")]
    [SerializeField] private Material[] playerSkins;
    [SerializeField] private GameObject[] playerUpgrades;
    [SerializeField] private GameObject playerSecondPart;
    private GameObject currentUpgrade;
    private ParticleSystem upgradeParticleS;
    private Vector3 initialScale;
    private float upgradeFadeSpeed = 0.01f;
    private bool isSkinMoving;
    private float skinMoveSpeed = -1f;
    private Vector2 offset;
    [SerializeField] private MeshRenderer skin;
    [SerializeField] private MeshRenderer skin2; // 3rd world part of the player
    private string movingSkinPrefs = "MovingSkin";  // 1 if moving / 0 otherwise

    /***/


    private void Awake()
    {
        ResetCurrentUpgrade();
        DisableCollision();
        ApplySkinAndUpgrade();
        offset = new Vector2(skinMoveSpeed, 0); // for moving skin
    }

    private void Update()
    {
        if (IsSkinMoving())
            MoveSkin();

        if (currentUpgrade == null) return;
        else
        {
            currentUpgrade.transform.rotation = Quaternion.Euler(   gameObject.transform.rotation.x * -1.0f, 
                                                                    gameObject.transform.rotation.y * -1.0f, 
                                                                    gameObject.transform.rotation.z * -1.0f     );
        }
    }


    public void OnRightScreenPress()
    {
        if (gameManager.IsHeadstart()) return;
        if (isGrounded) {
            
            DisableJump();
            PlayJumpAnim();
            PlayJumpSFX();
        }
    }
    
    private void OnCollisionEnter(Collision other)  // For now  - disabling world movement when colliding with obstacle
    {
        if (!canCollide)
            return;

        if (gameManager.GameEnded())
            return;

        // Collission with obstacle
        if (IsCollisionWithObstacles() && other.gameObject.tag == obstacleTag)
        {
            if (deflection)
            {
                deflection = false;
                powerUpManager.TurnDeflectionEffectOff();
                sfxPlayer.PlayFailSFX();
                return;
            }

            PointsManager.UpdateHighscore(); // updating highscore
            gameManager.RespawnOrRetry();
        }
        // Picked up coin
        else if (other.gameObject.tag == coinTag)
        {
            other.transform.GetComponent<Collider>().enabled = false;
            other.transform.GetComponent<Animator>().SetTrigger("Despawn");
            sfxPlayer.PlayCoinPickupSFX();
            coinManager.IncreaseCoins(coinManager.GetCoinValue());
        }
        // Picked up power up
        else if (powerUpManager.CheckIfPowerUp(other.gameObject.tag))
        {
            if (gameManager.IsHeadstart()) return;  //double safe?

            other.gameObject.GetComponent<Animator>().SetTrigger("Despawn");
            powerUpManager.PickedUpPowerUp(other.gameObject.tag);
            sfxPlayer.PlayPowerUpPickupSFX();
            Destroy(other.gameObject, 2);
        }
    }

    private void PlayJumpAnim()
    {
        // Disable + enable world changing (partly)
        StartCoroutine(WorldChangeAccordingToJumpCo());

        switch (GameManager.currentWorld)
        {
            case 0:
                animator.SetTrigger("Jump");
                break;
            case 1:
                animator.SetTrigger("Teleportation");
                break;
            case 2:
                animator.SetTrigger("Split");
                break;
            default:
                break;
        }
    }

    private void PlayJumpSFX() { sfxPlayer.PlayPlayerJumpSFX(); }

    public void PlayWorldChangeVFX() { vfxPlayer.PlayWorldChangeVFX(this.transform); }

    public void PlayRespawnVFX() { vfxPlayer.PlayRespawnVFX(this.transform); }

    public void PlayStompParticles() { vfxPlayer.PlayStompParticles(this.transform); }

    public void DisableLight() { light.enabled = false; }

    public void EnableLight() { light.enabled = true; }

    public void EnableJump() { isGrounded = true; }

    public void DisableJump() { isGrounded = false; }

    public void EnableCollision() { this.canCollide = true; }

    public void DisableCollision() { this.canCollide = false; }

    public void SetColliding(bool c) { this.canCollide = c; }

    public void SetDeflection(bool b) { deflection = b; }
    public bool GetDeflection() { return deflection; }

    public void StartPlayer() { animator.SetTrigger("Start"); }

    public void TriggerFloatingState() { animator.SetTrigger("Entry"); }
    public void TriggerIdleState() { animator.SetTrigger("Idle"); }

    public Animator GetAnimator() { return animator; }

    // Collision with obstacles
    public void SetCollisionWithObstacles(bool b) { obstacleCollision = b; }
    public bool IsCollisionWithObstacles() { return obstacleCollision; }
    public void EnableCollisionWithObstaclesWithDelay() { StartCoroutine(ECWOCo()); }
    private IEnumerator ECWOCo() // ECWO - enable collision with obstacles
    {
        yield return new WaitForSeconds(safeTimeAfterHeadstart);    // safe time here not to prolong animation anymore
        yield return new WaitUntil(() => !powerUpManager.GetInvincibility());   // for safety - not to leave collision disabled for good
        SetCollisionWithObstacles(true);
    }   

    private IEnumerator WorldChangeAccordingToJumpCo()
    {
        worldsManager.SetJumpFinished(false);
        yield return new WaitForSeconds(secondsBetweenJumpAndChange);
        worldsManager.SetJumpFinished(true);
    }


    /* Player customization part */


    public string SkinName(int which)
    {
        string name;
        if (IsSkinMoving())
            name = playerSkins[which].name + "Moving";
        else
            name = playerSkins[which].name;

        return name;
    }
    public string UpgradeName(int which)
    {
        return playerUpgrades[which].name;
    }

    public void ChangeSkin(int which)
    {
        if (which > playerSkins.Length) return;

        var chosenSkin = playerSkins[which];
        skin.material = chosenSkin;
        skin2.material = chosenSkin;
    }

    private void ResetCurrentUpgrade() { currentUpgrade = null; upgradeParticleS = null; }
    public void ChangeUpgrade(int which)
    {
        if (which == -1 || which > playerUpgrades.Length) return;   // -1 if no upgrade has been bought

        var chosenUpgrade = playerUpgrades[which];
        if (currentUpgrade != null)
            Destroy(currentUpgrade.gameObject);
        ResetCurrentUpgrade();
        GameObject newUpgrade = Instantiate(chosenUpgrade, transform);
        currentUpgrade = newUpgrade;
        upgradeParticleS = currentUpgrade.GetComponentInChildren<ParticleSystem>();
    }

    public void ApplySkinAndUpgrade()
    {
        // Changing skin & upgrade
        ChangeSkin(PlayerPrefs.GetInt("CurrentSkin", 0));
        ChangeUpgrade(PlayerPrefs.GetInt("CurrentUpgrade", -1));

        // Applying skin movement if necessary
        int movement = PlayerPrefs.GetInt(MovingSkinPrefs(), 0);
        if (movement == 1)
            SetIsSkinMoving(true);
        else
        {
            SetIsSkinMoving(false);
        }
    }

    // Scaling for the purpose of teleport
    public void ScaleUpgradeDown()
    {
        if (currentUpgrade == null || upgradeParticleS == null) return;

        initialScale = upgradeParticleS.transform.localScale;
        while (upgradeParticleS.transform.localScale.x > 0 || upgradeParticleS.transform.localScale.y > 0 || upgradeParticleS.transform.localScale.z > 0)
        {
            upgradeParticleS.transform.localScale -= new Vector3(upgradeFadeSpeed, upgradeFadeSpeed, upgradeFadeSpeed);
        }
    }
    public void ScaleUpgradeUp()
    {
        if (currentUpgrade == null || upgradeParticleS == null) return;
        
        while (upgradeParticleS.transform.localScale.x < initialScale.x)
        {
            upgradeParticleS.transform.localScale += new Vector3(upgradeFadeSpeed, 0, 0);
        }
        while (upgradeParticleS.transform.localScale.y < initialScale.y)
        {
            upgradeParticleS.transform.localScale += new Vector3(0, upgradeFadeSpeed, 0);
        }
        while (upgradeParticleS.transform.localScale.z < initialScale.z)
        {
            upgradeParticleS.transform.localScale += new Vector3(0, 0, upgradeFadeSpeed);
        }
    }

    public bool IsSkinMoving() { return isSkinMoving; }
    public void SetIsSkinMoving(bool b)
    {
        isSkinMoving = b;
        if (!b) // if not moving -> resetting offset to 0,0 to make it look as normally would
        {
            skin.material.mainTextureOffset = new Vector2(0, 0);
            skin2.material.mainTextureOffset = new Vector2(0, 0);
        }
    }
    // Changing offset of the material to make it 'move'
    private void MoveSkin()
    {
        if (skin)
            skin.material.mainTextureOffset += offset * Time.deltaTime;
        if (playerSecondPart && skin2)
            skin2.material.mainTextureOffset += offset * Time.deltaTime;
    }

    public void AdjustMovingSkinPrefs()
    {
        if (IsSkinMoving())
            PlayerPrefs.SetInt(MovingSkinPrefs(), 1);
        else
            PlayerPrefs.SetInt(MovingSkinPrefs(), 0);
    }

    public string MovingSkinPrefs() { return movingSkinPrefs; }
}
