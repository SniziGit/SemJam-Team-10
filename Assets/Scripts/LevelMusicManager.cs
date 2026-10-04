using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMusicManager : MonoBehaviour
{
    private void Start()
    {
        // Ensure AudioManager exists and play appropriate music
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("AudioManager not found in scene. Please add AudioManager prefab to the scene.");
            return;
        }

        PlaySceneSpecificMusic();
    }

    private void PlaySceneSpecificMusic()
    {
        string sceneName = SceneManager.GetActiveScene().name.ToLower();

        // Check scene name and play appropriate music
        if (sceneName.Contains("mainmenu") || sceneName.Contains("menu") || sceneName.Contains("title"))
        {
            AudioManager.Instance.PlayMainMenuMusic();
        }
        else if (sceneName.Contains("level1") || sceneName.Contains("level 1") || sceneName.Contains("lvl1"))
        {
            AudioManager.Instance.PlayLevel1Music();
        }
        else if (sceneName.Contains("level2") || sceneName.Contains("level 2") || sceneName.Contains("lvl2"))
        {
            AudioManager.Instance.PlayLevel2Music();
        }
        else if (sceneName.Contains("level3") || sceneName.Contains("level 3") || sceneName.Contains("lvl3"))
        {
            AudioManager.Instance.PlayLevel3Music();
        }
        else
        {
            // Default to main menu music if scene name doesn't match
            Debug.Log($"Scene '{sceneName}' not recognized. Playing main menu music as default.");
            AudioManager.Instance.PlayMainMenuMusic();
        }
    }
}
