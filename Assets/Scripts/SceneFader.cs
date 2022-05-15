using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    [Tooltip("Black image to imitate fade")]
    [SerializeField] Image image;
    [Tooltip("Animator of the above image")]
    [SerializeField] Animator animator;

    [SerializeField] GameManager gameManager;

    // Controlling the changes, not to override each other
    private bool changing = false;

    /***/
    
    public IEnumerator FadeOutAndReloadCo()
    {
        if (gameManager.IsAction()) yield break;
        gameManager.SetAction(true);
        animator.SetTrigger("FadeOut");
        yield return new WaitUntil(() => image.color.a == 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public IEnumerator FadeCanvasOutCo(GameObject canvas, float fadeSpeed)
    {
        CanvasGroup group = canvas.GetComponent<CanvasGroup>();
        //group.alpha = 1;
        while (group.alpha > 0)
        {
            group.alpha -= Time.deltaTime * fadeSpeed;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        canvas.SetActive(false);
        gameManager.SetAction(false);
    }

    public IEnumerator FadeCanvasInCo(GameObject canvas, float fadeSpeed)
    {
        if (gameManager.IsAction()) yield break;
        gameManager.SetAction(true);
        canvas.SetActive(true);
        gameManager.SetCurrentCanvas(canvas);
        CanvasGroup group = canvas.GetComponent<CanvasGroup>();
        group.alpha = 0;
        while (group.alpha < 1)
        {
            group.alpha += Time.deltaTime * fadeSpeed;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        gameManager.SetAction(false);
    }

    public IEnumerator FadeUnfadeCo(GameObject toFade, GameObject toShow, float fadeSpeed)
    {
        if (gameManager.IsAction()) yield break;
        gameManager.SetAction(true);
        // Fading
        CanvasGroup fade = toFade.GetComponent<CanvasGroup>();
        while (fade.alpha > 0)
        {
            fade.alpha -= Time.deltaTime * fadeSpeed;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        // Off/On
        toFade.SetActive(false);
        toShow.SetActive(true);
        gameManager.SetCurrentCanvas(toShow);

        // Unfading
        CanvasGroup show = toShow.GetComponent<CanvasGroup>();
        show.alpha = 0;
        while (show.alpha < 1)
        {
            show.alpha += Time.deltaTime * fadeSpeed;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        gameManager.SetAction(false);
    }
}
