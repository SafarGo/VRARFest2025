using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallBehavior : MonoBehaviour
{
    public GameObject hitVfxPrefab;
    public int damageAmount = 10;

    private Transform target;
    private CastleDefenseGame gameManager;

    private Rigidbody rb;
    private bool isShot = false;

    private float shootSpeed;
    private float startScale;

    private Vector3 savedVelocity;

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
    }

    public void ShootToTarget()
    {
        if (isShot) return;
        isShot = true;

        Vector3 initialVelocity = CalculateGuaranteedHitVelocity(target.position, shootSpeed);
        rb.linearVelocity = initialVelocity;

        StartCoroutine(DestroyAfterTime(15f));
    }

    public void PauseBall()
    {
        if (rb == null || rb.isKinematic) return;

        savedVelocity = rb.linearVelocity; 
        rb.isKinematic = true;             
        rb.useGravity = false;            
    }

    public void ResumeBall()
    {
        if (rb == null || !rb.isKinematic) return;

        rb.isKinematic = false;         
        rb.useGravity = true;        
        rb.linearVelocity = savedVelocity; 
        savedVelocity = Vector3.zero;      
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Paddle"))
        {
            Vector3 hitPoint = collision.contacts[0].point;
            
            GameObject vfx = Instantiate(hitVfxPrefab, hitPoint, Quaternion.identity);
            Destroy(vfx, 2f); 

        }

        if (target != null && collision.transform == target)
        {
            gameManager.CastleHit(10);
            Destroy(gameObject);
        }
    }
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

        float distToTarget = displacement.magnitude;
        float vMinSquared = g * (distToTarget + vDist);

        if (speedSq < vMinSquared * 0.995f)
        {
            finalAngle = maxAngleRad;
        }
        else
        {
            float discriminant = speedSq * speedSq - g * (g * hDist * hDist + 2 * vDist * speedSq);
            if (discriminant < 0) discriminant = 0;
            float sqrtDisc = Mathf.Sqrt(discriminant);

            float tanTheta1 = (speedSq - sqrtDisc) / (g * hDist);
            float tanTheta2 = (speedSq + sqrtDisc) / (g * hDist);

            float angle1 = Mathf.Atan(tanTheta1);
            float angle2 = Mathf.Atan(tanTheta2);

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
                finalAngle = angle1;
                if (float.IsNaN(finalAngle)) finalAngle = angle2;
            }
        }

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
        Destroy(gameObject);
    }
}