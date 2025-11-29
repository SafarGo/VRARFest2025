using UnityEngine;

/// <summary>
/// Простой контроллер для автоматических машин - просто двигаются вперед по прямой
/// </summary>
public class AutoCarController : MonoBehaviour
{
    [Tooltip("Скорость движения машины (метров/секунду)")]
    public float moveSpeed = 10f;

    void Update()
    {
        // Просто двигаемся вперед по прямой
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.Self);
    }
}
