using UnityEngine;
public class BallBounceSound : MonoBehaviour
{
    public AudioClip bounceClip;

    [Header("Settings")]
    public float minVelocityForSound = 0.5f;
    public float maxVolumeScale = 0.1f;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1.0f;
            audioSource.playOnAwake = false;
        }
    }


    private void OnCollisionEnter(Collision collision)
    {

        float impactVelocity = collision.relativeVelocity.magnitude;

        if (impactVelocity > minVelocityForSound && bounceClip != null)
        {
            float volume = Mathf.Clamp01(impactVelocity * maxVolumeScale);

            audioSource.PlayOneShot(bounceClip, volume);
        }
    }
}