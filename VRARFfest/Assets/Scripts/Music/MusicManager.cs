using UnityEngine;
using System.Collections;

// Этот скрипт должен висеть на пустом объекте на сцене
public class MusicManager : MonoBehaviour
{
    // Singleton для доступа из любой точки
    public static MusicManager Instance { get; private set; }

    [Header("Audio Sources")]
    // Две дорожки для кроссфейда
    public AudioSource source1;
    public AudioSource source2;

    [Header("Settings")]
    // Скорость, с которой происходит затухание/нарастание громкости
    public float fadeDuration = 2.0f;

    // Текущий активный AudioSource
    private AudioSource _activeSource;
    // Источник, который в данный момент затухает
    private AudioSource _inactiveSource;

    private void Awake()
    {
        // Настройка Singleton
        if (Instance == null)
        {
            Instance = this;
            // Убедимся, что источники существуют
            if (source1 == null || source2 == null)
            {
                Debug.LogError("MusicManager requires two AudioSource components assigned.");
            }
            // Инициализация громкости
            source1.volume = 0f;
            source2.volume = 0f;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Запускает плавный переход к новому музыкальному клипу.
    /// </summary>
    /// <param name="newClip">Новый аудиоклип для воспроизведения.</param>
    public void CrossfadeTo(AudioClip newClip)
    {
        if (_activeSource != null && _activeSource.clip == newClip)
        {
            // Если этот клип уже играет, ничего не делаем
            return;
        }

        // 1. Определяем, какой источник станет новым активным
        AudioSource newActiveSource = (_activeSource == source1) ? source2 : source1;
        AudioSource newInactiveSource = _activeSource;

        // 2. Настраиваем новый активный источник
        newActiveSource.clip = newClip;

        // Если предыдущий источник уже был установлен, останавливаем его coroutine,
        // чтобы избежать конфликтов затухания.
        if (newInactiveSource != null)
        {
            StopAllCoroutines();
        }

        // 3. Запускаем кроссфейд
        StartCoroutine(PerformCrossfade(newActiveSource, newInactiveSource));

        // Обновляем ссылки
        _activeSource = newActiveSource;
        _inactiveSource = newInactiveSource;
    }

    private IEnumerator PerformCrossfade(AudioSource fadeInSource, AudioSource fadeOutSource)
    {
        // Если fadeOutSource == null, это первое воспроизведение
        if (fadeOutSource != null)
        {
            // Начинаем воспроизведение нового клипа, если он еще не играет
            if (!fadeInSource.isPlaying)
            {
                fadeInSource.Play();
            }

            float currentTime = 0;
            float startVolume = (fadeOutSource != null) ? fadeOutSource.volume : 0f;

            // Запускаем цикл кроссфейда
            while (currentTime < fadeDuration)
            {
                currentTime += Time.deltaTime;
                float t = currentTime / fadeDuration;

                // Нарастание громкости
                fadeInSource.volume = Mathf.Lerp(0f, 1f, t);

                // Затухание громкости (если есть что затушить)
                if (fadeOutSource != null)
                {
                    fadeOutSource.volume = Mathf.Lerp(startVolume, 0f, t);
                }

                yield return null;
            }

            // Убеждаемся, что громкость установлена точно
            fadeInSource.volume = 1f;
            if (fadeOutSource != null)
            {
                fadeOutSource.volume = 0f;
                // Останавливаем старый клип, чтобы он не потреблял ресурсы
                fadeOutSource.Stop();
            }
        }
        else // Если это первый запуск музыки
        {
            fadeInSource.volume = 0f;
            fadeInSource.Play();

            float currentTime = 0;
            while (currentTime < fadeDuration)
            {
                currentTime += Time.deltaTime;
                float t = currentTime / fadeDuration;
                fadeInSource.volume = Mathf.Lerp(0f, 1f, t);
                yield return null;
            }
            fadeInSource.volume = 1f;
        }
    }
}