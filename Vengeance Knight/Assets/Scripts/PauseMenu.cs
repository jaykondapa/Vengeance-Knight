using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseCanvas;
    private bool isPaused = false;

    void Start()
    {
        SetPaused(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        SetPaused(!isPaused);
    }

    public void ResumeGame()
    {
        SetPaused(false);
    }

    void SetPaused(bool pause)
    {
        isPaused = pause;

        if (pauseCanvas != null)
            pauseCanvas.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;
    }
}