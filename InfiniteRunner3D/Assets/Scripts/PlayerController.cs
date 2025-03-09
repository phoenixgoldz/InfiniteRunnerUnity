using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;
    private Vector3 lanePosition;
    private bool isMoving = false; // ✅ Movement starts after touching the path

    public float speed = 10f;
    public float turnSpeed = 5f;
    public float jumpForce = 10f;
    public float laneDistance = 3f;

    private int currentLane = 0;
    private bool isJumping = false;
    private bool isSliding = false;
    private bool justTurned = false;
    private Vector2 startTouchPosition, swipeDelta;

    [SerializeField, Space(10)] private InputActionMap playerControls;

    // Keyboard Controls
    private InputAction jumpAction;
    private InputAction slideAction;
    private InputAction turnAction;
    private InputAction laneShiftAction;

    // Touch Controls
    private InputAction tiltAction;
    private float currentTilt => tiltAction.ReadValue<float>();


    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        playerControls = new InputActionMap();

        // Prevents Constraint Issues (hopefully)
        // Thank you https://discussions.unity.com/t/rigibody-constraints-do-not-work-still-moves-a-little/205580, you are amazing
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
        SwipeDetection.instance.swipePerformed -= context => HandleSwipe(context);
        #endif

        playerControls.Disable();
    }

    void FixedUpdate() //  Use FixedUpdate for physics-based movement
    {
        // (justTurned) justTurned = false;

        if (isMoving)
        {
            #if UNITY_ANDROID || UNITY_IOS
            DetectTilt();
            #endif

            // Move Player forward
            MovePlayer();
        }

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
        else
        {
            Debug.Log($"▶️ Player touched: {collision.gameObject.name} | Tag: {collision.gameObject.tag}"); // Debug log
        }
    }


    private void CreateKeyboardControls()
    {
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
    }

    private void AddMobileControls()
    {
        SwipeDetection.instance.swipePerformed += context => HandleSwipe(context);

        // Enable Gyro Sensors
        // Thank you https://discussions.unity.com/t/tutorial-for-input-system-and-accelerometer-gyro/790202/2, extremely appreciated
        if (UnityEngine.InputSystem.Gyroscope.current != null) InputSystem.EnableDevice(UnityEngine.InputSystem.Gyroscope.current);

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
        {
            Debug.LogError("❌ Rigidbody not found on player!");
            return;
        }

        // Constant forward movement (running)
        rb.AddRelativeForce(Vector3.forward * speed - rb.linearVelocity);  // Thank you https://discussions.unity.com/t/moving-a-rigidbody-at-a-constant-speed/435417/9

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

    void Die()
    {
        Debug.Log("❌ Player died! Saving score & transitioning to leaderboard...");
        // SceneManager.LoadScene("Leaderboard");

        SceneManager.LoadScene("MainMenu");  // Redirects to the Main Menu for now
    }

    // Thank you https://discussions.unity.com/t/how-can-i-check-if-my-rigidbody-player-is-grounded/256346, this is very nice
    bool IsGrounded() { return Physics.Raycast(transform.position, -Vector3.up, 0.6f); }
}
