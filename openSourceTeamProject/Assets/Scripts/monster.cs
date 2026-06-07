using UnityEngine;

public class Monster : MonoBehaviour
{
    private Rigidbody2D rigid;
    private Animator anim;
    CapsuleCollider2D capsuleCollider;
    private SpriteRenderer spriteRenderer;

    public int nextMove;
    private bool isDead = false;

    [Header("이동 속도")]
    public float moveSpeed = 2f;

    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        Think();
    }

    void FixedUpdate()
    {
        if (isDead) return;
        rigid.linearVelocity = new Vector2(nextMove * moveSpeed, rigid.linearVelocity.y);

        Vector2 frontVec = new Vector2(
            rigid.position.x + nextMove * 0.5f,
            rigid.position.y
        );

        Debug.DrawRay(frontVec, Vector3.down, Color.green);

        RaycastHit2D rayHit =
            Physics2D.Raycast(
            frontVec,
            Vector2.down,
            1f,
            LayerMask.GetMask("Ground")
        );
        if (rayHit.collider != null)
        {
            if(rayHit.distance < 0.5f)
                anim.SetBool("isJumping", false);
        }
        if (rayHit.collider == null)
        {
            nextMove *= -1;

            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    void Think()
    {
        if (isDead) return;
        // -1(왼쪽), 0(정지), 1(오른쪽)
        nextMove = Random.Range(-1, 2);

        // 애니메이션
        anim.SetBool("isMove", nextMove != 0);

        // 방향 전환
        if (nextMove != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x) * nextMove;
            transform.localScale = scale;
        }

        // 2~5초 후 다시 생각
        Invoke(nameof(Think), Random.Range(2f, 5f));
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    public void OnDamaged()
    {
        if (isDead) return;

        isDead = true;
        CancelInvoke();

        nextMove = 0;
        anim.SetBool("isMove", false);

        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        spriteRenderer.flipY = true;

        capsuleCollider.enabled = false;
        rigid.linearVelocity = Vector2.zero;
        rigid.AddForce(Vector2.up * 5f, ForceMode2D.Impulse);

        Invoke(nameof(DeActive), 1f);
    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }
}