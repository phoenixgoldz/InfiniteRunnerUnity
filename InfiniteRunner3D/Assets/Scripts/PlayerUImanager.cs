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
    private CharacterController characterController;

    public static PlayerUIManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateScore(0);
        UpdateDistance(0);
        pauseButton.onClick.AddListener(TogglePauseMenu);
        resumeButton.onClick.AddListener(TogglePauseMenu);
        quitButton.onClick.AddListener(QuitGame);

        backgroundMusic = Object.FindFirstObjectByType<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            characterController = player.GetComponent<CharacterController>();
        }
    }

    void Update()
    {
        if (!isPaused && characterController != null && characterController.velocity.magnitude > 0.1f)
        {
            distanceTraveled += Time.deltaTime * characterController.velocity.magnitude;
            UpdateDistance(distanceTraveled);
        }
    }

    public int GetScore()
    {
        return score;
    }

    public float GetDistance()
    {
        return distanceTraveled;
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
