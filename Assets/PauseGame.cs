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
         UIManager.Instance.OpenUI(pausePanel);
            Time.timeScale = 0f;
            foreach (var player in players)
            {
                if (player != null)
                    player.SetInputEnabled(false);
            }
        }
        else
        {
           UIManager.Instance.CloseUI(pausePanel);
            Time.timeScale = 1f;
            foreach (var player in players)
            {
                if (player != null)
                    player.SetInputEnabled(true);
            }
        }
    }
}
