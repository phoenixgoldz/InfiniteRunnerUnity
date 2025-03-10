using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;
    private Vector3 lanePosition;
    private bool isMoving = false;
    private bool isFalling = false;

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
#endif
        CreateKeyboardControls(); // Always initialize keyboard controls
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

        // ✅ Detect when player is in the air (falling)
        if (rb.linearVelocity.y < -0.1f && !IsGrounded()) // Falling downwards
        {
            isFalling = true;
            animator.SetBool("IsFalling", true);
        }

        // ✅ Detect when player lands
        if (transform.position.y <= 0.4024749f && isFalling)
        {
            isFalling = false;
            animator.SetBool("IsFalling", false);
            animator.SetBool("IsJumping", false); // Ensure jumping resets too
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

        // ✅ Detect landing from a jump
        if (collision.gameObject.CompareTag("PathTrigger"))
        {
            Debug.Log("🏁 Landed! Resetting Falling & Running.");
            isJumping = false;
            isFalling = false; // Immediately reset falling state
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", false);
            animator.SetBool("IsRunning", true);
        }
    }
    void Jump()
    {
        if (IsGrounded() && !isJumping)
        {
            isJumping = true;
            isFalling = false;
            animator.SetBool("IsJumping", true);
            animator.SetBool("IsFalling", false);
            animator.SetBool("IsRunning", false);

            Debug.Log("🔵 Jump triggered, isJumping = true");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // ✅ Invoke ResetJump faster
            Invoke(nameof(ResetJump), 0.3f);
        }
    }

    void ResetJump()
    {
        isJumping = false;
        animator.SetBool("IsJumping", false);

        // ✅ Check if player is still in air to transition to falling
        if (!IsGrounded())
        {
            animator.SetBool("IsFalling", true);
        }
    }
    private void CreateKeyboardControls()
    {
        if (playerControls == null)
        {
            playerControls = new InputActionMap("Player Controls");
            Debug.Log("🟢 Keyboard controls initialized.");
        }
        else
        {
            Debug.LogWarning("⚠️ Keyboard controls already exist. Skipping duplicate initialization.");
            return;
        }

        // ✅ Move Action (Check if it exists before adding)
        if (playerControls.FindAction("Move") == null)
        {
            moveAction = playerControls.AddAction("Move", binding: "<Keyboard>/a | <Keyboard>/d | <Keyboard>/leftArrow | <Keyboard>/rightArrow");
            moveAction.performed += context =>
            {
                float input = context.ReadValue<float>();
                Debug.Log($"🎮 Keyboard Move Input: {input}");
                MoveHorizontally(input);
            };

            moveAction.canceled += _ =>
            {
                Debug.Log("🎮 Keyboard Released: Stopping Movement");
            };
        }

        // ✅ Jump Action (Check if it exists before adding)
        if (playerControls.FindAction("Jump") == null)
        {
            jumpAction = playerControls.AddAction("Jump", binding: "<Keyboard>/space", interactions: "press");
            jumpAction.performed += _ =>
            {
                Debug.Log("🎮 Spacebar Pressed: Jump!");
                Jump();
            };
        }

        // ✅ Slide Action (Check if it exists before adding)
        if (playerControls.FindAction("Slide") == null)
        {
            slideAction = playerControls.AddAction("Slide", binding: "<Keyboard>/s | <Keyboard>/downArrow", interactions: "press");
            slideAction.performed += _ =>
            {
                Debug.Log("🎮 Down Key Pressed: Slide!");
                Slide();
            };
        }

        // ✅ Tilt Emulation via Arrow Keys (Check if it exists before adding)
        if (playerControls.FindAction("Tilt") == null)
        {
            tiltAction = playerControls.AddAction("Tilt", binding: "<Keyboard>/leftArrow | <Keyboard>/rightArrow");
            tiltAction.performed += context =>
            {
                float tiltInput = context.ReadValue<float>();
                Debug.Log($"🎮 Keyboard Tilt Input: {tiltInput}");
                MoveHorizontally(tiltInput);
            };
        }

        playerControls.Enable();
    }


    /// ✅ **Fix: Add Mobile Controls**
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

    /// ✅ **Fix: Add Swipe Handling**
    private void HandleSwipe(Vector2 swipeDirection)
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
    private void DetectTilt()
    {
        if (tiltAction == null)
        {
            Debug.LogWarning("⚠️ Tilt action is NULL! Skipping tilt detection.");
            return;
        }

        Vector3 tiltInput = tiltAction.ReadValue<Vector3>(); // Read full acceleration
        float tiltValue = tiltInput.x; // Get the X-axis tilt

        if (Mathf.Abs(tiltValue) > 0.2f) //  Apply dead zone to avoid jitter
        {
            MoveHorizontally(tiltValue);
        }
    }

    void MovePlayer()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, speed);
    }
    void MoveHorizontally(float direction)
    {
        if (direction == 0) return;

        float newX = transform.position.x + (direction * moveSpeed * Time.deltaTime);
        newX = Mathf.Clamp(newX, minX, maxX); // ✅ Prevent moving off screen

        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        Debug.Log($"🚶 Moving {direction}: New X Position = {newX}");

        // ✅ Adjust animations for left/right movement
        if (direction > 0)
        {
            animator.SetBool("IsTurningRight", true);
            animator.SetBool("IsTurningLeft", false);
        }
        else if (direction < 0)
        {
            animator.SetBool("IsTurningRight", false);
            animator.SetBool("IsTurningLeft", true);
        }

        Invoke(nameof(ResetTurnAnimations), 0.3f);
    }
 

    void ResetTurnAnimations()
    {
        animator.SetBool("IsTurningRight", false);
        animator.SetBool("IsTurningLeft", false);
    }

    void Slide()
    {
        if (!isSliding)
        {
            isSliding = true;
            animator.SetTrigger("IsSliding");
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
        // ✅ Raycast downwards to detect ground
        return Physics.Raycast(transform.position, Vector3.down, 0.6f);
    }

}
