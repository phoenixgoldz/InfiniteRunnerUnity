using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;
    private Vector3 lanePosition;
    private bool isMoving = false;

    public float speed = 10f;
    public float jumpForce = 10f;
    public float moveSpeed = 5f;
    public float minX = -6.5f;
    public float maxX = 7.5f;

    private bool isJumping = false;
    private bool isSliding = false;

    [SerializeField] private InputActionMap playerControls;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction slideAction;
    private InputAction tiltAction;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        rb.centerOfMass = Vector3.zero;
        rb.inertiaTensorRotation = Quaternion.identity;

        lanePosition = transform.position;

#if UNITY_ANDROID || UNITY_IOS
        AddMobileControls();
#else
        CreateKeyboardControls();
#endif

        playerControls.Enable();
    }

    void OnDestroy()
    {
#if UNITY_ANDROID || UNITY_IOS
        SwipeDetection.instance.swipePerformed -= HandleSwipe;
#endif
        playerControls.Disable();
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
#if UNITY_ANDROID || UNITY_IOS
            DetectTilt();
#endif
            MovePlayer();
        }

        if (transform.position.y < -5f) Die();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PathTrigger"))
        {
            isMoving = true;
            Debug.Log("✅ Player touched path! Movement activated.");
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("❌ Hit an Obstacle!");
            Die();
        }
    }

    void Jump()
    {
        if (IsGrounded() && !isJumping)
        {
            isJumping = true;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetBool("isJumping", true);

            Debug.Log("Jumping!");

            // Reset jump after landing
            Invoke(nameof(ResetJump), 0.5f);
        }
    }

    void ResetJump()
    {
        isJumping = false;
        animator.SetBool("isJumping", false);
    }

    private void CreateKeyboardControls()
    {
        playerControls = new InputActionMap("Player Controls");

        moveAction = playerControls.AddAction("Move", binding: "<Keyboard>/leftArrow" + " | <Keyboard>/rightArrow");
        moveAction.performed += context => MoveHorizontally(context.ReadValue<float>());

        jumpAction = playerControls.AddAction("Jump", binding: "<Keyboard>/space", interactions: "press");
        jumpAction.performed += _ => Jump(); // Ensure this is triggered correctly

        slideAction = playerControls.AddAction("Slide", binding: "<Keyboard>/downArrow", interactions: "press");
        slideAction.performed += _ => Slide();

        playerControls.Enable(); // Enable controls
    }


    private void AddMobileControls()
    {
        if (playerControls.FindAction("Swipe") == null)
        {
            var swipeAction = playerControls.AddAction("Swipe", binding: "<Touchscreen>/delta");
            swipeAction.performed += context => HandleSwipe(context.ReadValue<Vector2>());
            swipeAction.Enable();
        }

        if (playerControls.FindAction("Tilt") == null)
        {
            tiltAction = playerControls.AddAction("Tilt", binding: "<Accelerometer>/acceleration");
            tiltAction.Enable();
        }
    }


    void HandleSwipe(Vector2 swipeDirection)
    {
        Debug.Log($"Swipe Detected: {swipeDirection}");

        if (swipeDirection.y > 0.5f)
        {
            Debug.Log("Swipe Up → Jump Triggered");
            Jump();
        }
        else if (swipeDirection.y < -0.5f)
        {
            Debug.Log("Swipe Down → Slide Triggered");
            Slide();
        }
        else if (swipeDirection.x > 0.5f) // Swipe Right
        {
            Debug.Log("Swipe Right → Moving Right");
            MoveHorizontally(1);
        }
        else if (swipeDirection.x < -0.5f) // Swipe Left
        {
            Debug.Log("Swipe Left → Moving Left");
            MoveHorizontally(-1);
        }
    }


    void DetectTilt()
    {
        if (tiltAction == null)
        {
            Debug.LogWarning("⚠️ Tilt action is NULL! Skipping tilt detection.");
            return;
        }

        float tilt = tiltAction.ReadValue<float>();
        if (tilt > 0.2f) MoveHorizontally(1);
        if (tilt < -0.2f) MoveHorizontally(-1);
    }


    void MovePlayer()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, speed);
    }


    void MoveHorizontally(float direction)
    {
        if (direction == 0) return;

        float newX = transform.position.x + (direction * moveSpeed * Time.deltaTime);
        newX = Mathf.Clamp(newX, minX, maxX);

        transform.position = new Vector3(newX, transform.position.y, transform.position.z);

        Debug.Log($"Moving {direction}: New X Position = {newX}");
    }

    void Slide()
    {
        if (!isSliding)
        {
            isSliding = true;
            animator.SetTrigger("isSliding");
            Invoke(nameof(StopSliding), 1f);
        }
    }

    void StopSliding() { isSliding = false; }

    void Die()
    {
        Debug.Log("❌ Player died! Saving score & transitioning to leaderboard...");
        SceneManager.LoadScene("MainMenu");
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1f);
    }

}
