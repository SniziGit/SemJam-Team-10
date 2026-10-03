using System;
using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Typewriter : MonoBehaviour
{
    //public event Action onLineComplete;
    [SerializeField]private TMP_Text textBox;
    private int currentVisibleChars;
    private Coroutine typeWriting;

    private WaitForSecondsRealtime simpleDelay;
    private WaitForSecondsRealtime pauseDelay;

    public float typingSpeed = 5;
    [HideInInspector] public CompletionState completionState;


    public event Action onLineComplete;

    public UnityEvent lineCompleteEvent;

    
    void OnValidate()
    {
        textBox = GetComponent<TMP_Text>();
    }

    public void SetText(string text)
    {
        StopTyping();

        string processedtext = text;
      

        // 2. Assign the clean text to the text box first
        textBox.text = processedtext;

    

        StartTyping();
    }

    public void Skip()
    {
        if (completionState == CompletionState.Completed)
            return;

        // Instantly reveal all characters
        currentVisibleChars = textBox.textInfo.characterCount;
        textBox.maxVisibleCharacters = currentVisibleChars;
        textBox.ForceMeshUpdate();
    }

    public void StopTyping()
    {
        if (typeWriting != null)
        {
            StopCoroutine(typeWriting);
            typeWriting = null;
        }

        completionState = CompletionState.InProgress;
        currentVisibleChars = 0;
        
    }
    public void ClearText()
    {
        textBox.maxVisibleCharacters = 0;
        currentVisibleChars = 0;
    }

    // ==================================================
    // INTERNAL
    // ==================================================

    private void StartTyping()
    {
        completionState = CompletionState.InProgress;

        simpleDelay = new WaitForSecondsRealtime(1f / typingSpeed);
        pauseDelay = new WaitForSecondsRealtime(1.5f / typingSpeed);

        currentVisibleChars = 0;
        textBox.maxVisibleCharacters = 0;

        if (gameObject.activeInHierarchy)typeWriting = StartCoroutine(TypeWriting());
    }

    private IEnumerator TypeWriting()
    {
        textBox.ForceMeshUpdate();
        TMP_TextInfo textInfo = textBox.textInfo;

        while (currentVisibleChars < textInfo.characterCount)
        {
            textBox.maxVisibleCharacters = currentVisibleChars + 1;
            char c = textInfo.characterInfo[currentVisibleChars].character;

            yield return (c == ',' || c == '.' || c == '?')
                ? pauseDelay
                : simpleDelay;

            currentVisibleChars++;
        }
        yield return new WaitForSeconds(1f); // after line pause
        typeWriting = null;
        CompleteLine();
    }

    private void CompleteLine()
    {
        if (completionState == CompletionState.Completed)
            return;

        completionState = CompletionState.Completed;
        onLineComplete?.Invoke();
        lineCompleteEvent?.Invoke();
    }

    void OnDisable()
    {
        StopTyping();
    }
}
