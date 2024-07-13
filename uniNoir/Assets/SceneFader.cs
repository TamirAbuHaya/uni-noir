using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public Image fadeImage; // Assign the UI Image in the Inspector
    public float fadeDuration = 1f;

    void Start()
    {
        // Ensure the image starts fully transparent
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0f);
        // Start fade-in for the initial scene
        StartCoroutine(Fade(0f));
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOutAndIn(sceneName));
    }

    private IEnumerator FadeOutAndIn(string sceneName)
    {
        // Fade out
        yield return StartCoroutine(Fade(1f));
        // Load new scene
        SceneManager.LoadScene(sceneName);
        // Wait for the scene to load
        yield return new WaitForSeconds(0.1f); // Small delay to ensure the scene loads
        // Fade in
        yield return StartCoroutine(Fade(1f));
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeImage.color.a;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, newAlpha);
            yield return null;
        }

        // Ensure the target alpha is set at the end
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, targetAlpha);
    }
   
}
