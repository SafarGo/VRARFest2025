using UnityEngine;

public class DoorController : MonoBehaviour
{
    // Задаем время задержки прямо здесь
    public float startDelayTime = 3f;

    // Имя метода, который будет вызван с задержкой (должно совпадать с SetPlayerInRoomTrue в CastleDefenseGame)
    private const string StartMethodName = "SetPlayerInRoomTrue";

    // Ссылка на экземпляр менеджера игры
    private CastleDefenseGame _gameManager;

    void Start()
    {
        // Находим экземпляр менеджера на сцене
        _gameManager = FindObjectOfType<CastleDefenseGame>();
        if (_gameManager == null)
        {
            Debug.LogError("CastleDefenseGame Manager не найден на сцене! Скрипт DoorController не будет работать.");
        }
    }

    // Срабатывает, когда другой коллайдер входит в нашу триггерную зону
    private void OnTriggerEnter(Collider other)
    {
        // other - это коллайдер объекта, который вошел
        if (other.gameObject.CompareTag("Player"))
        {
            if (_gameManager != null)
            {
                // Отменяем любые предыдущие запросы, вызванные НА ОБЪЕКТЕ МЕНЕДЖЕРА
                _gameManager.CancelInvoke(StartMethodName);

                // Запускаем старт игры через 3 секунды НА ОБЪЕКТЕ МЕНЕДЖЕРА
                _gameManager.Invoke(StartMethodName, startDelayTime);

                Debug.Log($"Игрок вошел в зону. Запуск игры через {startDelayTime} секунд...");
            }
        }
    }

    // Срабатывает, когда другой коллайдер выходит из нашей триггерной зоны
    private void OnTriggerExit(Collider other)
    {
        // other - это коллайдер объекта, который вышел
        if (other.gameObject.CompareTag("Player"))
        {
            if (_gameManager != null)
            {
                // Отменяем запуск НА ОБЪЕКТЕ МЕНЕДЖЕРА
                _gameManager.CancelInvoke(StartMethodName);
            }

            // Сразу выключаем игру (CastleDefenseGame.isPlayerInRoom должен быть static)
            CastleDefenseGame.isPlayerInRoom = false;

            Debug.Log("Игрок покинул зону. Запуск отменен, CastleDefenseGame.isPlayerInRoom = false.");
        }
    }
}