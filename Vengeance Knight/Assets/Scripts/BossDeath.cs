using UnityEngine;

public class BossDeath : MonoBehaviour
{
    public GameObject victoryCanvas;
    public float victoryDelay = 3f;

    private EnemyHealth enemyHealth;
    private bool shown = false;

    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();

        if (victoryCanvas != null)
        {
            victoryCanvas.SetActive(false);
        }
    }

    void Update()
    {
        if (!shown && enemyHealth != null && enemyHealth.IsDead)
        {
            shown = true;

            Invoke(nameof(ShowVictoryScreen), victoryDelay);
        }
    }

    void ShowVictoryScreen()
    {
        if (victoryCanvas != null)
        {
            victoryCanvas.SetActive(true);
        }

        Time.timeScale = 0f;
    }
}