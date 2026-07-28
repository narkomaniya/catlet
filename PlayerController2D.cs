using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))] 
public class PlayerController2D : MonoBehaviour
{
    public float moveSpeed = 8f;
    private float horizontalInput;
    private bool facingRight = true;

    public float jumpForce = 12f;
    [Range(0f, 1f)] public float jumpCutMultiplier = 0.5f; 
    public float fallGravityMultiplier = 1.5f; 
    private float defaultGravity;

    public LayerMask groundLayer; 
    // Сделать зону чуть уже персонажа (0.5f), чтобы не цеплять стены по бокам
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);
    public float groundCheckOffset = 0.05f; 
    
    public float jumpBufferTime = 0.1f;
    private float jumpBufferCounter;
    public float coyoteTime = 0.1f; 
    private float coyoteTimeCounter;
    private bool isGrounded;

    public bool enableCameraFollow = true; 
    public Vector3 cameraOffset = new Vector3(0f, 0f, -10f); 

    public bool useCameraBounds = true;
    public Vector2 minCamPos = new Vector2(-5f, -5f);
    public Vector2 maxCamPos = new Vector2(5f, 5f);

    private Rigidbody2D rb;
    private CapsuleCollider2D coll;
    private Transform mainCamTransform; 

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CapsuleCollider2D>();
    }

    void Start()
    {
        rb.freezeRotation = true; 
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        defaultGravity = rb.gravityScale;

        FindCamera();
    }

    void Update()
    {
        // Ходьба (A / D и Стрелочки)
        horizontalInput = 0f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) 
        {
            horizontalInput = 1f;
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) 
        {
            horizontalInput = -1f;
        }

        // Фикс бага прыжка: смещаем зону проверки строго НИЖЕ коллайдера и делаем ее уже
        Vector2 checkCenter = new Vector2(coll.bounds.center.x, coll.bounds.min.y - (groundCheckSize.y / 2f) - groundCheckOffset);
        isGrounded = Physics2D.OverlapBox(checkCenter, groundCheckSize, 0f, groundLayer);

        // Таймер прыжка при сходе с платформы (Coyote Time)
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Запись нажатия прыжка на Space, W и Стрелку вверх (Jump Buffer)
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Физическое совершение прыжка
        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpBufferCounter = 0f; 
            coyoteTimeCounter = 0f; 
        }

        // Срезка высоты прыжка при быстром отпускании кнопки
        if ((Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow)) && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutMultiplier);
            coyoteTimeCounter = 0f;
        }

        Flip();
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);

        // Ускоренное падение вниз для хорошего ощущения физики
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = defaultGravity * fallGravityMultiplier;
        }
        else
        {
            rb.gravityScale = defaultGravity;
        }
    }

    void LateUpdate()
    {
        if (mainCamTransform == null)
        {
            FindCamera();
        }

        if (enableCameraFollow && mainCamTransform != null)
        {
            Vector3 targetPos = transform.position + cameraOffset;

            if (useCameraBounds)
            {
                targetPos.x = Mathf.Clamp(targetPos.x, minCamPos.x, maxCamPos.x);
                targetPos.y = Mathf.Clamp(targetPos.y, minCamPos.y, maxCamPos.y);
            }

            mainCamTransform.position = targetPos;
        }
    }

    private void FindCamera()
    {
        if (Camera.main != null)
        {
            mainCamTransform = Camera.main.transform;
        }
    }

    private void Flip()
    {
        if (horizontalInput > 0 && !facingRight || horizontalInput < 0 && facingRight)
        {
            facingRight = !facingRight;
            Vector3 scaler = transform.localScale;
            scaler.x *= -1;
            transform.localScale = scaler;
        }
    }

    private void OnDrawGizmosSelected()
    {
        CapsuleCollider2D c = GetComponent<CapsuleCollider2D>();
        if (c != null)
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Vector2 checkCenter = new Vector2(c.bounds.center.x, c.bounds.min.y - (groundCheckSize.y / 2f) - groundCheckOffset);
            Gizmos.DrawCube(checkCenter, groundCheckSize);
        }

        if (useCameraBounds)
        {
            Gizmos.color = Color.green;
            Vector2 center = (minCamPos + maxCamPos) / 2f;
            Vector2 size = maxCamPos - minCamPos;
            Gizmos.DrawWireCube(center, size);
        }
    }
}
