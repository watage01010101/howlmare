using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public partial class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float runSpeed = 8f;
    public float acceleration = 50f;
    public float deceleration = 25f;
    [SerializeField] private float airControlMultiplier = 0.6f;

    [Header("Jump")]
    public float jumpForce = 18f;

    [Header("Run Input")]
    public float doubleTapTime = 0.2f;
    public float runGraceTime = 0.05f;

    [Header("Attack")]
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackRange = 0.8f;
    [SerializeField] private float attackOffset = 0.7f;
    [SerializeField] private LayerMask enemyLayer = ~0;

    private Rigidbody2D rb;
    private float moveInput;
    private bool jumpRequest;
    private bool jumpReleased;
    private bool isJumpHeld;

    private bool isRunning;
    private bool isDoubleTapRunning;
    private float lastTapTime;
    private int lastDirection;
    private float lastMoveTime;

    private bool isGrounded;
    private bool canAirJump;
    private bool isTouchingWall;
    private int wallDirection;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        isGrounded = true;
        canAirJump = true;
    }

    private void Update()
    {
        HandleInput();
        HandleAttack();
    }

    private void FixedUpdate()
    {
        Move();
        Jump();
        ApplyJumpCut();
        ApplyBetterFall();
    }
}
