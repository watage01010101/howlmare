using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // ===== 移動設定 =====
    public float speed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 18f;
    public float doubleTapTime = 0.2f;
    public float runGraceTime = 0.05f; // 猶予時間
    public float acceleration = 50f;   // 加速の速さ
    public float deceleration = 25f;   // 減速の速さ
    // public int maxJumpCount = 1;      // 空中ジャンプ回数
    private int wallDirection; // 壁の方向（右=1、左=-1）
    [SerializeField] private float airControlMultiplier = 0.6f; // 空中での操作の効き具合（0.3〜0.8くらい）

    // ===== 内部状態 =====
    private Rigidbody2D rb;
    private float moveInput;        // 左右入力値（-1〜1）
    private bool jumpRequest;       // ジャンプ入力を保持（FixedUpdate用）
    private bool isRunning;         // ダッシュ状態
    private float lastTapTime;      // 最後にキーを押した時間
    private int lastDirection;      // 最後の入力方向
    private float lastMoveTime;     // 最後に移動入力があった時間

    // private int jumpCount;
    private bool isGrounded;        // 地面にいるか
    private bool canAirJump;        // 空中ジャンプ可能か
    private bool isTouchingWall;    // 壁に接触しているか（未使用）
    private bool jumpReleased;      // ジャンプボタンを離したか
    private float wallCoyoteTime = 0.1f; // 壁猶予時間（0.05〜0.15くらい）
    private float wallCoyoteTimer;
    private float groundCoyoteTime = 0.1f;
    private float groundCoyoteTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // jumpCount = maxJumpCount;

        isGrounded = true;  // 初期状態では地面にいる想定
        canAirJump = true;  // 空中ジャンプ可能にしておく
    }

    void Update()
    {
        HandleInput();

        // =========================
        // ミニジャンプ用
        // =========================
        if (Input.GetKeyUp(KeyCode.Space))
        {
            jumpReleased = true; // 離した瞬間を記録（後で高さカット）
        }
    }

    void FixedUpdate()
    {
        Move();
        Jump();

//壁ジャン地面ジャン競合阻止
if (groundCoyoteTimer > 0)
{
    groundCoyoteTimer -= Time.fixedDeltaTime;
}

        // 壁猶予タイマー減少
        if (wallCoyoteTimer > 0)
        {
            wallCoyoteTimer -= Time.fixedDeltaTime;
        }

        // ミニジャンプ処理（上昇中に離したら高さをカット）
        if (jumpReleased)
        {
            if (rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(
                    rb.linearVelocity.x,
                    rb.linearVelocity.y * 0.5f // 上昇速度を半分に
                );
            }
            jumpReleased = false; // 一度だけ実行
        }

        ApplyBetterFall();
    }

    // =========================
    // 入力処理
    // =========================
    void HandleInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // 入力があった時間を記録（ダッシュ解除用）
        if (moveInput != 0)
        {
            lastMoveTime = Time.time;
        }

        // ダブルタップ判定
        int direction = 0;
        if (Input.GetKeyDown(KeyCode.A)) direction = -1;
        if (Input.GetKeyDown(KeyCode.D)) direction = 1;

        if (direction != 0)
        {
            // 同じ方向を短時間で2回押すとダッシュ
            if (direction == lastDirection &&
                Time.time - lastTapTime < doubleTapTime)
            {
                isRunning = true;
            }

            lastDirection = direction;
            lastTapTime = Time.time;
        }
// ★ Ctrl状態
bool isCtrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

// ★ ダブルタップ状態を一時保存
bool doubleTapRunning = isRunning;

// ★ 一度リセット（これが超重要）
isRunning = false;

// ★ Ctrl優先
if (isCtrl)
{
    isRunning = true;
}

// ★ Ctrlを離した瞬間、ダブルタップ状態をリセット
if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))
{
    isRunning = false;
}

// ★ ダブルタップ
else if (doubleTapRunning)
{
    isRunning = true;
}

