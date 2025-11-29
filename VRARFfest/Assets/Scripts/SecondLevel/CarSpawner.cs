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

        spawnInterval = Random.Range(min_int, max_int);
        initialSpawnInterval = spawnInterval;
        startTime = Time.time;
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (PerekrestokLevelController.IsGameStarted)
            {
                float elapsedTime = Time.time - startTime;
                float progress = Mathf.Clamp01(elapsedTime / timeToReachMinInterval);

                float currentSpawnInterval = Mathf.Lerp(initialSpawnInterval, minSpawnInterval, progress);

                yield return new WaitForSeconds(currentSpawnInterval);

                if (spawnPoints.Length > 0 && carPrefab != null)
                {
                    Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                    GameObject car = Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation);

                    AutoCarController controller = car.GetComponent<AutoCarController>();
                    if (controller == null)
                    {
                        controller = car.AddComponent<AutoCarController>();
                    }
                    controller.moveSpeed = carSpeed;
                }
            }
            else
            {
                yield return null;
            }
        }
    }
}
