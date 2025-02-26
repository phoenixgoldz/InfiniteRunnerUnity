using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public enum GameState
{
    Start,
    Playing,
    Dead
}

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public Text ScoreText, StatusText;
    private float score = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreText();
    }
    
    public void SetScore(float value)
    {
        score = value;
        UpdateScoreText();
    }

    public void IncreaseScore(float value)
    {
        score += value;
        UpdateScoreText();
    }
    
    private void UpdateScoreText()
    {
        if (ScoreText != null)
            ScoreText.text = score.ToString();
    }

    public void SetStatus(string text)
    {
        if (StatusText != null)
            StatusText.text = text;
    }
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState GameState { get; set; }
    public bool CanSwipe { get; set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        GameState = GameState.Start;
        CanSwipe = false;
    }

    public void Die()
    {
        UIManager.Instance.SetStatus("Dead. Tap to start");
        GameState = GameState.Dead;
    }
}

public class CharacterSidewaysMovement : MonoBehaviour
{
    private CharacterController controller;
    private Animator anim;
    private Vector3 moveDirection = Vector3.zero;
    private bool isChangingLane = false;
    private Vector3 locationAfterChangingLane;
    private Vector3 sidewaysMovementDistance = Vector3.right * 2f;
    public float SideWaysSpeed = 5.0f;
    public float JumpSpeed = 8.0f;
    public float Speed = 6.0f;
    public float Gravity = 20f;
    public Transform CharacterGO;
    private IInputDetector inputDetector;

    void Start()
    {
        moveDirection = transform.forward * Speed;
        UIManager.Instance.ResetScore();
        UIManager.Instance.SetStatus("Tap to start");
        GameManager.Instance.GameState = GameState.Start;
        anim = CharacterGO.GetComponent<Animator>();
        inputDetector = GetComponent<IInputDetector>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        switch (GameManager.Instance.GameState)
        {
            case GameState.Start:
                if (Input.GetMouseButtonUp(0))
                {
                    anim.SetBool("started", true);
                    GameManager.Instance.GameState = GameState.Playing;
                    UIManager.Instance.SetStatus("");
                }
                break;
            case GameState.Playing:
                UIManager.Instance.IncreaseScore(0.001f);
                CheckHeight();
                DetectJumpOrSwipeLeftRight();
                moveDirection.y -= Gravity * Time.deltaTime;
                if (isChangingLane && Mathf.Abs(transform.position.x - locationAfterChangingLane.x) < 0.1f)
                {
                    isChangingLane = false;
                    moveDirection.x = 0;
                }
                controller.Move(moveDirection * Time.deltaTime);
                break;
            case GameState.Dead:
                anim.SetBool("started", false);
                if (Input.GetMouseButtonUp(0))
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
                break;
        }
    }

    private void CheckHeight()
    {
        if (transform.position.y < -10)
        {
            GameManager.Instance.Die();
        }
    }

    private void DetectJumpOrSwipeLeftRight()
    {
        var inputDirection = inputDetector?.DetectInputDirection();
        if (controller.isGrounded && inputDirection.HasValue && inputDirection == InputDirection.Top && !isChangingLane)
        {
            moveDirection.y = JumpSpeed;
            anim.SetBool("jump", true);
        }
        else
        {
            anim.SetBool("jump", false);
        }
        if (controller.isGrounded && inputDirection.HasValue && !isChangingLane)
        {
            isChangingLane = true;
            locationAfterChangingLane = inputDirection == InputDirection.Left ? transform.position - sidewaysMovementDistance : transform.position + sidewaysMovementDistance;
            moveDirection.x = inputDirection == InputDirection.Left ? -SideWaysSpeed : SideWaysSpeed;
        }
    }
}
