using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public float fadeDuration = 5f;
    public float sceneDisplayDuration = 5f;
    public Image fadeImage;
    public string nextSceneName;

    [Header("Audio")]
    public AudioSource backgroundMusicSource;
    public AudioSource soundEffectSource;
    public AudioClip backgroundMusic;
    public AudioClip transitionSound;

    private void Start()
    {
        // Ensure the fade image covers the entire screen
        fadeImage.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
        
        /* Start background music
        if (backgroundMusic != null && backgroundMusicSource != null)
        {
            backgroundMusicSource.clip = backgroundMusic;
            backgroundMusicSource.loop = true;
            backgroundMusicSource.Play();
        }
        */
        // Start with a black screen and fade in
        //StartCoroutine(FadeInAndWait());
    }

    public void ExitGame()
    {
        StartCoroutine(FadeOutAndExit());
    }

    private IEnumerator FadeInAndWait()
    {
        yield return StartCoroutine(FadeIn());

        // Wait for the specified duration
        yield return new WaitForSeconds(sceneDisplayDuration);

        // Start fading out and load the next scene
        StartCoroutine(FadeOutAndLoadScene());
    }

    public IEnumerator FadeIn()
    {
        fadeImage.gameObject.SetActive(true);
         // Start background music
        if (backgroundMusic != null && backgroundMusicSource != null)
        {
            backgroundMusicSource.clip = backgroundMusic;
            backgroundMusicSource.loop = true;
            backgroundMusicSource.Play();
        }
        

        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = 1f - (elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        // Ensure the image is fully transparent at the end
        color.a = 0f;
        fadeImage.color = color;
        fadeImage.gameObject.SetActive(false);
    }

    private IEnumerator FadeOutAndLoadScene()
    {
        fadeImage.gameObject.SetActive(true);
        // Play transition sound
        if (transitionSound != null && soundEffectSource != null)
        {
            soundEffectSource.PlayOneShot(transitionSound);
        }

        // Start fading out the background music
        float startVolume = backgroundMusicSource.volume;

        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = elapsedTime / fadeDuration;
            fadeImage.color = color;

            // Fade out the background music
            if (backgroundMusicSource != null)
            {
                backgroundMusicSource.volume = Mathf.Lerp(startVolume, 0, elapsedTime / fadeDuration);
            }

            yield return null;
        }

        // Ensure the image is fully opaque and music is stopped at the end
        color.a = 1f;
        fadeImage.color = color;
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.Stop();
        }

        // Load the next scene
        SceneManager.LoadScene(nextSceneName);
    }

    public IEnumerator FadeOutAndExit()
    {
        fadeImage.gameObject.SetActive(true);
        // Play transition sound
        if (transitionSound != null && soundEffectSource != null)
        {
            soundEffectSource.PlayOneShot(transitionSound);
        }

        // Start fading out the background music
        float startVolume = backgroundMusicSource.volume;

        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = elapsedTime / fadeDuration;
            fadeImage.color = color;

            // Fade out the background music
            if (backgroundMusicSource != null)
            {
                backgroundMusicSource.volume = Mathf.Lerp(startVolume, 0, elapsedTime / fadeDuration);
            }

            yield return null;
        }

        // Ensure the image is fully opaque and music is stopped at the end
        color.a = 1f;
        fadeImage.color = color;
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.Stop();
        }

        // Exit the game
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void FadeToNextScene()
    {
        StartCoroutine(FadeOutAndLoadScene());
    }


}