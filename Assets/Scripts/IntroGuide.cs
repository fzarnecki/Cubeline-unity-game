using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroGuide : MonoBehaviour
{
    [Header("All displayed components of the guide")]
    [SerializeField] private GameObject componentsContainer;    // to enable components only if needed
    [SerializeField] private GameObject[] components;

    [Header("Other required scripts")]
    [SerializeField] GameManager gameManager;

    // Instead of checking animation state due to some issues
    private float despawnAnimationDuration = 1f;
    // Tracks whether its players' first time playing or not
    private string timesPlayedCounterPrefsName = "TimesPlayed";
    // Tracks whether current display has been pressed
    private bool displayPressed;

    /***/

    public void CheckIfFirstPlay()
    {
        int t = PlayerPrefs.GetInt(timesPlayedCounterPrefsName, 0);
        if (t > 0)
        {
            gameManager.SetFirstTime(false);
            return; // player has already played
        }
        else
        {
            PlayerPrefs.SetInt(timesPlayedCounterPrefsName, 1);  // indicating that first play has occured, not to display guide anymore
            StartCoroutine(PlayGuideCo());
        }
    }

    private IEnumerator PlayGuideCo()
    {
        // Turning off menu canvas
        gameManager.SetMenuCanvasActive(false);

        // Turning components off in case not done in unity (we want to display only one at a time)
        componentsContainer.SetActive(true);
        foreach(GameObject o in components) { o.SetActive(false); }
        // Resetting the press check
        SetDisplayPressed(false);
        
        // Displaing each component and waiting for player to press the screen
        foreach(GameObject o in components)
        {
            // Spawning next display
            o.SetActive(true);
            Animator animator = o.GetComponent<Animator>();
            // animator.SetTrigger("Spawn");    // caused bug when entering

            // Waiting for press
            yield return new WaitUntil(() => IsDisplayPressed());

            // Resetting the check for next iteration
            SetDisplayPressed(false);

            // Despawning the current display
            animator.SetTrigger("Despawn");
            yield return new WaitForSeconds(despawnAnimationDuration);
            o.SetActive(false);
        }
        // Disabling component container
        componentsContainer.SetActive(false);

        // Indicating end of intro guide
        gameManager.SetFirstTime(false);
    }


    private bool IsDisplayPressed() { return displayPressed; }
    private void SetDisplayPressed(bool b) { displayPressed = b; }
    public void DisplayPressed() { displayPressed = true; }   // button

    // public bool IsAnimatorPlaying(Animator a) { return a.GetCurrentAnimatorStateInfo(0).length > a.GetCurrentAnimatorStateInfo(0).normalizedTime; } // ?
}
