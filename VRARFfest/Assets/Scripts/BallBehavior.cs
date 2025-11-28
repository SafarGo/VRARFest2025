using System.Collections;
using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    private Transform castleTarget;
    private CastleDefenseGame gameManager;

    private Rigidbody rb;
    private bool isShot = false;

    private float shootSpeed;
    private float startScale;

    public void Initialize(Transform target, CastleDefenseGame manager, float currentSpeed, float currentScale)
    {
        castleTarget = target;
        gameManager = manager;

        shootSpeed = currentSpeed;
        startScale = currentScale;

        rb = GetComponent<Rigidbody>();
        if (!rb) rb = gameObject.AddComponent<Rigidbody>();

        rb.useGravity = true;
        rb.isKinematic = false;

        // фиксируем размер
        transform.localScale = Vector3.one * startScale;

        Debug.Log($"Ball INIT | Speed={shootSpeed} | Scale={startScale} (scale fixed, will NOT change)");
    }

    public void ShootBall(Vector3 direction)
    {
        if (isShot) return;
        isShot = true;

        rb.linearVelocity = direction.normalized * shootSpeed;

        Debug.Log($"Ball SHOT | Speed={shootSpeed} | Scale stays {startScale}");

        StartCoroutine(DestroyAfterTime(15f));
    }

    IEnumerator DestroyAfterTime(float t)
    {
        yield return new WaitForSeconds(t);
        Destroy(gameObject);
    }
}
