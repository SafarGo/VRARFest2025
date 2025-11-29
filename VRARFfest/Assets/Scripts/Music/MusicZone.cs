using UnityEngine;

// Этот скрипт вешается на Collider с Is Trigger = true
public class MusicZone : MonoBehaviour
{
    [Tooltip("Аудиоклип, который должен начать играть при входе в эту зону.")]
    public AudioClip zoneMusic;

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что вошел именно игрок. Убедитесь, что у игрока есть тег "Player".
        if (other.CompareTag("Player"))
        {
            // Убеждаемся, что менеджер существует и что клип назначен
            if (MusicManager.Instance != null && zoneMusic != null)
            {
                // Запускаем кроссфейд через менеджер
                MusicManager.Instance.CrossfadeTo(zoneMusic);
                Debug.Log($"Игрок вошел в зону {gameObject.name}. Запущен переход к музыке: {zoneMusic.name}");
            }
            else
            {
                Debug.LogWarning("MusicManager или AudioClip не найдены. Музыка не запущена.");
            }
        }
    }
}