using System.Collections;
using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    private Transform target;
    private CastleDefenseGame gameManager;

    private Rigidbody rb;
    private bool isShot = false;

    private float shootSpeed;
    private float startScale;

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

        // фиксируем размер
        transform.localScale = Vector3.one * startScale;

        Debug.Log($"Ball INIT | Speed={shootSpeed} | Scale={startScale}");
    }

    public void ShootToTarget()
    {
        if (isShot) return;
        isShot = true;

        // Рассчитываем точную скорость для попадания в цель
        Vector3 velocity = CalculatePerfectVelocity(target.position, shootSpeed);
        rb.linearVelocity = velocity;

        Debug.Log($"Ball SHOT | Speed={shootSpeed} | Target={target.name}");

        StartCoroutine(DestroyAfterTime(15f));
    }

    private Vector3 CalculatePerfectVelocity(Vector3 targetPos, float speed)
    {
        Vector3 direction = targetPos - transform.position;
        float horizontalDistance = new Vector3(direction.x, 0, direction.z).magnitude;
        float verticalDistance = direction.y;

        float g = Mathf.Abs(Physics.gravity.y);

        // Рассчитываем необходимый угол для точного попадания
        float angle = CalculateOptimalAngle(horizontalDistance, verticalDistance, speed, g);

        // Если угол невалидный (скорость слишком мала), используем максимально возможный угол
        if (float.IsNaN(angle))
        {
            angle = gameManager.maxLaunchAngle * Mathf.Deg2Rad;
            Debug.LogWarning($"Speed {speed} is too low for distance {horizontalDistance}, using max angle");
        }

        // Рассчитываем компоненты скорости
        float vx = speed * Mathf.Cos(angle);
        float vy = speed * Mathf.Sin(angle);

        // Нормализуем горизонтальное направление
        Vector3 horizontalDir = direction;
        horizontalDir.y = 0;
        horizontalDir.Normalize();

        // Создаем вектор скорости
        Vector3 velocity = horizontalDir * vx;
        velocity.y = vy;

        return velocity;
    }

    private float CalculateOptimalAngle(float horizontalDistance, float verticalDistance, float speed, float gravity)
    {
        float speedSquared = speed * speed;
        float discriminant = speedSquared * speedSquared - gravity * (gravity * horizontalDistance * horizontalDistance + 2 * verticalDistance * speedSquared);

        // Если дискриминант отрицательный, скорость слишком мала для попадания
        if (discriminant < 0)
        {
            return float.NaN;
        }

        float sqrtDiscriminant = Mathf.Sqrt(discriminant);

        // Два возможных угла (высокая и низкая траектория)
        float angle1 = Mathf.Atan((speedSquared + sqrtDiscriminant) / (gravity * horizontalDistance));
        float angle2 = Mathf.Atan((speedSquared - sqrtDiscriminant) / (gravity * horizontalDistance));

        // Выбираем оптимальный угол в пределах допустимых значений
        float minAngleRad = gameManager.minLaunchAngle * Mathf.Deg2Rad;
        float maxAngleRad = gameManager.maxLaunchAngle * Mathf.Deg2Rad;

        // Предпочитаем более низкую траекторию, если она в пределах допустимого
        if (angle2 >= minAngleRad && angle2 <= maxAngleRad)
        {
            return angle2;
        }
        // Иначе используем высокую траекторию
        else if (angle1 >= minAngleRad && angle1 <= maxAngleRad)
        {
            return angle1;
        }
        // Если оба угла вне допустимого диапазона, используем ближайший допустимый
        else
        {
            return Mathf.Clamp(angle2, minAngleRad, maxAngleRad);
        }
    }

    // Альтернативный метод для гарантированного попадания (более простой)
    private Vector3 CalculateGuaranteedHit(Vector3 targetPos, float speed)
    {
        Vector3 direction = targetPos - transform.position;
        float horizontalDistance = new Vector3(direction.x, 0, direction.z).magnitude;
        float verticalDistance = direction.y;

        float g = Mathf.Abs(Physics.gravity.y);

        // Рассчитываем необходимое время полета
        float time = horizontalDistance / speed;

        // Рассчитываем вертикальную компоненту скорости
        float vy = (verticalDistance / time) + (0.5f * g * time);

        // Нормализуем горизонтальное направление
        Vector3 horizontalDir = direction;
        horizontalDir.y = 0;
        horizontalDir.Normalize();

        Vector3 velocity = horizontalDir * speed;
        velocity.y = vy;

        return velocity;
    }

    IEnumerator DestroyAfterTime(float t)
    {
        yield return new WaitForSeconds(t);
        if (gameObject != null)
            Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform == target)
        {
            gameManager.CastleHit(10);
            Destroy(gameObject);
        }
    }

    // Для отладки - визуализация траектории
    void OnDrawGizmos()
    {
        if (!isShot || rb == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, rb.linearVelocity.normalized * 2f);

        // Показываем цель
        if (target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(target.position, 0.3f);
        }
    }
}