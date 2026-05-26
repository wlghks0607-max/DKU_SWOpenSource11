using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpPower = 12f;

    [Header("바닥 체크")]
    public Transform groundCheck;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.12f);
    public LayerMask groundLayer;

    [Header("점프 보정")]
    public float coyoteTime = 0.12f;
    public float jumpBufferTime = 0.12f;

    [Header("중력 가속도 설정")]
    public float fallMultiplier = 4f;
    public float lowJumpMultiplier = 2.5f;

    private Rigidbody2D rb;
    private float moveInput;
    private float coyoteCounter;
    private float jumpBufferCounter;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        bool isGrounded = Physics2D.OverlapBox(
            groundCheck.position,
            groundCheckSize,
            0f,
            groundLayer
        );

        if (isGrounded)
            coyoteCounter = coyoteTime;
        else
            coyoteCounter -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            jumpBufferCounter = 0f;
            coyoteCounter = 0f;
        }

        ModifyGravity();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    void ModifyGravity()
    {
        // 점프 후 최고점 도달했을때 
        if (rb.linearVelocity.y < 0) 
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        // 위로 상승 & 스페이스 똈을때 
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space)) 
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }

    
}