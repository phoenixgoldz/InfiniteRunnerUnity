using UnityEngine;
using Assets.Scripts;
using UnityEngine.SceneManagement;


public class CharacterRotateMovement : MonoBehaviour
{
    //character model found in https://www.assetstore.unity3d.com/en/#!/content/3012

    private Vector3 moveDirection = Vector3.zero;
    public float gravity = 20f;
    private CharacterController controller;
    private Animator anim;

    public float JumpSpeed = 8.0f;
    public float Speed = 6.0f;
    public Transform CharacterGO;

    bool isInSwipeArea;


    IInputDetector inputDetector = null;

    // Use this for initialization
    void Start()
    {
        moveDirection = transform.forward;
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= Speed;

        UIManager.Instance.ResetScore();
        UIManager.Instance.SetStatus(Constants.StatusTapToStart);

        GameManager.Instance.GameState = GameState.Start;

        anim = CharacterGO.GetComponent<Animator>();
        inputDetector = GetComponent<IInputDetector>();
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (GameManager.Instance.GameState)
        {
            case GameState.Start:
                if (Input.GetMouseButtonUp(0))
                {
                    anim.SetBool(Constants.AnimationStarted, true);
                    var instance = GameManager.Instance;
                    instance.GameState = GameState.Playing;

                    UIManager.Instance.SetStatus(string.Empty);
                }
                break;
            case GameState.Playing:
                UIManager.Instance.IncreaseScore(0.001f);

                CheckHeight();

                DetectJumpOrSwipeLeftRight();

                //apply gravity
                moveDirection.y -= gravity * Time.deltaTime;
                //move the player
                controller.Move(moveDirection * Time.deltaTime);

                break;
            case GameState.Dead:
                anim.SetBool(Constants.AnimationStarted, false);
                if (Input.GetMouseButtonUp(0))
                {
                    //restart
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
                break;
            default:
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
        var inputDirection = inputDetector.DetectInputDirection();
        if (controller.isGrounded && inputDirection.HasValue && inputDirection == InputDirection.Top)
        {
            moveDirection.y = JumpSpeed;
            anim.SetBool(Constants.AnimationJump, true);
        }
        else
        {
            anim.SetBool(Constants.AnimationJump, false);
        }


        if (GameManager.Instance.CanSwipe && inputDirection.HasValue &&
         controller.isGrounded && inputDirection == InputDirection.Right)
        {
            transform.Rotate(0, 90, 0);
            moveDirection = Quaternion.AngleAxis(90, Vector3.up) * moveDirection;
            //allow the user to swipe once per swipe location
            GameManager.Instance.CanSwipe = false;
        }
        else if (GameManager.Instance.CanSwipe && inputDirection.HasValue &&
         controller.isGrounded && inputDirection == InputDirection.Left)
        {
            transform.Rotate(0, -90, 0);
            moveDirection = Quaternion.AngleAxis(-90, Vector3.up) * moveDirection;
            GameManager.Instance.CanSwipe = false;
        }


    }





}
