using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSwitcher : MonoBehaviour
{
    public string levelName;
    public float waitTime; // if zero, don't run
    public GameObject fadeToBlack;
    public GameObject fadeOutBlack;
    /*
    public bool autoBlack = true;

    private void Start()
    {
        if (autoBlack && fadeToBlack)
        {
            UIManager.Instance.OpenUI(fadeToBlack);
        }
    }
    */
    private void Awake()
    {
        if (fadeOutBlack ) fadeOutBlack.SetActive(true);// smooth transition
    }
    public void SetLevelName(string name)
    {
        levelName = name;
    }
    public void LoadDefaultScene()
    {
        if (levelName == null) {Debug.LogError("NULL SCENE LOAD REQUEST"); return; }
        if (fadeToBlack != null) UIManager.Instance.OpenUI(fadeToBlack.gameObject);
        StartCoroutine(delayLoad(levelName));
    }
    public void loadScene(string LevelName)
    {
        //if (levelName ) { LevelName = levelName; }
        if (fadeToBlack != null) UIManager.Instance.OpenUI(fadeToBlack);
        StartCoroutine(delayLoad(LevelName));
    }
    private IEnumerator delayLoad(string LevelName)
    {
        yield return new WaitForSecondsRealtime(waitTime);
        SceneManager.LoadScene(LevelName);

        Time.timeScale = 1.0f; // ensure every scene starts up at speed 1

    }
    [ContextMenu("Reload Current Scene")]
    public void ReloadScene()
    {
        loadScene(SceneManager.GetActiveScene().name);
    }

   
    bool _isShuttingDown = false;
    public void QuitApplication()
    {
        if (_isShuttingDown) return;
        _isShuttingDown = true;

        Debug.Log("Initiating End of Lifetime Protocol...");

        // 1. Run your shared cleanup protocols
        SavePlayerProgress();
        DisconnectFromNetwork();

        // 2. Execute platform-specific exit logic
        ExecutePlatformExit();
    }

    private void SavePlayerProgress()
    {
        Debug.Log("Saving game data...");
        PlayerPrefs.Save();
        // YourSaveSystem.SaveAll();
    }

    private void DisconnectFromNetwork()
    {
        Debug.Log("Disconnecting from multiplayer servers...");
        // YourNetworkManager.Disconnect();
    }

    private void ExecutePlatformExit()
    {
#if UNITY_EDITOR
        // Inside the Unity Editor, just stop playing
        UnityEditor.EditorApplication.isPlaying = false;

#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_LINUX
        // PC platforms expect a clean hard exit
        Application.Quit();

#elif UNITY_ANDROID
        // Android apps should minimize, not force close
        MinimizeAndroidActivity();

#elif UNITY_IOS
        // iOS rules strictly forbid programmatic quitting. Suspend instead.
        // Note: iOS usually lacks a "Quit" button entirely for compliance.
        Application.Quit(); 

#elif UNITY_WSA
        // Universal Windows Platform (UWP / Xbox fallback)
        Application.Quit();
        
#else
        // Catch-all fallback
        Application.Quit();
#endif
    }

    private void MinimizeAndroidActivity()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    // Calls moveTaskToBack(true), sending the app to the background cleanly
                    currentActivity.Call<bool>("moveTaskToBack", true);
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to minimize Android activity: {ex.Message}");
            Application.Quit(); // Fallback if JNI fails
        }
#endif
    }
}
