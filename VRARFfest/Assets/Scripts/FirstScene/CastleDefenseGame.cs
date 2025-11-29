using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleDefenseGame : MonoBehaviour
{
    [Header("Door Simulation")]
    public static bool isPlayerInRoom = false;
    private bool _wasInRoom = false;

    [Header("Game Objects")]
    // 1. Точки, ОТКУДА вылетает ядро (позиция спавна)
    public GameObject[] spawnPoints;
    // 2. Объекты, КОТОРЫЕ проигрывают анимацию (должен быть Animator)
    public GameObject[] cannonModels;
    public GameObject castle;
    public GameObject[] ballPrefabs;
    public Transform targetObject;

    [Header("Animation Settings")]
    [Tooltip("Задержка между стартом анимации и вылетом снаряда/звуком.")]
    public float shootSyncDelay = 0.5f;
    // Приватные массивы для быстрой работы
    private Animator[] _cannonAnimators;

    [Header("Audio Clips")]
    [Tooltip("Звук, проигрываемый при выстреле пушки (выстрел.mp3).")]
    public AudioClip shootSound;
    [Tooltip("Звук, проигрываемый при попадании в замок (попадание.mp3).")]
    public AudioClip hitSound;
    [Tooltip("Звук, проигрываемый, когда мяч отбит (отбил.mp3). Используется в Bita.cs")]
    public AudioClip deflectionSound;

    private AudioSource _audioSource; // Центральный AudioSource на этом объекте

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
        if (targetObject == null)
            targetObject = castle.transform;

        // --- ИНИЦИАЛИЗАЦИЯ ЦЕНТРАЛЬНОГО AUDIO SOURCE ---
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
        }
        // ------------------------------------------------

        StopGameLogic();
    }

    void Update()
    {
        if (isPlayerInRoom != _wasInRoom)
        {
            if (isPlayerInRoom)
            {
                StartGameLogic();
            }
            else
            {
                StopGameLogic();
            }
            _wasInRoom = isPlayerInRoom;
        }

        if (isPlayerInRoom && !gameOver)
            gameTimer += Time.deltaTime;
    }

    // =================================================================
    // МЕТОДЫ УПРАВЛЕНИЯ ИГРОВЫМ СОСТОЯНИЕМ
    // =================================================================

    public void StartGameLogic()
    {
        Debug.Log(">> Игрок вошел в комнату. Игра началась.");

        if (spawnPoints.Length != cannonModels.Length)
        {
            Debug.LogError("ОШИБКА: Количество точек спавна не совпадает с количеством моделей пушек!");
            return;
        }

        // Сброс переменных
        castleHealth = maxCastleHealth;
        gameOver = false;
        gameTimer = 0f;
        currentBallSpeed = startBallSpeed;
        currentBallScale = startBallScale;

        activeBalls.Clear();
        StopAllCoroutines();

        // Инициализация массива аниматоров
        _cannonAnimators = new Animator[cannonModels.Length];
        for (int i = 0; i < cannonModels.Length; i++)
        {
            _cannonAnimators[i] = cannonModels[i].GetComponent<Animator>();
            if (_cannonAnimators[i] == null)
            {
                Debug.LogError($"Модель пушки {cannonModels[i].name} (индекс {i}) не имеет компонента Animator!");
            }
        }

        // Запуск корутин
        StartCoroutine(ShootingRoutine());
        StartCoroutine(SpeedIncreaseRoutine());
        StartCoroutine(ScaleIncreaseRoutine());
    }

    public void StopGameLogic()
    {
        Debug.Log("<< Игрок вышел. Игра остановлена и сброшена.");
        StopAllCoroutines();

        foreach (GameObject ball in activeBalls)
        {
            if (ball != null) Destroy(ball);
        }
        activeBalls.Clear();

        gameOver = false;
        _cannonAnimators = null;
    }

    // =================================================================
    // КОРУТИНЫ
    // =================================================================

    IEnumerator SpeedIncreaseRoutine()
    {
        while (!gameOver && isPlayerInRoom)
        {
            yield return new WaitForSeconds(speedIncreaseInterval);
            currentBallSpeed += speedIncreaseAmount;
            if (currentBallSpeed > maxBallSpeed) currentBallSpeed = maxBallSpeed;
        }
    }

    IEnumerator ScaleIncreaseRoutine()
    {
        while (!gameOver && isPlayerInRoom)
        {
            yield return new WaitForSeconds(speedIncreaseInterval);
            currentBallScale -= scaleIncreaseAmount;
            if (currentBallScale < maxBallScale) currentBallScale = maxBallScale;
        }
    }

    IEnumerator ShootingRoutine()
    {
        if (_cannonAnimators == null) yield break;

        while (!gameOver && isPlayerInRoom && spawnPoints.Length > 0 && ballPrefabs.Length > 0)
        {
            // 1. Выбираем случайный ИНДЕКС
            int randomIndex = Random.Range(0, spawnPoints.Length);

            // 2. Получаем Точку Спавна (позиция)
            GameObject selectedSpawnPoint = spawnPoints[randomIndex];

            // 3. Получаем Аниматор
            Animator cannonAnim = _cannonAnimators[randomIndex];

            // 4. Запускаем анимацию
            if (cannonAnim != null)
            {
                cannonAnim.SetTrigger("Fire");
            }

            // 5. Ждем, пока анимация дойдет до момента "бабах" (синхронизация)
            if (shootSyncDelay > 0)
                yield return new WaitForSeconds(shootSyncDelay);

            // --- ВОСПРОИЗВЕДЕНИЕ ЗВУКА ВЫСТРЕЛА (НОВАЯ ПОЗИЦИЯ) ---
            if (shootSound != null && _audioSource != null)
            {
                // Теперь звук синхронизирован с моментом создания ядра
                _audioSource.PlayOneShot(shootSound, 1f);
            }
            // -------------------------------------------------------

            // 6. Создаем и запускаем ядро в точке спавна
            GameObject ballPrefab = ballPrefabs[Random.Range(0, ballPrefabs.Length)];
            GameObject ball = Instantiate(ballPrefab, selectedSpawnPoint.transform.position, Quaternion.identity);

            BallBehavior bb = ball.GetComponent<BallBehavior>();
            if (bb == null) bb = ball.AddComponent<BallBehavior>();

            bb.Initialize(targetObject, this, currentBallSpeed, currentBallScale);
            bb.ShootToTarget();

            activeBalls.Add(ball);

            // 7. Ждем следующего залпа
            yield return new WaitForSeconds(Random.Range(minShootInterval, maxShootInterval));
        }
    }

    // =================================================================
    // ЛОГИКА УРОНА И АУДИО
    // =================================================================

    public void CastleHit(int damage)
    {
        if (!isPlayerInRoom) return;

        castleHealth -= damage;

        // --- ВОСПРОИЗВЕДЕНИЕ ЗВУКА ПОПАДАНИЯ ---
        if (hitSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(hitSound, 1f);
        }
        // ----------------------------------------

        if (castleHealth <= 0)
        {
            castleHealth = 0;
            GameOver();
        }
    }

    void GameOver()
    {
        gameOver = true;
        StopAllCoroutines();
    }
    public void SetPlayerInRoomTrue()
    {
        Counter.score = 0;
        isPlayerInRoom = true;
        Debug.Log("Игра началась! isPlayerInRoom = true.");
    }
}