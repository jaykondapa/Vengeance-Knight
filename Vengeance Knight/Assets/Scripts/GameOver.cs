using UnityEngine;

public class GameOver : MonoBehaviour
{
    public GameObject gameCanvas; // Assign GameCanvas in Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerGameOver();
        }
    }

    void TriggerGameOver()
    {
        gameCanvas.SetActive(true);
        Time.timeScale = 0f; // Freeze the game
    }
}