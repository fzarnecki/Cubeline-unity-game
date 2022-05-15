using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SFXPlayer : MonoBehaviour
{
    [SerializeField] AudioSource audioSrc;
    [SerializeField] AudioClip clickSFX;
    [SerializeField] AudioClip changeSFX;
    [SerializeField] AudioClip jumpSFX;
    [SerializeField] AudioClip teleportSFX;
    [SerializeField] AudioClip divisionSFX;
    [SerializeField] AudioClip failSFX;
    [SerializeField] AudioClip pointsSFX;
    [SerializeField] AudioClip respawnSFX;
    [SerializeField] AudioClip coinPickupSFX;
    [SerializeField] AudioClip moneyBagSFX;
    [SerializeField] AudioClip animationChange;
    [SerializeField] AudioClip powerUpPickup;

    /***/

    public void PlayPowerUpPickupSFX() { audioSrc.PlayOneShot(powerUpPickup); }

    public void PlayAnimationChangeSFX() { audioSrc.PlayOneShot(animationChange); }

    public void PlayMoneyBagSFX() { audioSrc.PlayOneShot(moneyBagSFX); }

    public void PlayCoinPickupSFX() { audioSrc.PlayOneShot(coinPickupSFX); }

    public void PlayRespawnSFX() { audioSrc.PlayOneShot(respawnSFX); }

    public void PlayPointCountingSFX() { audioSrc.PlayOneShot(pointsSFX); }

    public void PlayClickSFX() { audioSrc.PlayOneShot(clickSFX); }

    public void PlayChangeSFX() { audioSrc.PlayOneShot(changeSFX); }

    public void PlayJumpSFX() { audioSrc.PlayOneShot(jumpSFX); }

    public void PlayTeleportSFX() { audioSrc.PlayOneShot(teleportSFX); }

    public void PlayDivisionSFX() { audioSrc.PlayOneShot(divisionSFX); }

    public void PlayFailSFX() { audioSrc.PlayOneShot(failSFX); }

    public void PlayPlayerJumpSFX()
    {
        switch (GameManager.currentWorld)
        {
            case 0:
                PlayJumpSFX();
                break;
            case 1:
                PlayTeleportSFX();
                break;
            case 2:
                PlayDivisionSFX();
                break;
            default:
                break;
        }
    }

    /***/

    //[SerializeField] AudioClip hoverSFX;

    /*public void PlayHoverSFX()
    {
        audioSrc.PlayOneShot(hoverSFX);
    }*/
}