using UnityEngine;

public class Bita : MonoBehaviour
{
    // --- ПУБЛИЧНОЕ ПОЛЕ ДЛЯ НАЗНАЧЕНИЯ ЗВУКА В ИНСПЕКТОРЕ ---
    [Tooltip("Звуковой клип для проигрывания при отбитии мяча (отбил.mp3).")]
    public AudioClip deflectionSound;
    // --------------------------------------------------------

    private CastleDefenseGame _gameManager;

    void Start()
    {
        // Находим экземпляр CastleDefenseGame на сцене один раз при старте
        _gameManager = FindObjectOfType<CastleDefenseGame>();
        if (_gameManager == null)
        {
            Debug.LogError("CastleDefenseGame не найден на сцене! Невозможно получить доступ к ScoreManager.");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            // Увеличиваем счет
            // (Предполагается, что ScoreManager существует)
            if (_gameManager != null)
            {
                ScoreManager.score += 10;
                Debug.Log(ScoreManager.score);
                collision.gameObject.tag = "";
            }

            // --- ВОСПРОИЗВЕДЕНИЕ ЗВУКА "ОТБИЛ" ---
            if (deflectionSound != null)
            {
                // PlayClipAtPoint идеально подходит: проигрывает звук в 3D-пространстве и сам удаляет AudioSource
                AudioSource.PlayClipAtPoint(deflectionSound, collision.contacts[0].point, 1f);
            }
            // ---------------------------------------------

            // Дополнительная логика: уничтожение мяча, изменение вектора и т.д.
        }
    }
}