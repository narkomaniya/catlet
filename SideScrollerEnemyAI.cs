using UnityEngine;

public class PlatformEnemy : MonoBehaviour
{
    public float speed = 2f;
    public float platformCenterX = 29.87f;
    public float platformHalfWidth = 1.0f;
    private int direction = 1;

    public Transform player;
    public float aggroRange = 2.5f;
    public float attackRange = 1.0f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.freezeRotation = true;
    }

    void Update()
    {
        if (player == null || rb == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (transform.position.x > platformCenterX + platformHalfWidth) direction = -1;
        else if (transform.position.x < platformCenterX - platformHalfWidth) direction = 1;

        if (dist <= attackRange)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else if (dist <= aggroRange)
        {
            direction = player.position.x > transform.position.x ? 1 : -1;
            rb.velocity = new Vector2(direction * speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(direction * speed, rb.velocity.y);
            if (Mathf.Abs(transform.position.x - platformCenterX) >= platformHalfWidth) direction *= -1;
        }
    }
}
