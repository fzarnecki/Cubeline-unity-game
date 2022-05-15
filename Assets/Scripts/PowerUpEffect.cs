using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpEffect : MonoBehaviour
{
    [Tooltip("Power up duration slider")]
    [SerializeField] private Slider slider;
    [Tooltip("Animator of the power up effect")]
    [SerializeField] private Animator animator;
    [Tooltip("Particle system of the effect")]
    [SerializeField] private ParticleSystem ps;
    // Initial emission is used to go back to it after it has been changed
    private float initialEmission;

    [Tooltip("By how much slider decreases at once")]
    [SerializeField] private float timeChunk = 0.05f;

    [SerializeField] private GameManager gameManager;

    private float sliderOutTimeBorder = 0.1f;

    /***/

    private void Awake()
    {
        initialEmission = ps.emissionRate;
    }

    public IEnumerator ResetEffectCo(float time)
    {
        // Appear effect
        animator.SetTrigger("In");
        //
        bool isOut = false;
        // Maxing out the slider
        slider.maxValue = time;
        slider.value = time;
        // Resetting the emission
        ps.emissionRate = initialEmission;

        while (slider.value >= timeChunk)
        {
            if (gameManager.GetPause())   // game is paused
            {
                yield return new WaitUntil(() => !gameManager.GetPause());
                var l = FindObjectOfType<LineMover>();
                yield return new WaitUntil(() => l.IsMoving());
            }
            else if (gameManager.GameEnded()) // game is lost
            {
                yield return new WaitUntil(() => !gameManager.GameEnded());
                var l = FindObjectOfType<LineMover>();
                yield return new WaitUntil(() => l.IsMoving());
            }

            // If not already (and it should be) triggering the despawn animation
            if (!isOut && slider.value <= sliderOutTimeBorder*time) { animator.SetTrigger("Out"); isOut = true; ps.emissionRate = 0; }

            // Decreasing the slider value
            slider.value -= timeChunk;
            // Delay
            yield return new WaitForSeconds(timeChunk);
        }
    }

    public bool IsAnimatorPlaying() { return animator.GetCurrentAnimatorStateInfo(0).length > animator.GetCurrentAnimatorStateInfo(0).normalizedTime; }
    public void MaxSlider(float time)
    {
        // if effect is hidden/hiding already, because timing of picking it up again was infortunate, need to trigger entry again
        if (slider.value <= sliderOutTimeBorder * time)
            animator.SetTrigger("In");
        // resetting slider
        slider.value = time;
    }
    public bool IsSliderZero() { return slider.value < 0.05f; }
    public ParticleSystem GetParticleSystem() { return ps; }
    public float GetInitialEmission() { return initialEmission; }
}
