using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text scoreText;
    public TMP_Text distanceText;
    public GameObject pauseMenu;
    public Button pauseButton;
    public Button resumeButton;
    public Button quitButton;

    private int score = 0;
    private float distanceTraveled = 0f;
    private bool isPaused = false;
    private AudioSource backgroundMusic;
    private GameObject player;
    private CharacterController characterController; // Using Unity's CharacterController

    void Start()
    {
        UpdateScore(0);
        UpdateDistance(0);
        pauseButton.onClick.AddListener(TogglePauseMenu);
        resumeButton.onClick.AddListener(TogglePauseMenu);
        quitButton.onClick.AddListener(QuitGame);

        // Find background music in the scene
        backgroundMusic = Object.FindFirstObjectByType<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player"); // Ensure player has "Player" tag

        if (player != null)
        {
            characterController = player.GetComponent<CharacterController>();
        }
    }

    void Update()
    {
        if (!isPaused && characterController != null && characterController.velocity.magnitude > 0.1f)
        {
            distanceTraveled += Time.deltaTime * characterController.velocity.magnitude; // Distance based on movement
            UpdateDistance(distanceTraveled);
        }
    }

    public void UpdateScore(int amount)
    {
        if (characterController != null && characterController.velocity.magnitude > 0.1f)
        {
            score += amount;
            scoreText.text = "Score: " + score;
        }
    }

    public void UpdateDistance(float distance)
    {
        distanceText.text = Mathf.FloorToInt(distance) + "m";
    }

    public void TogglePauseMenu()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;

        if (backgroundMusic != null)
        {
            backgroundMusic.mute = isPaused;
        }
    }

    public void QuitGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
}