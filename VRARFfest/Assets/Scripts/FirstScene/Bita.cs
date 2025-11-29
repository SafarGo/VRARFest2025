using UnityEngine;

public class Bita : MonoBehaviour
{
    public AudioClip deflectionSound;
    // --------------------------------------------------------

    private CastleDefenseGame _gameManager;

    void Start()
    {
        _gameManager = FindObjectOfType<CastleDefenseGame>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Counter.score += 10;
            collision.gameObject.tag = "Untagged";
            AudioSource.PlayClipAtPoint(deflectionSound, collision.contacts[0].point, 1f);
        }
    }
}