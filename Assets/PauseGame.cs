using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public GameObject pausePanel;
    public PlayerController[] players;
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0f;
            foreach (var player in players)
            {
                if (player != null)
                    player.SetInputEnabled(false);
            }
        }
        else
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1f;
            foreach (var player in players)
            {
                if (player != null)
                    player.SetInputEnabled(true);
            }
        }
    }
}
