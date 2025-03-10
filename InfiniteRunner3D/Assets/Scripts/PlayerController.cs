using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEditor.Rendering.LookDev;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;
<<<<<<< Updated upstream
    private Vector3 lanePosition;
    private bool isMoving = false; // ✅ Movement starts after touching the path
=======

    private bool isMoving = false;
    private bool isFalling = false;
>>>>>>> Stashed changes

    public float speed = 10f;
    public float turnSpeed = 5f;
    public float jumpForce = 10f;
<<<<<<< Updated upstream
    public float laneDistance = 3f;
=======
    public float moveSpeed = 3f;
    public float pathWidth = 4f;

    private float currentKeyboardShift = 0;
    private float shiftVelocity = 0;
>>>>>>> Stashed changes

    private int currentLane = 0;
    private bool isJumping = false;
    private bool isSliding = false;
    private bool justTurned = false;
    private Vector2 startTouchPosition, swipeDelta;

<<<<<<< Updated upstream
    [SerializeField, Space(10)] private InputActionMap playerControls;
=======
    private Vector2 startTouchPosition, swipeDelta;

    public InputActionMap playerControls;
>>>>>>> Stashed changes

    // Keyboard Controls
    private InputAction jumpAction;
    private InputAction slideAction;
<<<<<<< Updated upstream
    private InputAction turnAction;
    private InputAction laneShiftAction;
=======
    private InputAction shiftAction;
>>>>>>> Stashed changes

    // Touch Controls
    private InputAction tiltAction;
    private float currentTilt => tiltAction.ReadValue<float>();


    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

<<<<<<< Updated upstream
        playerControls = new InputActionMap();
=======
        // playerControls ??= new InputActionMap();
>>>>>>> Stashed changes

        // Prevents Constraint Issues (hopefully)
        // Thank you https://discussions.unity.com/t/rigibody-constraints-do-not-work-still-moves-a-little/205580, you are amazing
        rb.centerOfMass = Vector3.zero;
        rb.inertiaTensorRotation = Quaternion.identity;

<<<<<<< Updated upstream
        lanePosition = transform.position;

        #if UNITY_ANDROID || UNITY_IOS
        AddMobileControls();
        #else
        CreateKeyboardControls();
        #endif
=======
        CreateKeyboardControls();
        AddMobileControls();
>>>>>>> Stashed changes

        playerControls.Enable();
    }

    void OnDestroy()
    {
<<<<<<< Updated upstream
        #if UNITY_ANDROID || UNITY_IOS
        SwipeDetection.instance.swipePerformed -= context => HandleSwipe(context);
        #endif
=======
        SwipeDetection.instance.swipePerformed -= context => HandleSwipe(context);
>>>>>>> Stashed changes

        playerControls.Disable();
    }

<<<<<<< Updated upstream
    void FixedUpdate() //  Use FixedUpdate for physics-based movement
=======
    void FixedUpdate() // Use FixedUpdate for physics-based movement
