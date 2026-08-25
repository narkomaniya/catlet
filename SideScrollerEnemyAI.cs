using UnityEngine;

public class SideScrollerEnemyAI : MonoBehaviour
{
    public float speed = 2f;
    public float moveDistance = 3f; // Дистанция патруля в стороны от точки спавна
    private float startX;
    private int direction = 1;

    public Transform player;
    public float aggroRange = 5f; // Дистанция, на которой видит игрока
    public float attackRange = 1.2f; // Дистанция удара

    private Rigidbody2D rb;

    void Start()
    {
        startX = transform.position.x;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            // Ближний бой / атака
            Attack();
        }
        else if (distanceToPlayer <= aggroRange)
        {
            // Погоня за игроком по оси X (высоту не трогаем, чтобы не летал)
            float targetX = Mathf.MoveTowards(transform.position.x, player.position.x, speed * Time.deltaTime);
            transform.position = new Vector3(targetX, transform.position.y, transform.position.z);
            
            // Разворачиваем морду в сторону игрока (опционально)
            if (player.position.x > transform.position.x)
                direction = 1;
            else
                direction = -1;
        }
        else
        {
            // Патрулирование туда-сюда по платформе
            float newX = transform.position.x + direction * speed * Time.deltaTime;
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);

            // Если отошел слишком далеко от стартовой точки — разворачиваемся
            if (Mathf.Abs(transform.position.x - startX) >= moveDistance)
            {
                direction *= -1;
            }
        }
    }

    void Attack()
    {
        Debug.Log("Враг бьет сбоку!");
    }
}
