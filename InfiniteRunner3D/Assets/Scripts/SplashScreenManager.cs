using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class SplashScreenManager : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += OnVideoFinished; // Event when video ends
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        SceneManager.LoadScene("MainMenuScene"); 
    }
}
