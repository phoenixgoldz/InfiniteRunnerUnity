using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject menuPanel;
    public GameObject optionsPanel;

    [Header("UI Elements")]
    public Slider volumeSlider;
    public TMP_Dropdown graphicsDropdown;
    public Slider sensitivitySlider;
    public Toggle musicToggle;
    public Toggle vibrationToggle;

    [Header("Saving UI")]
    public GameObject savingText;  // "Saving..." text
    public GameObject savingIcon;  // Rotating icon

    private bool isSaving = false;

    void Start()
    {
        SetupGraphicsDropdown(); // Restored function for setting up graphics options
        LoadSettings(); // Load saved settings on start
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void OpenOptions()
    {
        menuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void ApplySettings()
    {
        if (isSaving) return; // Prevent multiple saves at once

        Debug.Log("Applying Settings...");
        isSaving = true;

        // Show "Saving..." UI
        savingText.SetActive(true);
        savingIcon.SetActive(true);
        StartCoroutine(RotateSavingIcon()); // Restored rotation function

        // Save settings using PlayerPrefs
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        PlayerPrefs.SetInt("GraphicsQuality", graphicsDropdown.value);
        PlayerPrefs.SetFloat("ControlSensitivity", sensitivitySlider.value);
        PlayerPrefs.SetInt("MusicEnabled", musicToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("VibrationEnabled", vibrationToggle.isOn ? 1 : 0);

        PlayerPrefs.Save(); // Save changes
        Debug.Log("Settings Saved!");

        // Apply vibration setting
        ApplyVibrationSetting();

        // Hide saving UI after a short delay
        StartCoroutine(HideSavingUI()); // Restored function to hide "Saving..."
    }

    void ApplyVibrationSetting()
    {
        bool isVibrationEnabled = PlayerPrefs.GetInt("VibrationEnabled", 1) == 1;

        if (isVibrationEnabled)
        {
            Debug.Log("Vibration is ON");
            VibrateDevice(); // Test vibration when setting is applied
        }
        else
        {
            Debug.Log("Vibration is OFF");
        }
    }

    void LoadSettings()
    {
        Debug.Log("Loading Settings...");

        // Load saved values (or defaults)
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1.0f);
        graphicsDropdown.value = PlayerPrefs.GetInt("GraphicsQuality", DetectBestQualityLevel()); // Restored function
        sensitivitySlider.value = PlayerPrefs.GetFloat("ControlSensitivity", 1.0f);
        musicToggle.isOn = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        vibrationToggle.isOn = PlayerPrefs.GetInt("VibrationEnabled", 1) == 1;

        ApplyLoadedSettings();
    }

    void ApplyLoadedSettings()
    {
        AudioListener.volume = volumeSlider.value;
        QualitySettings.SetQualityLevel(graphicsDropdown.value);
        ApplyVibrationSetting();
        Debug.Log("Settings Applied from Load!");
    }

    void SetupGraphicsDropdown()
    {
        graphicsDropdown.ClearOptions();

        // Get available quality levels
        string[] qualityLevels = QualitySettings.names;
        int bestQualityLevel = DetectBestQualityLevel();

        // Populate dropdown with dynamic quality labels
        foreach (string level in qualityLevels)
        {
            graphicsDropdown.options.Add(new TMP_Dropdown.OptionData(level));
        }

        // Set default based on detected quality
        graphicsDropdown.value = bestQualityLevel;
        graphicsDropdown.RefreshShownValue();
    }

    int DetectBestQualityLevel()
    {
        // Detect device performance and set a default quality level
        int memory = SystemInfo.systemMemorySize; // RAM in MB
        int processorCores = SystemInfo.processorCount;
        int gpuPerformance = (SystemInfo.graphicsShaderLevel >= 45) ? 2 : (SystemInfo.graphicsShaderLevel >= 30) ? 1 : 0;

        // Assign quality level based on detected specs
        if (memory > 6000 && processorCores >= 6) return 2;  // High Quality
        if (memory > 3000 && processorCores >= 4) return 1;  // Medium Quality
        return 0;  // Low Quality
    }

    IEnumerator RotateSavingIcon()
    {
        while (savingIcon.activeSelf)
        {
            savingIcon.transform.Rotate(0, 0, -200 * Time.deltaTime); // Rotate on Z-axis
            yield return null;
        }
    }

    IEnumerator HideSavingUI()
    {
        yield return new WaitForSeconds(1.5f); // Simulate saving delay

        // Hide UI
        savingText.SetActive(false);
        savingIcon.SetActive(false);
        isSaving = false;

        CloseOptions(); // Return to main menu
    }

    public void VibrateDevice()
    {
        if (PlayerPrefs.GetInt("VibrationEnabled", 1) == 1)
        {
#if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
            Debug.Log("Device Vibrating...");
#endif
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
