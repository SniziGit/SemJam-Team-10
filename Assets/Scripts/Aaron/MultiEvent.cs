
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class MultiEvent : MonoBehaviour
{
    //THIS IS A VISUAL METHOD BUILDER
    //USE RESPONSIBLY:
    //1. NO RECURSION. (Event called "Recurse" has a unityevent that fires "Recurse", or causes another script to fire "Recurse")
    //2. TYPE PROPERLY. The script will tell you if you called a wrong string method
    //3. KEEP IT ORGANISED. Don't overextend the methods across the entire scene.
    //4. Regular maintenance
    public NamedEvent[] multiEvent;
    public bool startOnEnable = false;
    private string[] eventNames; // Cached for cheap reuse later
    private void Awake() // ensure no duplicates
    {
        if (multiEvent == null || multiEvent.Length == 0)
        {
            eventNames = System.Array.Empty<string>();
            return;
        }

        // Allocate the exact memory footprint needed, exactly once
        eventNames = new string[multiEvent.Length];

        // High-performance indexing loop (No garbage collector allocations)
        for (int i = 0; i < multiEvent.Length; i++)
        {
            // Fallback guard: protects against null inspector slots or unassigned names
            eventNames[i] = multiEvent[i] != null ? multiEvent[i].name : string.Empty;
        }
        if (Utility.HasDuplicates(eventNames))
        {
            Debug.LogError($"DUPLICATE events detected on {gameObject.name}'s MultiEvent!!!");
        }
    }
    private void OnEnable()
    {
        if(startOnEnable)
        {
            InvokeEvent(0);
            currentIndex = 0;
        }
    }
    int currentIndex = 0;
    public void InvokeNextEvent()
    {
        if(currentIndex < multiEvent.Length)
        {
            currentIndex++;
            InvokeEvent(currentIndex);
        }
        else
        {
            Debug.LogWarning($"⚠️ <b>[Event System]</b> No more events to invoke on {gameObject.name}");
        }
    }
    public void InvokeEvent(int index)
    {
        if (index < multiEvent.Length)
        {
            //CheckForMissingEvents(multiEvent[index].unityEvent, index);// uncomment to find missing events
        multiEvent[index].unityEvent?.Invoke(); 
        }
    }
    public void InvokeEvent(string eventName)
    {
        // Passes the array, followed by the condition to check against
        int index = System.Array.FindIndex(multiEvent, x => x.name == eventName);

        if (index >= 0)
        {
            InvokeEvent(index);
        }
        else
        {
            // Safety warning instantly exposes typos in the console log
            Debug.LogError($"⚠️ <b>[Event System]</b> Failed to invoke '{eventName}' on {gameObject.name}. No matching name found in the inspector list!", gameObject);
        }
    }
    void CheckForMissingEvents(UnityEvent uEvent, int index) // expensive, run only when debugging
    {
        for (int i = 0; i < uEvent.GetPersistentEventCount(); i++)
        {
            Object targetObject = uEvent.GetPersistentTarget(i);
            if(targetObject == null || !System.Object.ReferenceEquals(targetObject, null))
            {
                Debug.LogError($"Missing UnityEvent Detected on {gameObject.name}!! Check event {index}, method {i} for missing references");
            }
        }
    }

    public void InvokeRandomEvent()
    {
        InvokeEvent(Random.Range(0, multiEvent.Length));
    }
    [System.Serializable]
    public class NamedEvent 
    { 
        public string name;
        public UnityEvent unityEvent;
    
    }



}
