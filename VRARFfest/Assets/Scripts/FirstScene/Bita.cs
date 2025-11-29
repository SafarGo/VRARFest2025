using UnityEngine;

public class Bita : MonoBehaviour
{
    public AudioClip deflectionSound;
    public int score = 0;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            score += 10;
            Debug.Log(score);
            collision.gameObject.tag = "Untagged";
            AudioSource.PlayClipAtPoint(deflectionSound, collision.contacts[0].point, 1f);
        }
    }
}