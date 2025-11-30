using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public GameObject carPrefab;
    public GameObject carPrefab1;

    public Transform[] spawnPoints;
    public float spawnInterval;
    public float min_int;
    public float max_int;

    public float minSpawnInterval = 0.3f;
    public float timeToReachMinInterval = 60f;
    public float carSpeed = 10f;

    private float initialSpawnInterval;
    private float startTime;
    private GameObject car;
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
                float tmp = Random.Range(0, 3);
                float currentSpawnInterval = Mathf.Lerp(initialSpawnInterval, minSpawnInterval, progress);

                yield return new WaitForSeconds(currentSpawnInterval);

                if (spawnPoints.Length > 0 && carPrefab != null)
                {
                    Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                    if(tmp > 1)
                    {
                        car = Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation);
                    }
                    else
                    {
                        car = Instantiate(carPrefab1, spawnPoint.position, spawnPoint.rotation);
                    }

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
