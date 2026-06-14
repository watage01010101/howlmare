using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public partial class PlayerController : MonoBehaviour
{
    private const string GroundTag = "Ground";
    private const float StopInputThreshold = 0.01f;
    private const float JumpCutMultiplier = 0.5f;
    private const float AirJumpMultiplier = 0.9f;
    private const float HoldJumpBoost = 10f;
    private const float WallJumpHorizontalForce = 8f;
    private const float WallSlideSpeed = -2f;
    private const float AirHoldDecelerationMultiplier = 0.1f;
    private const float GroundNormalThreshold = 0.5f;
    private const float CeilingNormalThreshold = -0.5f;
    private const float WallNormalThreshold = 0.1f;

    [Header("Movement")]
    public float speed = 5f;
    public float runSpeed = 8f;
    public float acceleration = 50f;
    public float deceleration = 25f;
    [SerializeField] private float airControlMultiplier = 0.6f;

    [Header("Jump")]
    public float jumpForce = 18f;
    [SerializeField] private float groundCoyoteTime = 0.1f;
    [SerializeField] private float wallCoyoteTime = 0.1f;

    [Header("Run Input")]
    public float doubleTapTime = 0.2f;
    public float runGraceTime = 0.05f;

    private Rigidbody2D rb;
    private float moveInput;
    private bool jumpRequest;
    private bool jumpReleased;

    private bool isRunning;
    private float lastTapTime;
    private int lastDirection;
    private float lastMoveTime;

    private bool isGrounded;
    private bool canAirJump;
    private bool isTouchingWall;
    private int wallDirection;
    private float wallCoyoteTimer;
    private float groundCoyoteTimer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        isGrounded = true;
        canAirJump = true;
    }

    private void Update()
    {
        HandleInput();

        if (Input.GetKeyUp(KeyCode.Space))
        {
            jumpReleased = true;
        }
    }

    private void FixedUpdate()
    {
        Move();
        Jump();
        UpdateCoyoteTimers();
        ApplyJumpCut();
        ApplyBetterFall();
    }
}
