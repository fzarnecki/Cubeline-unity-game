// Script for the purpose of tag screen displayed at the beginning

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TagSceneFader : MonoBehaviour
{
    [Tooltip("Canvas with the tag")]
    [SerializeField] private CanvasGroup canvas;

    private float fadeSpeed = 0.7f;

    /***/

    void Start()
    {
        StartCoroutine(FadeUnfadeCo());
    }

    private IEnumerator FadeUnfadeCo()
    {
        // Make invisible
        canvas.alpha = 0;

        // Fade in
        while (canvas.alpha < 1)
        {
            canvas.alpha += fadeSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        // Delay
        yield return new WaitForSeconds(0.5f);

        // Fade out
        while (canvas.alpha > 0)
        {
            canvas.alpha -= fadeSpeed * 1.5f * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        // Load game(next) scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
