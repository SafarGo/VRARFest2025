using System.Collections;
using UnityEngine;

/// <summary>
/// Простой спавнер машин на перекрестке
/// </summary>
public class CarSpawner : MonoBehaviour
{
    [Header("Настройки")]
    [Tooltip("Префаб машины для спавна")]
    public GameObject carPrefab;

    [Tooltip("Точки спавна машин (Transform)")]
    public Transform[] spawnPoints;

    [Tooltip("Интервал между спавном машин (секунды)")]
    public float spawnInterval;
    public float min_int;
    public float max_int;

    [Tooltip("Минимальный интервал спавна (до которого будет уменьшаться)")]
    public float minSpawnInterval = 0.5f;

    [Tooltip("Время (в секундах) за которое интервал достигнет минимума")]
    public float timeToReachMinInterval = 60f;

    [Tooltip("Скорость машин")]
    public float carSpeed = 10f;

    private float initialSpawnInterval;
    private float startTime;

    void Start()
    {
        // Начинаем спавн
        spawnInterval = Random.Range(min_int, max_int);
        initialSpawnInterval = spawnInterval;
        startTime = Time.time;
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // Вычисляем прошедшее время с начала игры
            float elapsedTime = Time.time - startTime;
            
            // Плавно уменьшаем интервал со временем
            float progress = Mathf.Clamp01(elapsedTime / timeToReachMinInterval);
            spawnInterval = Mathf.Lerp(initialSpawnInterval, minSpawnInterval, progress);
            
            // Ждем интервал
            yield return new WaitForSeconds(spawnInterval);

            // Выбираем случайную точку спавна
            if (spawnPoints.Length > 0 && carPrefab != null)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                
                // Спавним машину
                GameObject car = Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation);
                
                // Добавляем AutoCarController если его нет
                AutoCarController controller = car.GetComponent<AutoCarController>();
                if (controller == null)
                {
                    controller = car.AddComponent<AutoCarController>();
                }
                
                // Устанавливаем скорость
                controller.moveSpeed = carSpeed;
            }
        }
    }
}
