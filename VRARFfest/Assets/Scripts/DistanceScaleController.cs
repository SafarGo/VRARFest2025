using UnityEngine;

/// <summary>
/// Масштабирует объект в зависимости от расстояния до камеры
/// </summary>
public class DistanceScaleController : MonoBehaviour
{
    [Header("Настройки масштабирования")]
    [Tooltip("Камера для расчета расстояния (если не указана, используется Main Camera)")]
    public GameObject targetCamera;

    [Tooltip("Минимальное расстояние до камеры")]
    public float minDistance = 5f;

    [Tooltip("Максимальное расстояние до камеры")]
    public float maxDistance = 50f;

    [Tooltip("Минимальный масштаб объекта (при максимальном расстоянии)")]
    public float minScale = 0.5f;

    [Tooltip("Максимальный масштаб объекта (при минимальном расстоянии)")]
    public float maxScale = 2f;

    [Tooltip("Скорость изменения масштаба (для плавности)")]
    public float scaleSmoothness = 5f;

    private Vector3 originalScale;
    private float currentDistance;

    void Start()
    {
        // Сохраняем исходный масштаб
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (targetCamera == null) return;

        // Вычисляем расстояние до камеры
        currentDistance = Vector3.Distance(transform.position, targetCamera.transform.position);

        // Ограничиваем расстояние в пределах minDistance и maxDistance
        float clampedDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

        // Вычисляем нормализованное значение (0-1) для интерполяции
        float normalizedDistance = Mathf.InverseLerp(maxDistance, minDistance, clampedDistance);
        // Инвертируем: при minDistance = 1 (максимальный масштаб), при maxDistance = 0 (минимальный масштаб)

        // Вычисляем целевой масштаб
        float targetScale = Mathf.Lerp(minScale, maxScale, normalizedDistance);

        // Применяем масштаб с плавностью
        Vector3 targetScaleVector = originalScale * targetScale;
        transform.localScale = Vector3.Lerp(transform.localScale, targetScaleVector, Time.deltaTime * scaleSmoothness);
    }

    /// <summary>
    /// Получить текущее расстояние до камеры
    /// </summary>
    public float GetCurrentDistance()
    {
        return currentDistance;
    }
}

