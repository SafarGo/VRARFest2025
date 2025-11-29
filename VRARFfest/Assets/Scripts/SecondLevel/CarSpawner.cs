using System.Collections;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public GameObject carPrefab;

    public Transform[] spawnPoints;
    public float spawnInterval;
    public float min_int;
    public float max_int;

    public float minSpawnInterval = 0.5f;
    public float timeToReachMinInterval = 60f;
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
            if (PerekrestokLevelController.IsGameStarted && !PerekrestokLevelController.IsGameEnded)
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
