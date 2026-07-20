using UnityEngine;

// Эти две строки автоматически добавляют физику и коллизию на объект в Unity
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))] 
public class PlayerController2D : MonoBehaviour
{
    [Header("Настройки движения")]
    public float moveSpeed = 8f;
    private float horizontalInput;
    private bool facingRight = true;

    [Header("Настройки прыжка")]
    public float jumpForce = 12f;
    public LayerMask groundLayer; // Слой земли для коллизии ног с платформой
    
    private float jumpBufferTime = 0.1f;
    private float jumpBufferCounter;
    private bool isGrounded;

    // Ссылки на компоненты физики и коллизии
    private Rigidbody2D rb;
    private CapsuleCollider2D playerCollider;

    void Start()
    {
        // Связываем код с компонентами на объекте
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<CapsuleCollider2D>();

        // Замораживаем вращение персонажа, чтобы коллизия со стенами не опрокидывала его
        rb.freezeRotation = true; 
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void Update()
    {
        // Считываем WASD (клавиши A/D)
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Буфер прыжка на Пробел
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Если нажали прыжок И коллизия подтверждает, что мы на земле — прыгаем
        if (jumpBufferCounter > 0 && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpBufferCounter = 0; 
        }

        Flip();
    }

    void FixedUpdate()
    {
        // Двигаем тело через физику. Коллизии стен будут автоматически останавливать персонажа
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
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

    // --- ОБРАБОТКА КОЛЛИЗИЙ (Встроено в Unity) ---
    
    // Этот метод вызывается автоматически ВСЁ ВРЕМЯ, пока коллайдер игрока касается другого коллайдера
    private void OnCollisionStay2D(Collision2D collision)
    {
        // Проверяем, что коснулись именно слоя Земли
        if ((groundLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            Bounds bounds = playerCollider.bounds;
            float rayLength = 0.1f; 

            // Пускаем лучи вниз, чтобы проверить, что коллизия происходит ИМЕННО под ногами
            bool hitLeft = Physics2D.Raycast(new Vector2(bounds.min.x + 0.05f, bounds.min.y), Vector2.down, rayLength, groundLayer);
            bool hitCenter = Physics2D.Raycast(new Vector2(bounds.center.x, bounds.min.y), Vector2.down, rayLength, groundLayer);
            bool hitRight = Physics2D.Raycast(new Vector2(bounds.max.x - 0.05f, bounds.min.y), Vector2.down, rayLength, groundLayer);

            if (hitLeft || hitCenter || hitRight)
            {
                isGrounded = true;
            }
        }
    }

    // Этот метод вызывается в момент, когда коллизия прекращается (мы оторвались от объекта)
    private void OnCollisionExit2D(Collision2D collision)
    {
        if ((groundLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            isGrounded = false;
        }
    }
}