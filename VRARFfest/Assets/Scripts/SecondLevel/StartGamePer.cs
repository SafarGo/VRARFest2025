using System.Collections;
using UnityEngine;

public class StartGamePer : MonoBehaviour
{

    private bool isFirstTime = true;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && isFirstTime)
            {
                gameObject.GetComponent<AudioSource>().Play();
                StartCoroutine(Timer());
                isFirstTime = false;
            PerekrestokLevelController.timeInGame = 0;
            }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !isFirstTime)
        {
            PerekrestokLevelController.IsGameEnded = true;
            isFirstTime = true;
        }
    }


    IEnumerator Timer()
    {
        yield return new WaitForSeconds(4);
        Counter.ResetScore(GameType.CarGame);
        PerekrestokLevelController.IsGameStarted = true;
        PerekrestokLevelController.IsGameEnded = false;
        
        Debug.Log(PerekrestokLevelController.IsGameStarted + "!11111S");
    }


}
