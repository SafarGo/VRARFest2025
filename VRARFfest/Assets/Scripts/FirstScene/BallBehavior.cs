using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallBehavior : MonoBehaviour
{
    // --- ПУБЛИЧНЫЕ НАСТРОЙКИ (Видны в Инспекторе) ---
    [Tooltip("Урон, который наносит ядро замку.")]
    public int damageAmount = 10;

    // --- Приватные поля, устанавливаемые при инициализации ---
    private Transform target;
    private CastleDefenseGame gameManager;

    private Rigidbody rb;
    private bool isShot = false;

    private float shootSpeed;
    private float startScale;

    // Переменная для сохранения скорости перед паузой
    private Vector3 savedVelocity;

    /// <summary>
    /// Инициализирует шарик для броска.
    /// </summary>
    public void Initialize(Transform target, CastleDefenseGame manager, float currentSpeed, float currentScale)
    {
        this.target = target;
        gameManager = manager;

        shootSpeed = currentSpeed;
        startScale = currentScale;

        rb = GetComponent<Rigidbody>();
        if (!rb) rb = gameObject.AddComponent<Rigidbody>();

        rb.useGravity = true;
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;

        transform.localScale = Vector3.one * startScale;

        Debug.Log($"Ball INIT | Speed={shootSpeed:F2} | Scale={startScale:F2}");
    }

    /// <summary>
    /// Запускает шарик по цели, используя баллистический расчет с калибровкой.
    /// </summary>
    public void ShootToTarget()
    {
        if (isShot) return;
        isShot = true;

        Vector3 initialVelocity = CalculateGuaranteedHitVelocity(target.position, shootSpeed);
        rb.linearVelocity = initialVelocity;

        Debug.Log($"Ball SHOT | Speed={shootSpeed:F2} | Target={target.name} | Final Vel: {initialVelocity}");

        // Устанавливаем таймер на самоуничтожение, если мяч не достиг цели
        StartCoroutine(DestroyAfterTime(15f));
    }

    // --- МЕТОДЫ ДЛЯ ПАУЗЫ/ВОЗОБНОВЛЕНИЯ (Используются CastleDefenseGame) ---

    /// <summary>
    /// Сохраняет текущую скорость и замораживает шарик (пауза).
    /// </summary>
    public void PauseBall()
    {
        if (rb == null || rb.isKinematic) return;

        savedVelocity = rb.linearVelocity; // Сохраняем текущую скорость
        rb.isKinematic = true;             // Замораживаем физику
        rb.useGravity = false;             // Отключаем гравитацию
        Debug.Log($"Ball paused, saved velocity: {savedVelocity}");
    }

    /// <summary>
    /// Восстанавливает скорость шарика (возобновление).
    /// </summary>
    public void ResumeBall()
    {
        if (rb == null || !rb.isKinematic) return;

        rb.isKinematic = false;          // Включаем физику
        rb.useGravity = true;            // Включаем гравитацию
        rb.linearVelocity = savedVelocity; // Восстанавливаем сохраненную скорость
        savedVelocity = Vector3.zero;      // Сбрасываем сохраненное значение
        Debug.Log($"Ball resumed, restored velocity: {rb.linearVelocity}");
    }

    // --- ОБРАБОТКА СТОЛКНОВЕНИЙ ---

    void OnCollisionEnter(Collision collision)
    {
        // Проверяем, что игра не на паузе
        if (gameManager != null && (gameManager.isPaused || !gameManager.isPlayerInRoom)) return;

        // --- УДАР ПО ЗАМКУ ---
        if (collision.gameObject.CompareTag("Castle"))
        {
            if (gameManager != null)
            {
                // Вызываем метод нанесения урона в менеджере игры
                gameManager.CastleHit(damageAmount);
            }
            Destroy(gameObject); // Уничтожаем шарик
        }

        // --- УДАР О ЗЕМЛЮ (или другую поверхность) ---
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Можно добавить отскок или уничтожить через короткое время
            // Destroy(gameObject, 3f); 
        }

        // Примечание: столкновение с битой обрабатывается в скрипте Bita.cs
    }

    // --- БАЛЛИСТИЧЕСКИЙ РАСЧЕТ И КАЛИБРОВКА ---

    private Vector3 CalculateGuaranteedHitVelocity(Vector3 targetPos, float speedMagnitude)
    {
        Vector3 displacement = targetPos - transform.position;
        Vector3 horizontalDisplacement = new Vector3(displacement.x, 0, displacement.z);
        float hDist = horizontalDisplacement.magnitude;
        float vDist = displacement.y;
        float g = Mathf.Abs(Physics.gravity.y);

        float speedSq = speedMagnitude * speedMagnitude;

        Vector3 horizontalDir = horizontalDisplacement.normalized;
        float finalAngle = float.NaN;

        float minAngleRad = gameManager.minLaunchAngle * Mathf.Deg2Rad;
        float maxAngleRad = gameManager.maxLaunchAngle * Mathf.Deg2Rad;

        // --- 1. КАЛИБРОВКА: Проверка минимальной скорости (V_min) ---
        float distToTarget = displacement.magnitude;
        float vMinSquared = g * (distToTarget + vDist);

        if (speedSq < vMinSquared * 0.995f)
        {
            // Скорость недостаточна. Используем максимальный угол
            finalAngle = maxAngleRad;
        }
        else
        {
            // --- 2. РАСЧЕТ ГАРАНТИРОВАННОГО ПОПАДАНИЯ ---
            float discriminant = speedSq * speedSq - g * (g * hDist * hDist + 2 * vDist * speedSq);
            if (discriminant < 0) discriminant = 0;
            float sqrtDisc = Mathf.Sqrt(discriminant);

            float tanTheta1 = (speedSq - sqrtDisc) / (g * hDist);
            float tanTheta2 = (speedSq + sqrtDisc) / (g * hDist);

            float angle1 = Mathf.Atan(tanTheta1);
            float angle2 = Mathf.Atan(tanTheta2);

            // Выбор угла (предпочтение низкой траектории в пределах диапазона)
            if (angle1 >= minAngleRad && angle1 <= maxAngleRad)
            {
                finalAngle = angle1;
            }
            else if (angle2 >= minAngleRad && angle2 <= maxAngleRad)
            {
                finalAngle = angle2;
            }
            else
            {
                // Если ни один из углов не в диапазоне, берем первый (наиболее прямой)
                finalAngle = angle1;
                if (float.IsNaN(finalAngle)) finalAngle = angle2;
            }
        }

        // --- 3. ФОРМИРОВАНИЕ ВЕКТОРА СКОРОСТИ ---
        if (float.IsNaN(finalAngle))
        {
            return Vector3.zero;
        }

        float vx = speedMagnitude * Mathf.Cos(finalAngle);
        float vy = speedMagnitude * Mathf.Sin(finalAngle);

        Vector3 velocity = horizontalDir * vx;
        velocity.y = vy;

        return velocity;
    }


    IEnumerator DestroyAfterTime(float t)
    {
        yield return new WaitForSeconds(t);
        if (gameObject != null)
            Destroy(gameObject);
    }
}