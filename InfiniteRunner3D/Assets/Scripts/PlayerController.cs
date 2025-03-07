using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;
    private Vector3 moveDirection;
    private bool isMoving = false; // ✅ Movement starts after touching the path

    public float speed = 10f;
    public float turnSpeed = 5f;
    public float jumpForce = 10f;
    public float laneDistance = 3f;

    private int currentLane = 0;
    private bool isJumping = false;
    private bool isSliding = false;
    private Vector2 startTouchPosition, swipeDelta;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate() //  Use FixedUpdate for physics-based movement
    {
        if (isMoving)
        {
            MovePlayer();
            HandleInput();
        }

        if (transform.position.y < -5f)
        {
            Die();
        }
    }

    void MovePlayer()
    {
        if (!isMoving) return; //  Stops movement until player touches the floor

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("❌ Rigidbody not found on player!");
            return;
        }

        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, speed); //  Force movement forward

        Debug.Log($" Moving Forward: {rb.linearVelocity.z}");
    }


    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"▶️ Player touched: {other.gameObject.name} | Tag: {other.tag}"); // Debug log

        if (other.CompareTag("PathTrigger"))
        {
            isMoving = true;
            Debug.Log("✅ Player touched path! Movement activated.");
        }
        else if (other.CompareTag("Obstacle"))
        {
            Debug.Log("❌ Hit an Obstacle!");
            Die();
        }
    }



    void HandleInput()
    {
#if UNITY_ANDROID
            DetectSwipe();
            DetectTilt();
#else
        DetectKeyboardInput();
#endif
    }

    void DetectKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Space)) Jump();
        if (Input.GetKeyDown(KeyCode.S)) Slide();
        if (Input.GetKeyDown(KeyCode.A)) TurnLeft();
        if (Input.GetKeyDown(KeyCode.D)) TurnRight();
        if (Input.GetKeyDown(KeyCode.LeftArrow) && currentLane > -1) ChangeLane(-1);
        if (Input.GetKeyDown(KeyCode.RightArrow) && currentLane < 1) ChangeLane(1);
    }

    void DetectSwipe()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                swipeDelta = touch.position - startTouchPosition;

                if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                {
                    if (swipeDelta.x > 100) ChangeLane(1);
                    else if (swipeDelta.x < -100) ChangeLane(-1);
                }
                else
                {
                    if (swipeDelta.y > 100) Jump();
                    else if (swipeDelta.y < -100) Slide();
                }
            }
        }
    }

    void DetectTilt()
    {
        if (SystemInfo.supportsGyroscope)
        {
            float tilt = Input.acceleration.x;

            if (tilt > 0.2f && currentLane < 1) ChangeLane(1);
            else if (tilt < -0.2f && currentLane > -1) ChangeLane(-1);
        }
    }

    void Jump()
    {
        if (!isJumping)
        {
            isJumping = true;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // ✅ Use Rigidbody physics
            animator.SetBool("isJumping", true);
        }
    }

    void Slide()
    {
        if (!isSliding)
        {
            isSliding = true;
            animator.SetTrigger("isSliding");
            Invoke("StopSliding", 1f);
        }
    }

    void StopSliding()
    {
        isSliding = false;
    }

    void TurnLeft() { animator.SetTrigger("isTurningLeft"); transform.Rotate(Vector3.up, -90f); }
    void TurnRight() { animator.SetTrigger("isTurningRight"); transform.Rotate(Vector3.up, 90f); }

    void ChangeLane(int direction)
    {
        currentLane += direction;
        Vector3 newPosition = transform.position;
        newPosition.x = currentLane * laneDistance;
        rb.MovePosition(newPosition); // ✅ Use Rigidbody movement
    }

    void Die()
    {
        Debug.Log("❌ Player fell off! Saving score & transitioning to leaderboard...");
        SceneManager.LoadScene("Leaderboard");
    }
}
