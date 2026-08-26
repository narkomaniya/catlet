using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 6f;

    private Rigidbody rb;
    private bool isGrounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        MovePlayer();
        JumpPlayer();
    }

    void MovePlayer()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(moveX, 0f, moveZ).normalized * speed;
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
    }

    void JumpPlayer()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    // Проверяем именно то, на что наступаем снизу
    void OnCollisionStay(Collision col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint2D contact in col.contacts) // Если у вас 3D, то ContactPoint, но для 3D Unity использует ContactPoint
            {
                // Проверяем, что точка контакта снизу (нормаль смотрит вверх)
            }
            
            // Простой вариант без сложных нормалей: проверяем, что соприкосновение идет снизу
            foreach (ContactPoint contact in col.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    isGrounded = true;
                }
            }
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
