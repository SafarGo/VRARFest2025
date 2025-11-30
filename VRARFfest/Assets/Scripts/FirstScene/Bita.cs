using UnityEngine;

public class Bita : MonoBehaviour
{
    public AudioClip deflectionSound;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            CastleDefenseGame.score += 1;
            collision.gameObject.tag = "Untagged";
            AudioSource.PlayClipAtPoint(deflectionSound, collision.contacts[0].point, 1f);
        }
    }
}