>>>>>>> Stashed changes
    {
        // (justTurned) justTurned = false;

        if (isMoving)
        {
<<<<<<< Updated upstream
            #if UNITY_ANDROID || UNITY_IOS
            DetectTilt();
            #endif

=======
>>>>>>> Stashed changes
            // Move Player forward
            MovePlayer();
        }

<<<<<<< Updated upstream
=======
        // ✅ Detect when player is in the air (falling)
        if (rb.linearVelocity.y < -0.1f && !IsGrounded()) // Falling downwards
        {
            isFalling = true;
            animator.SetBool("IsFalling", true);
        }

        // ✅ Detect when player lands
        if (isFalling && IsGrounded())
        {
            isFalling = false;
            animator.SetBool("IsFalling", false);
            animator.SetBool("IsJumping", false); // Ensure jumping resets too
        }

>>>>>>> Stashed changes
        // Check if Player falls off
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
<<<<<<< Updated upstream
        else
        {
            Debug.Log($"▶️ Player touched: {collision.gameObject.name} | Tag: {collision.gameObject.tag}"); // Debug log
=======

        // ✅ Detect landing from a jump
        if (collision.gameObject.CompareTag("PathTrigger"))
        {
            Debug.Log("🏁 Landed! Resetting Falling & Running.");
            isJumping = false;
            isFalling = false; // Immediately reset falling state
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", false);
            animator.SetBool("IsRunning", true);
>>>>>>> Stashed changes
        }
    }


    private void CreateKeyboardControls()
    {
<<<<<<< Updated upstream
        playerControls = new InputActionMap("Player Controls");
        
        jumpAction = playerControls.AddAction("Jump", binding: "<Keyboard>/Space", interactions: "press");
        jumpAction.performed += _ => Jump();
        
        slideAction = playerControls.AddAction("Slide", binding: "<Keyboard>/s", interactions: "press");
        slideAction.performed += _ => Slide();

        turnAction = playerControls.AddAction("Turn", interactions: "press");
        turnAction.AddCompositeBinding("1DAxis").With("Negative", "<Keyboard>/a").With("Positive", "<Keyboard>/d");
        // turnAction.performed += context => Turn(context.ReadValue<float>());

        laneShiftAction = playerControls.AddAction("Move", interactions: "press");
        laneShiftAction.AddCompositeBinding("1DAxis").With("Negative", "<Keyboard>/leftArrow").With("Positive", "<Keyboard>/rightArrow");
        laneShiftAction.performed += context => ChangeLane(context.ReadValue<float>());
=======
        if (playerControls.FindAction("Jump") == null) jumpAction = playerControls.AddAction("Jump", binding: "<Keyboard>/Space", interactions: "press");
        else jumpAction = playerControls.FindAction("Jump");
        jumpAction.performed += _ => Jump();

        if (playerControls.FindAction("Slide") == null) slideAction = playerControls.AddAction("Slide", binding: "<Keyboard>/s", interactions: "press");
        else slideAction = playerControls.FindAction("Slide");
        slideAction.performed += _ => Slide();

        if (playerControls.FindAction("Shift Horizontally") == null)
        {
            shiftAction = playerControls.AddAction("Shift Horizontally", interactions: "hold");
            shiftAction.AddCompositeBinding("1DAxis").With("Negative", "<Keyboard>/leftArrow").With("Positive", "<Keyboard>/rightArrow");
            shiftAction.AddCompositeBinding("1DAxis").With("Negative", "<Keyboard>/a").With("Positive", "<Keyboard>/d");
        }
        else shiftAction = playerControls.FindAction("Shift Horizontally");
        shiftAction.started += context => currentKeyboardShift = context.ReadValue<float>();
        shiftAction.canceled += _ => { currentKeyboardShift = 0; shiftVelocity = 0; };
>>>>>>> Stashed changes
    }

    private void AddMobileControls()
    {
        SwipeDetection.instance.swipePerformed += context => HandleSwipe(context);

        // Enable Gyro Sensors
        // Thank you https://discussions.unity.com/t/tutorial-for-input-system-and-accelerometer-gyro/790202/2, extremely appreciated
        if (UnityEngine.InputSystem.Gyroscope.current != null) InputSystem.EnableDevice(UnityEngine.InputSystem.Gyroscope.current);

<<<<<<< Updated upstream
        tiltAction = playerControls.AddAction("Move", binding: "<Gyroscope>/angularVelocity/y");
    }

    void HandleSwipe(Vector2 swipeDirection)
    {
        print("Screen Swiped");

        if (swipeDirection.y == 1) Jump();
        else if (swipeDirection.y == -1) Slide();
    }

    void DetectTilt()
    {
        if (currentTilt != 0) print(currentTilt);

        if (currentTilt > 0.2f) ChangeLane(1);
        if (currentTilt < -0.2f) ChangeLane(-1);
    }

    void MovePlayer()
    {
        if (rb == null)
=======
        if (playerControls.FindAction("Tilt") == null) tiltAction = playerControls.AddAction("Tilt", binding: "<Accelerometer>/acceleration/x");
    }

    void HandleSwipe(Vector2 swipeDirection)
    {
        // print("Screen Swiped");

        if (swipeDirection.y == 1) Jump();
        else if (swipeDirection.y == -1) Slide();
    }

    void DetectTilt()
    {
        if (Mathf.Abs(currentTilt) > 0.3f)
        {
            print(currentTilt);
            ShiftHorizontally(currentTilt);
        }
    }

    void MovePlayer()
    {
        shiftVelocity = 0;

        // Detect Phone Tilt
        DetectTilt();

        // Detect Keyboard Horizontal Input (Band-aid solution, bear with me please)
        if (currentKeyboardShift != 0) ShiftHorizontally(currentKeyboardShift);

        // Move Player
        rb.linearVelocity = new Vector3(shiftVelocity, rb.linearVelocity.y, speed);
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

    void ShiftHorizontally(float direction)
    {
        if ((transform.position.x > -pathWidth || direction > 0) && (transform.position.x < pathWidth || direction < 0)) shiftVelocity = direction * moveSpeed;

        // ✅ Adjust animations for left/right movement
        if (direction > 0)
>>>>>>> Stashed changes
        {
            Debug.LogError("❌ Rigidbody not found on player!");
            return;
        }

<<<<<<< Updated upstream
        // Constant forward movement (running)
        rb.AddRelativeForce(Vector3.forward * speed - rb.linearVelocity);  // Thank you https://discussions.unity.com/t/moving-a-rigidbody-at-a-constant-speed/435417/9
=======
        Invoke(nameof(ResetTurnAnimations), 0.3f);
    }
>>>>>>> Stashed changes

        lanePosition.y = transform.position.y;
        lanePosition.z = transform.position.z;

        rb.Move(lanePosition, Quaternion.identity);
    }

    void Jump()
    {
        if (IsGrounded())
        {
            print("Jumped");

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // ✅ Use Rigidbody physics
            animator.SetBool("isJumping", true);
        }
    }

<<<<<<< Updated upstream
    void Slide()
    {
        if (!isSliding)
        {
            print("Sliding");

            isSliding = true;
            animator.SetTrigger("isSliding");
            Invoke(nameof(StopSliding), 1f);
        }
    }

    void StopSliding() { isSliding = false; }

    // Turn currently cannot work with how lane changes are done & forward movement is calculated
    // Other functions will need to be refactored/changed if we are set on implementing it
    // Also, we don't have any corners to turn around yet anyways
    void Turn(float direction) 
    {
        if (!justTurned)
        {
            if (direction == -1)
            {
                print("Turning Left");

                animator.SetTrigger("isTurningLeft");
                transform.Rotate(Vector3.up, -90f);
            }
            else if (direction == 1)
            {
                print("Turning Right");

                animator.SetTrigger("isTurningRight");
                transform.Rotate(Vector3.up, 90f);
            }

            justTurned = true;
        }
    }

    void ChangeLane(float direction)
    {
        if ((direction == -1 && currentLane > -1) || (direction == 1 && currentLane < 1))
        {
            print("Changed Lane");
            
            currentLane += (int)direction;
            
            Vector3 newPosition = transform.position;
            newPosition.x = currentLane * laneDistance;

            lanePosition = newPosition;
        }
    }

=======
>>>>>>> Stashed changes
    void Die()
    {
        Debug.Log("❌ Player died! Saving score & transitioning to leaderboard...");
        // SceneManager.LoadScene("Leaderboard");

<<<<<<< Updated upstream
        SceneManager.LoadScene("MainMenu");  // Redirects to the Main Menu for now
    }

    // Thank you https://discussions.unity.com/t/how-can-i-check-if-my-rigidbody-player-is-grounded/256346, this is very nice
    bool IsGrounded() { return Physics.Raycast(transform.position, -Vector3.up, 0.6f); }
=======
        SceneManager.LoadScene("MainMenu");  // Redirects to the Main Menu, for now
    }

    // Thank you https://discussions.unity.com/t/how-can-i-check-if-my-rigidbody-player-is-grounded/256346, this is very nice
    bool IsGrounded() { return Physics.Raycast(transform.position, Vector3.down, 0.6f); }
>>>>>>> Stashed changes
}
