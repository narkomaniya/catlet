using UnityEngine;
using UnityEngine.SceneManagement; // Нужно для переключения сцен

public class HouseEntrance : MonoBehaviour
{
    [Header("Настройки перехода")]
    public string sceneToLoad; // Впиши сюда название сцены с интерьером
    public Animator doorAnimator; // Сюда закинь аниматор двери (если есть)

    private bool isNearDoor = false;

    void Update()
    {
        // Если игрок стоит у двери и жмет 'E'
        if (isNearDoor && Input.GetKeyDown(KeyCode.E))
        {
            EnterHouse();
        }
    }

    // Когда кто-то заходит в зону двери
    void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, что это именно наш ебучий игрок, а не враг или пуля
        if (other.CompareTag("Player"))
        {
            isNearDoor = true;
            // Тут можно включить UI-подсказку "Нажми E, чтобы войти"
        }
    }

    // Когда игрок отходит от двери
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isNearDoor = false;
        }
    }

    void EnterHouse()
    {
        // Если брат уже нарисовал и подключил анимацию открытия
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Open"); // Запускаем анимацию
            // Ждем полсекунды, чтобы дверь успела открыться, и грузим сцену
            Invoke("LoadRoom", 0.5f); 
        }
        else
        {
            // Если анимации пока нет, грузим комнату моментально, хуле
            LoadRoom();
        }
    }

    void LoadRoom()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
