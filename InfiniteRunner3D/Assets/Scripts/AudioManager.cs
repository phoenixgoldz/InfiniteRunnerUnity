using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;  // Main music player
    public AudioSource sfxSource;    // Sound Effects (SFX) player

    [Header("Audio Clips")]
    public AudioClip mainMenuMusic;
    public AudioClip[] level1MusicTracks; // Array for Level 1 music
    public AudioClip buttonClickSFX; // Button click sound effect

    [Header("UI Elements")]
    public Slider masterVolumeSlider; // Reference to the volume slider

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f); // Default to 50% if not set
        SetVolume(savedVolume);

        PlayMusicForScene(SceneManager.GetActiveScene().name);
        SceneManager.sceneLoaded += OnSceneLoaded;

        ApplyButtonClickSound();

        // 🔥 Attach the real-time listener to the volume slider
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = savedVolume;
            masterVolumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
        ApplyButtonClickSound();

        // 🔄 Ensure the volume slider still controls the volume in a new scene
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.onValueChanged.AddListener(SetVolume);
            masterVolumeSlider.value = GetVolume();
        }
    }

    void PlayMusicForScene(string sceneName)
    {
        if (sceneName == "MainMenu")
        {
            PlayMusic(mainMenuMusic);
        }
        else if (sceneName == "Level 1")
        {
            PlayRandomLevel1Music();
        }
    }

    void PlayRandomLevel1Music()
    {
        if (level1MusicTracks.Length == 0)
        {
            Debug.LogWarning("No Level 1 music tracks assigned!");
            return;
        }

        int randomIndex = Random.Range(0, level1MusicTracks.Length);
        PlayMusic(level1MusicTracks[randomIndex]);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip) return;

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void SetVolume(float volume)
    {
        musicSource.volume = volume;
        sfxSource.volume = volume;

        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return PlayerPrefs.GetFloat("MusicVolume", 0.5f); // Ensure it starts at 50%
    }

    void ApplyButtonClickSound()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);

        foreach (Button button in buttons)
        {
            button.onClick.RemoveListener(PlayButtonClickSFX);
            button.onClick.AddListener(PlayButtonClickSFX);
        }

        Debug.Log($"Applied button click sound to {buttons.Length} buttons in {SceneManager.GetActiveScene().name}");
    }

    void PlayButtonClickSFX()
    {
        PlaySFX(buttonClickSFX);
    }
}
