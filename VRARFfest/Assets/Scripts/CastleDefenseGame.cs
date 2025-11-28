using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleDefenseGame : MonoBehaviour
{
    [Header("Game Objects")]
    public GameObject[] cannons;
    public GameObject castle;
    public GameObject[] ballPrefabs;

    [Header("Game Settings")]
    public float minShootInterval = 3f;
    public float maxShootInterval = 6f;
    public int castleHealth = 100;
    public int maxCastleHealth = 100;

    [Header("Speed Settings")]
    public float startBallSpeed = 10f;
    public float maxBallSpeed = 20f;
    public float speedIncreaseInterval = 10f;
    public float speedIncreaseAmount = 1f;

    [Header("Scale Settings")]
    public float startBallScale = 0.5f;
    public float maxBallScale = 0.2f;
    public float scaleIncreaseAmount = 0.05f; // насколько увеличиваем размер

    [Header("Game State")]
    public int score = 0;
    public bool gameOver = false;

    public float currentBallSpeed;
    public float currentBallScale;

    private List<GameObject> activeBalls = new List<GameObject>();
    private float gameTimer = 0f;

    void Start()
    {
        currentBallSpeed = startBallSpeed;
        currentBallScale = startBallScale;

        StartCoroutine(ShootingRoutine());
        StartCoroutine(SpeedIncreaseRoutine());
        StartCoroutine(ScaleIncreaseRoutine());
    }

    void Update()
    {
        if (!gameOver)
            gameTimer += Time.deltaTime;
    }

    IEnumerator SpeedIncreaseRoutine()
    {
        while (!gameOver)
        {
            yield return new WaitForSeconds(speedIncreaseInterval);

            currentBallSpeed += speedIncreaseAmount;
            if (currentBallSpeed > maxBallSpeed)
                currentBallSpeed = maxBallSpeed;

            Debug.Log("—корость увеличена: " + currentBallSpeed);
        }
    }

    IEnumerator ScaleIncreaseRoutine()
    {
        while (!gameOver)
        {
            yield return new WaitForSeconds(speedIncreaseInterval);

            currentBallScale -= scaleIncreaseAmount;
            if (currentBallScale > maxBallScale)
                currentBallScale = maxBallScale;

            Debug.Log("–азмер шариков увеличен: " + currentBallScale);
        }
    }

    IEnumerator ShootingRoutine()
    {
        while (!gameOver)
        {
            GameObject cannon = cannons[Random.Range(0, cannons.Length)];
            GameObject ballPrefab = ballPrefabs[Random.Range(0, ballPrefabs.Length)];

            GameObject ball = Instantiate(ballPrefab, cannon.transform.position, cannon.transform.rotation);

            BallBehavior bb = ball.AddComponent<BallBehavior>();
            bb.Initialize(castle.transform, this, currentBallSpeed, currentBallScale);

            bb.ShootBall(cannon.transform.forward);

            activeBalls.Add(ball);

            yield return new WaitForSeconds(Random.Range(minShootInterval, maxShootInterval));
        }
    }

    public void CastleHit(int damage)
    {
        castleHealth -= damage;

        if (castleHealth <= 0)
        {
            castleHealth = 0;
            GameOver();
        }

        Debug.Log("Castle Health: " + castleHealth);
    }

    public void BallDeflected(GameObject ball)
    {
        score += 10;
        activeBalls.Remove(ball);
        Debug.Log("Score: " + score);
    }

    void GameOver()
    {
        gameOver = true;
        StopAllCoroutines();

        foreach (GameObject ball in activeBalls)
            if (ball != null) Destroy(ball);

        activeBalls.Clear();

        Debug.Log("GAME OVER");
    }
}
