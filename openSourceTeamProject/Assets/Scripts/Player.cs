using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{


    public TMP_Text hpText;
    public GameObject gameOverUI;

    private Vector3 initialPosition;
    private SpriteRenderer sp;
    private Rigidbody2D rigid;

    public float moveSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpPower = 12f;
    private bool isKnockback = false;

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

    [Header("목숨& 부활 설정")]
    public int hp = 4;
    private bool isDead = false;

    private Rigidbody2D rb;
    private Animator anim;

    private float moveInput;
    private float currentSpeed;
    private float coyoteCounter;
    private float jumpBufferCounter;
    private Vector3 originalScale;
    private bool isDamaged = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
        currentSpeed = moveSpeed;

        if (gameOverUI != null)
            gameOverUI.SetActive(false);
        
        LifeCount();
        initialPosition = transform.position;
    }

    void Update()
    {
        if (isDead) return;

        if (hp <= 0)
        {
            Die();
            return;
        }

        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKey(KeyCode.LeftShift))
            currentSpeed = runSpeed;
        else
            currentSpeed = moveSpeed;

        anim.SetBool("isRun", moveInput != 0);

        if (moveInput > 0)
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);

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
        if (isDead) return;
        if (isKnockback) return;

        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
    }

    public void Die()
    {
        if (isDead) return;

        hp -= 1;
        LifeCount();
        
        if (hp > 0){
            transform.position = initialPosition;
            rb.linearVelocity = Vector2.zero;
        }        
        else 
        {
            isDead = true;
            moveInput = 0f;

            anim.SetBool("isRun", false);
            anim.SetTrigger("Die");

            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;

            if (gameOverUI != null)
                gameOverUI.SetActive(true);

            Debug.Log("Player Dead");
        }
        
    }

    void LifeCount() 
    {
        if (hpText != null)
        {
            hpText.text = ""+hp;
        }
    }

    void ModifyGravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
          OnDamaged(collision.transform.position);
        }
    }

    void OnDamaged(Vector2 targetPos)
    {
        if (isDead) return;

        hp -= 1;
        LifeCount();
        
        if (hp <= 0){
            isDead = true;
            moveInput = 0f;

            anim.SetBool("isRun", false);
            anim.SetTrigger("Die");

            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;

            if (gameOverUI != null)
                gameOverUI.SetActive(true);

            Debug.Log("Player Dead");
        }

        isDamaged = true;
        isKnockback = true;

        gameObject.layer = 8;
        sp.color = new Color(1, 1, 1, 0.4f);

        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(dirc * 8f, 3f), ForceMode2D.Impulse);

        Invoke("OffKnockback", 0.2f);
        Invoke("offDamaged", 1.5f);
    }

    void offDamaged()
    {
        isDamaged = false;
        gameObject.layer = 7;
        sp.color = new Color(1, 1, 1, 1);
    }

    void OffKnockback()
    {
        isKnockback = false;
    }
}