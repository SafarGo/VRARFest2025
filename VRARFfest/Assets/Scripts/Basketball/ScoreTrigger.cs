using UnityEngine;

// Вешается на объект, который является триггером попадания (ScoreTrigger)
public class ScoreTrigger : MonoBehaviour
{
    public AudioClip scoreSound;
    public ParticleSystem scoreVFX;

    public AudioSource audioSource;
    private int score = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Basketball"))
        {
            score++;
            audioSource.PlayOneShot(scoreSound);
            Debug.Log("shoot");
            scoreVFX.Play();
        }
    }
}