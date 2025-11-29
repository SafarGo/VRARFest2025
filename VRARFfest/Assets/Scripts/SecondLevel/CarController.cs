using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class CarController : MonoBehaviour
{
    [Header("Настройки движения")]
    [Tooltip("Базовая скорость движения вперед (метров/секунду)")]
    public float moveSpeed;

    [Header("Настройки ускорения/замедления при свайпе")]
    [Tooltip("Множитель ускорения при свайпе вперед")]
    public float boostMultiplier = 2f;

    [Tooltip("Множитель замедления при свайпе назад (0-1, где 0.2 = 20% скорости, сильное замедление)")]
    [Range(0f, 1f)]
    public float slowdownMultiplier = 0.2f;

    [Tooltip("Минимальная скорость свайпа для активации (м/с)")]
    public float minSwipeSpeed = 1f;

    [Tooltip("Длительность эффекта ускорения/замедления после свайпа (секунды)")]
    public float boostDuration = 1f;

    [Tooltip("Скорость возврата к нормальной скорости")]
    public float speedDecayRate = 2f;

    [Tooltip("Настройки размеров при спавне")]
    public float scale;
    public float min_scale_coef;
    public float max_scale_coef;

    // Приватные переменные
    private float currentSpeed;
    private float boostTimer = 0f;
    private bool isBoosted = false;
    private bool isSlowed = false;
    private XRGrabInteractable grabInteractable;
    private IXRSelectInteractor currentInteractor;
    private Vector3 lastControllerPosition;
    private bool hasLastPosition = false;

    void Start()
    {
        currentSpeed = moveSpeed;

        // Получаем или добавляем компонент XRGrabInteractable
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable == null)
        {
            grabInteractable = gameObject.AddComponent<XRGrabInteractable>();
        }

        // Подписываемся на события захвата и отпускания
        grabInteractable.selectEntered.AddListener(OnGrabStarted);
        grabInteractable.selectExited.AddListener(OnGrabEnded);
        scale = Random.Range(min_scale_coef, max_scale_coef);
        transform.localScale = new Vector3(transform.localScale.x * scale, scale * transform.localScale.x * scale, scale * transform.localScale.x * scale);
        moveSpeed = Random.Range(0.5f, 2f);
    }

    void Update()
    {
        // Обработка свайпа при захвате
        if (currentInteractor != null && currentInteractor.isSelectActive)
        {
            Transform interactorTransform = currentInteractor.transform;
            if (interactorTransform != null)
            {
                Vector3 controllerPosition = interactorTransform.position;
                
                if (hasLastPosition)
                {
                    // Вычисляем движение контроллера в мировых координатах
                    Vector3 controllerMovement = controllerPosition - lastControllerPosition;
                    
                    // Преобразуем движение в локальное пространство объекта
                    Vector3 localMovement = transform.InverseTransformDirection(controllerMovement);
                    
                    // Вычисляем скорость движения контроллера (в локальном пространстве)
                    float swipeSpeed = localMovement.z / Time.deltaTime;
                    
                    // Определяем направление свайпа
                    if (Mathf.Abs(swipeSpeed) > minSwipeSpeed)
                    {
                        if (swipeSpeed > 0f)
                        {
                            // Свайп вперед - активируем ускорение
                            isBoosted = true;
                            isSlowed = false;
                            boostTimer = boostDuration;
                            gameObject.GetComponent<AudioSource>().Play();
                        }
                        else
                        {
                            // Свайп назад - активируем замедление
                            isSlowed = true;
                            isBoosted = false;
                            boostTimer = boostDuration;
                        }
                    }
                }
                else
                {
                    hasLastPosition = true;
                }
                
                lastControllerPosition = controllerPosition;
            }
        }
        else
        {
            hasLastPosition = false;
        }

        // Обработка таймера ускорения/замедления
        if (boostTimer > 0f)
        {
            boostTimer -= Time.deltaTime;
            
            if (boostTimer <= 0f)
            {
                isBoosted = false;
                isSlowed = false;
                boostTimer = 0f;
            }
        }

        // Вычисляем целевую скорость
        float targetSpeed = moveSpeed + 0.001f;
        if (isBoosted)
        {
            targetSpeed = moveSpeed * boostMultiplier;
        }
        else if (isSlowed)
        {
            targetSpeed = moveSpeed * slowdownMultiplier;
        }

        // Плавное изменение скорости
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * speedDecayRate);

        // Движение строго вперед по прямой
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime, Space.Self);
    }

    void FixedUpdate()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    // Вызывается при захвате объекта
    private void OnGrabStarted(SelectEnterEventArgs args)
    {
        currentInteractor = args.interactorObject;
        hasLastPosition = false;
        
        if (currentInteractor != null && currentInteractor.transform != null)
        {
            lastControllerPosition = currentInteractor.transform.position;
        }
    }

    // Вызывается при отпускании объекта
    private void OnGrabEnded(SelectExitEventArgs args)
    {
        currentInteractor = null;
        hasLastPosition = false;
    }

    void OnDestroy()
    {
        // Отписываемся от событий при уничтожении объекта
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabStarted);
            grabInteractable.selectExited.RemoveListener(OnGrabEnded);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Car"))
        {
            Debug.Log("Collision with car");
        }
        if (collision.gameObject.CompareTag("Die"))
        {
            Counter.AddScore(1, GameType.CarGame);
            Debug.Log("Score: " + Counter.GetScore(GameType.CarGame));
            PerekrestokLevelController.IsGameEnded = true;
            Destroy(gameObject);
        }
    }
}
