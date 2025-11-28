using UnityEngine;

public class Bita : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            ScoreManager.score += 10;
            Debug.Log(ScoreManager.score);
            collision.gameObject.CompareTag("");
        }

    }
}
