using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleDefenseGame : MonoBehaviour
{
    [Header("Door Simulation")]
    public bool isPlayerInRoom = false; // Та самая булевая переменная для имитации
    private bool _wasInRoom = false;    // Для отслеживания изменения состояния

    [Header("Game Objects")]
    public GameObject[] cannons;
    public GameObject castle;
    public GameObject[] ballPrefabs;
    public Transform targetObject;

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
    public float scaleIncreaseAmount = 0.05f;

    [Header("Ballistic Settings")]
    public float minLaunchAngle = 10f;
    public float maxLaunchAngle = 80f;

    [Header("Game State")]
    public bool gameOver = false;
    public bool isPaused = false;

    public float currentBallSpeed;
    public float currentBallScale;

    private List<GameObject> activeBalls = new List<GameObject>();
    private float gameTimer = 0f;

    void Start()
    {
        // Базовая инициализация ссылок, но НЕ запуск игры
        if (targetObject == null)
            targetObject = castle.transform;

        // При старте сцены убеждаемся, что всё чисто
        StopGameLogic();
    }

    void Update()
    {
        // --- ЛОГИКА ДВЕРИ (ПРОВЕРКА ИЗМЕНЕНИЯ СОСТОЯНИЯ) ---
        if (isPlayerInRoom != _wasInRoom)
        {
            if (isPlayerInRoom)
            {
                StartGameLogic(); // Зашли в дверь -> Старт
            }
            else
            {
                StopGameLogic();  // Вышли из двери -> Стоп/Сброс
            }
            _wasInRoom = isPlayerInRoom;
        }

        // Таймер работает только если игра активна и игрок внутри
        if (isPlayerInRoom && !gameOver)
            gameTimer += Time.deltaTime;
    }

    // Метод запуска игры (перенесен из Start)
    public void StartGameLogic()
    {
        Debug.Log(">> Игрок вошел в комнату. Игра началась.");

        // Сброс переменных
        castleHealth = maxCastleHealth;
        gameOver = false;
        gameTimer = 0f;
        currentBallSpeed = startBallSpeed;
        currentBallScale = startBallScale;

        activeBalls.Clear();
        StopAllCoroutines(); // На случай, если что-то висело

        // Запуск корутин
        StartCoroutine(ShootingRoutine());
        StartCoroutine(SpeedIncreaseRoutine());
        StartCoroutine(ScaleIncreaseRoutine());
    }

    // Метод остановки игры (полная очистка)
    public void StopGameLogic()
    {
        Debug.Log("<< Игрок вышел. Игра остановлена и сброшена.");

        StopAllCoroutines();

        // Удаляем все шарики, которые сейчас есть на сцене
        foreach (GameObject ball in activeBalls)
        {
            if (ball != null) Destroy(ball);
        }
        activeBalls.Clear();

        // Сбрасываем флаги (опционально)
        gameOver = false;
    }

    // ... (Остальные корутины ShootingRoutine, SpeedIncreaseRoutine и т.д. остаются без изменений) ...

    IEnumerator SpeedIncreaseRoutine()
    {
        while (!gameOver && isPlayerInRoom) // Добавлена проверка isPlayerInRoom
        {
            yield return new WaitForSeconds(speedIncreaseInterval);
            currentBallSpeed += speedIncreaseAmount;
            if (currentBallSpeed > maxBallSpeed) currentBallSpeed = maxBallSpeed;
            Debug.Log("Скорость увеличена: " + currentBallSpeed);
        }
    }

    IEnumerator ScaleIncreaseRoutine()
    {
        while (!gameOver && isPlayerInRoom)
        {
            yield return new WaitForSeconds(speedIncreaseInterval);
            currentBallScale -= scaleIncreaseAmount;
            if (currentBallScale < maxBallScale) currentBallScale = maxBallScale;
            Debug.Log("Размер шариков уменьшен: " + currentBallScale);
        }
    }

    IEnumerator ShootingRoutine()
    {
        while (!gameOver && isPlayerInRoom)
        {
            if (cannons.Length > 0 && ballPrefabs.Length > 0)
            {
                GameObject cannon = cannons[Random.Range(0, cannons.Length)];
                GameObject ballPrefab = ballPrefabs[Random.Range(0, ballPrefabs.Length)];
                GameObject ball = Instantiate(ballPrefab, cannon.transform.position, Quaternion.identity);

                BallBehavior bb = ball.GetComponent<BallBehavior>();
                if (bb == null) bb = ball.AddComponent<BallBehavior>();

                bb.Initialize(targetObject, this, currentBallSpeed, currentBallScale);
                bb.ShootToTarget();

                activeBalls.Add(ball);
            }
            yield return new WaitForSeconds(Random.Range(minShootInterval, maxShootInterval));
        }
    }

    public void CastleHit(int damage)
    {
        if (!isPlayerInRoom) return; // Урон не проходит, если игрока нет

        castleHealth -= damage;
        if (castleHealth <= 0)
        {
            castleHealth = 0;
            GameOver();
        }
        Debug.Log("Castle Health: " + castleHealth);
    }

    void GameOver()
    {
        gameOver = true;
        StopAllCoroutines();
        Debug.Log("GAME OVER. Final Time: " + gameTimer);
        // Тут можно не удалять шары сразу, чтобы игрок увидел поражение,
        // но когда он выйдет из двери (isPlayerInRoom = false), StopGameLogic() всё почистит.
    }
}