// ★ ダブルタップの解除
if (!isCtrl && Time.time - lastMoveTime > runGraceTime)
{
    isRunning = false;
}

        // ジャンプ入力を保存
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpRequest = true;
        }
    }

    // =========================
    // 横移動
    // =========================
    void Move()
    {
        float currentSpeed = isRunning ? runSpeed : speed;

        // 目標の横速度
        float targetSpeed = moveInput * currentSpeed;

        // 現在の横速度
        float currentVelocityX = rb.linearVelocity.x;

// 入力がある時は加速、無い時は減速
bool isStopping = Mathf.Abs(targetSpeed) < 0.01f;

float accelRate;

if (isStopping)
{
    float decel = deceleration;

    // ★ 空中かつスペース押してないとき → 強く減速
    if (!isGrounded && !Input.GetKey(KeyCode.Space))
    {
        decel *= 1f; // ← 強い抵抗
    }
    // ★ 空中かつスペース押してるとき → 弱い減速
    else if (!isGrounded && Input.GetKey(KeyCode.Space))
    {
        decel *= 0.1f; // ← ほぼ慣性
    }

    accelRate = decel;
}
else
{
    accelRate = acceleration;
}

// ★ 空中では操作を弱める
if (!isGrounded)
{
    accelRate *= airControlMultiplier;
}

        // 徐々に目標速度に近づける
        float newVelocityX = Mathf.MoveTowards(
            currentVelocityX,
            targetSpeed,
            accelRate * Time.fixedDeltaTime
        );

        // 空中での速度維持用（※今は使われていない値）
        //float finalX = newVelocityX;

        //if (!isGrounded && Input.GetKey(KeyCode.Space))
        //{
        //   finalX = rb.linearVelocity.x * 1.01f; // わずかに速度を維持・増加
        //}

        // 実際に適用している速度（ここは newVelocityX を使っている）
        rb.linearVelocity = new Vector2(newVelocityX, rb.linearVelocity.y);

        // キャラの向きを反転
        float scaleX = Mathf.Abs(transform.localScale.x);

        if (moveInput > 0)
        {
            transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-scaleX, transform.localScale.y, transform.localScale.z);
        }
    }

    // =========================
    // ジャンプ
    // =========================
void Jump()
{
    if (!jumpRequest) return;

    // =========================
    // 壁ジャンプ（最優先）
    // =========================
    if (!isGrounded && isTouchingWall && rb.linearVelocity.y < 0f)
    {
        WallJump();
    }
    // =========================
    // 地上ジャンプ
    // =========================
    else if (isGrounded)
    {
        GroundJump();
    }
    // =========================
    // 空中ジャンプ
    // =========================
    else if (canAirJump)
    {
        AirJump();
    }

    jumpRequest = false;

    // 長押しで上昇を強化
    if (Input.GetKey(KeyCode.Space) &&
        rb.linearVelocity.y > 0 &&
        !jumpReleased)
    {
        rb.linearVelocity += Vector2.up * 10f * Time.fixedDeltaTime;
    }
}

    // =========================
    // 落下を速くする
    // =========================
void ApplyBetterFall()
{
 
    // 壁スライド（優先）

    if (isTouchingWall && !isGrounded && rb.linearVelocity.y < 0)
    {
        // 壁に沿ってゆっくり落ちる
        float slideSpeed = -2f; // ← 調整ポイント（-1〜-3くらいが自然）

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, slideSpeed);
        return;
    }

    // =========================
    // 通常落下
    // =========================
    if (!isGrounded && rb.linearVelocity.y < 0)
    {
        rb.linearVelocity += Vector2.up *
                             Physics2D.gravity.y *
                             1f *
                             Time.fixedDeltaTime;
    }
}

    // =========================
    // 地上ジャンプ
    // =========================
    void GroundJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        canAirJump = true; // 空中ジャンプをリセット
    }

    // =========================
    // 空中ジャンプ
    // =========================
    void AirJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 0.9f);
        canAirJump = false; // 1回だけに制限
    }


//接触判定
void OnCollisionStay2D(Collision2D collision)
{
    if (!collision.gameObject.CompareTag("Ground")) return;

    // 毎フレーム初期化（超重要）
    isGrounded = false;
    isTouchingWall = false;

    foreach (ContactPoint2D contact in collision.contacts)
    {
        // 地面
     if (contact.normal.y > 0.5f)
{
    isGrounded = true;
    groundCoyoteTimer = groundCoyoteTime; // ← 追加

    canAirJump = true;

    if (rb.linearVelocity.y < 0)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
    }
}
        // 天井
        else if (contact.normal.y < -0.5f)
        {
            if (rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            }
        }
        // 壁
        else if (Mathf.Abs(contact.normal.x) > 0.1f)
        {
            int detectedWallDir = (contact.normal.x > 0) ? -1 : 1;

            // ★ 向き取得（右=1、左=-1）
            int facingDir = (transform.localScale.x > 0) ? 1 : -1;

            // ★ 正面の壁だけ有効
            if (facingDir == detectedWallDir)
                {
                    isTouchingWall = true;
                    wallDirection = detectedWallDir;
                    wallCoyoteTimer = wallCoyoteTime;
                }
        }
    }
}

//接触exit
void OnCollisionExit2D(Collision2D collision)
{
    if (collision.gameObject.CompareTag("Ground"))
    {
        isGrounded = false;
        isTouchingWall = false;
    }
}

//壁ジャンプ
void WallJump()
{
    float jumpX = -wallDirection * 8f; // 横方向（強め推奨）
    float jumpY = jumpForce;

    rb.linearVelocity = new Vector2(jumpX, jumpY);

    isTouchingWall = false;
    canAirJump = true;
}



}