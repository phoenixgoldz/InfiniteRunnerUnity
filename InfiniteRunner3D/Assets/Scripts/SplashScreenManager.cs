using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;

public class SplashScreenManager : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    public CanvasGroup fadeCanvasGroup; // Reference to Canvas Group for fade effect
    public float fadeDuration = 1.5f; // Duration of fade-in/out

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += OnVideoFinished; // Event when video ends

        // Start Fade-In Effect
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float t = fadeDuration;
        while (t > 0)
        {
            t -= Time.deltaTime;
            fadeCanvasGroup.alpha = t / fadeDuration;
            yield return null;
        }
        fadeCanvasGroup.alpha = 0; // Fully transparent
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        StartCoroutine(FadeOutAndLoadScene("MainMenu"));
    }

    IEnumerator FadeOutAndLoadScene(string sceneName)
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCanvasGroup.alpha = t / fadeDuration;
            yield return null;
        }
        fadeCanvasGroup.alpha = 1; // Fully opaque
        SceneManager.LoadScene(sceneName);
    }
}
