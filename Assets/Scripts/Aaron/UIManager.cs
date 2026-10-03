using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityTouch = UnityEngine.Touch;

public class UIManager : MonoBehaviour
{
    public float transitionDuration = 0.1f;
    public GameObject[] panels; // Assign all UI panels in Inspector
    public static UIManager Instance;

    // Tracker for coroutine per UI element
    private Dictionary<CanvasGroup, Coroutine> fadeRoutines = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            enabled = false; // cheapest non-destructive neutralization
        }
    }

    private void Start()
    {
        
        //CloseAllPanels();
        foreach (GameObject panel in panels)
        {
                panel.SetActive(false);
        }
    }


    void Update()
    {
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Ended && !IsPointerOverUI(touch))
            {
                CloseAllPanels();
            }
        }
    }

    bool IsPointerOverUI(UnityTouch touch)
    {
        //return EventSystem.current.IsPointerOverGameObject();
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = touch.position
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        return results.Count > 0;
    }

  


    void CloseAllPanels()
    {   if (panels.Length > 0)
        {
            foreach (GameObject panel in panels)
            {
                if (!panel.activeSelf) return; // if currently inactive, skip
                if (panel.TryGetComponent<CanvasGroup>(out var cg))
                {
                    StartCoroutine(ShowFadeCanvasGroup(cg, false));
                }
                else
                {
                    panel.SetActive(false);
                }
            }
        }
    }
    public void ToggleUI(GameObject UI)
    {
        CanvasGroup cg = UI.GetComponent<CanvasGroup>();
        bool shouldShow = !UI.activeSelf; // invert the current state

        if (cg != null)
        {
            StartFade(cg, shouldShow);
        }
        else
        {
            UI.SetActive(shouldShow);
        }
    }

    public void OpenUI(GameObject UI)
    {
        CanvasGroup cg = UI.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            UI.SetActive(true);
            return;
        }
        StartFade(cg, true);
    }


    public void CloseUI(GameObject UI)
    {
        CanvasGroup cg = UI.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            UI.SetActive(false);
            return;
        }
        
        StartFade(cg, false);
    }
   

    void StartFade(CanvasGroup cg, bool isShowing)
    {
        // Cancel previous fade on this CanvasGroup
        if (fadeRoutines.TryGetValue(cg, out var running))
        {
            StopCoroutine(running);
        }

        fadeRoutines[cg] = StartCoroutine(ShowFadeCanvasGroup(cg, isShowing));
    }

    private IEnumerator ShowFadeCanvasGroup(CanvasGroup cg, bool isShowing)
    {
        //float from = (cg.alpha >= 0.001f || cg.alpha <= 0.999f) ? cg.alpha : gameObject.activeSelf ? 1f : 0f ; // check if alpha is in between transitions
        float from = cg.gameObject.activeSelf ? 1f : 0f;
        float to = isShowing ? 1f : 0f;

        //if (!(!cg.gameObject.activeSelf && !isShowing)) // avoid reactivation when already hiding, prevents a cascade of issues
        if (cg.gameObject.activeSelf || isShowing) // De Morgan's law conversion
        {

            cg.gameObject.SetActive(true); 
        }

        cg.interactable = false;

        float t = 0f;
        while (t < transitionDuration)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(from, to, t / transitionDuration);
            yield return null;
        }

        cg.alpha = to;
        cg.interactable = isShowing;

        if (!isShowing)
            cg.gameObject.SetActive(false);

        // Cleanup
        fadeRoutines.Remove(cg);
    }
}
