    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class CastleDefenseGame : MonoBehaviour
    {

        public GameObject cannonMuzzleFlashPrefab;

        [Header("Door Simulation")]
        public static bool isPlayerInRoom = false;
        private bool _wasInRoom = false;

        [Header("Game Objects")]

        public GameObject[] spawnPoints;

        public GameObject[] cannonModels;
        public GameObject castle;
        public GameObject[] ballPrefabs;
        public Transform targetObject;

        [Header("Animation Settings")]
        public float shootSyncDelay = 0.5f;

        private Animator[] _cannonAnimators;

        [Header("Audio Clips")]
        public AudioClip shootSound;
        public AudioClip hitSound;
        public AudioClip deflectionSound;

        private AudioSource _audioSource; 

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

            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
                _audioSource.playOnAwake = false;
            }

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

        public void StartGameLogic()
        {
            Counter.score = 0;
            castleHealth = maxCastleHealth;
            gameOver = false;
            gameTimer = 0f;
            currentBallSpeed = startBallSpeed;
            currentBallScale = startBallScale;

            activeBalls.Clear();
            StopAllCoroutines();

            _cannonAnimators = new Animator[cannonModels.Length];
            for (int i = 0; i < cannonModels.Length; i++)
            {
                _cannonAnimators[i] = cannonModels[i].GetComponent<Animator>();
            }

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
                Destroy(ball);
            }
            activeBalls.Clear();

            gameOver = false;
            _cannonAnimators = null;
        }

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
            int randomIndex = Random.Range(0, spawnPoints.Length);

            GameObject selectedSpawnPoint = spawnPoints[randomIndex];
            Animator cannonAnim = _cannonAnimators[randomIndex];

            cannonAnim.SetTrigger("Fire");

            if (shootSyncDelay > 0)
                yield return new WaitForSeconds(shootSyncDelay);

            if (shootSound != null)
            {
                _audioSource.PlayOneShot(shootSound, 1f);
            }

            if (cannonMuzzleFlashPrefab != null)
            {

                GameObject muzzleFlash = Instantiate(
                    cannonMuzzleFlashPrefab,
                    selectedSpawnPoint.transform.position,
                    cannonModels[randomIndex].transform.rotation
                );
                Destroy(muzzleFlash, 1f);
            }

            GameObject ballPrefab = ballPrefabs[Random.Range(0, ballPrefabs.Length)];
            GameObject ball = Instantiate(ballPrefab, selectedSpawnPoint.transform.position, Quaternion.identity);

            BallBehavior bb = ball.GetComponent<BallBehavior>();
            if (bb == null) bb = ball.AddComponent<BallBehavior>();

            bb.Initialize(targetObject, this, currentBallSpeed, currentBallScale);
            bb.ShootToTarget();

            activeBalls.Add(ball);

            yield return new WaitForSeconds(Random.Range(minShootInterval, maxShootInterval));
        }
    }

    public void CastleHit(int damage)
        {
            if (!isPlayerInRoom) return;

            castleHealth -= damage;

            _audioSource.PlayOneShot(hitSound, 1f);
        

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
        }
    }