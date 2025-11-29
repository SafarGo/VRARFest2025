using UnityEngine;
using TMPro;
public class ScoreTrigger : MonoBehaviour
{
    public AudioClip scoreSound;
    public ParticleSystem scoreVFX;

    public AudioSource audioSource;
    private int score = 0;  

    public TMP_Text score_text;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Basketball"))
        {
            score++;
            score_text.text = "м€чей заброшено: "+score.ToString();
            audioSource.PlayOneShot(scoreSound);
            Debug.Log("shoot");
            scoreVFX.Play();
        }
    }
}