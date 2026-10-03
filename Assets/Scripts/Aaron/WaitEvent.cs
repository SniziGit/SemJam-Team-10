using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaitEvent : MonoBehaviour
{
    public float waitTime = 1f;
    private float initialWaitTime;
    public UnityEvent OnWaitEvent;
    public bool unscaledTime = false;
    private bool executed = false;

    /*
    private void OnValidate()
    {
        initialWaitTime = waitTime; //REMOVE FOR BUILD
    }*/
    private void Awake()
    {
        initialWaitTime = waitTime;
    }
    private void OnEnable()
    {
        executed = false;
        waitTime = initialWaitTime;
        StartCoroutine(Countdown());
    }

    // Update is called once per frame
   IEnumerator Countdown()
    {
        if (!unscaledTime)
        {

            yield return new WaitForSeconds(waitTime);
        }
        else
        {
            yield return new WaitForSecondsRealtime(waitTime);
        }
        OnWaitEvent?.Invoke();
        yield break;
    }

}
