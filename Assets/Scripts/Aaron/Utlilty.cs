using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class Utility 
    
    /*This script can delay invocations of both methods and Unity events. Use it to prevent event / method clashes, or to time methods*/
{
    /* -------------------- TIME DELAY (seconds) -------------------- */

    public static async void InvokeAfter(this MonoBehaviour mb, Action action, float delay)
    {
        await Awaitable.WaitForSecondsAsync(delay);
        if (mb != null) action?.Invoke();
    }

    public static async void InvokeAfter<T>(this MonoBehaviour mb, Action<T> action, T arg, float delay)
    {
        await Awaitable.WaitForSecondsAsync(delay);
        if (mb != null) action?.Invoke(arg);
    }

    public static async void InvokeAfter(this MonoBehaviour mb, UnityEvent unityEvent, float delay)
    {
        await Awaitable.WaitForSecondsAsync(delay);
        if (mb != null) unityEvent?.Invoke();
    }


    /* -------------------- FRAME DELAY -------------------- */

    public static async void InvokeNextFrame(this MonoBehaviour mb, Action action)
    {
        await Awaitable.EndOfFrameAsync();
        if (mb != null) action?.Invoke();
    }

    public static async void InvokeFrames(this MonoBehaviour mb, Action action, int frames)
    {
        while (frames-- > 0)
        {
            await Awaitable.NextFrameAsync();
        }
        if (mb != null) action?.Invoke();
    }

    public static async void InvokeFrames<T>(this MonoBehaviour mb, Action<T> action, T arg, int frames)
    {
        while (frames-- > 0)
        {
            await Awaitable.NextFrameAsync();
        }
        if (mb != null) action?.Invoke(arg);
    }


    /* -------------------- UNITY EVENT (FRAME DELAY) -------------------- */

    public static async void InvokeNextFrame(this MonoBehaviour mb, UnityEvent unityEvent)
    {
        await Awaitable.EndOfFrameAsync();
        if (mb != null) unityEvent?.Invoke();
    }

    public static async void InvokeFrames(this MonoBehaviour mb, UnityEvent unityEvent, int frames)
    {
        while (frames-- > 0)
        {
            await Awaitable.NextFrameAsync();
        }
        if (mb != null) unityEvent?.Invoke();
    }


    // transform utilities
    public static void ScaleAround(Transform t, Vector3 pivotWorld, Vector3 newScale)
    {
        Vector3 oldScale = t.localScale;
        Vector3 scaleRatio = new Vector3(
            newScale.x / oldScale.x,
            newScale.y / oldScale.y,
            newScale.z / oldScale.z);

        Vector3 dir = t.position - pivotWorld;
        dir = Vector3.Scale(dir, scaleRatio);

        t.position = pivotWorld + dir;
        t.localScale = newScale;
    }


        public static Quaternion QuaternionSmoothDamp(Quaternion current, Quaternion target, ref Quaternion deriv, float time)
        {
            if (Time.deltaTime < Mathf.Epsilon) return current;

            // Match signs to guarantee the shortest rotation path
            if (Quaternion.Dot(current, target) < 0f)
            {
                target = new Quaternion(-target.x, -target.y, -target.z, -target.w);
            }

            // Smoothly dampen each individual component
            float x = Mathf.SmoothDamp(current.x, target.x, ref deriv.x, time);
            float y = Mathf.SmoothDamp(current.y, target.y, ref deriv.y, time);
            float z = Mathf.SmoothDamp(current.z, target.z, ref deriv.z, time);
            float w = Mathf.SmoothDamp(current.w, target.w, ref deriv.w, time);

            // Return normalized result to ensure valid rotation properties
            return new Quaternion(x, y, z, w).normalized;
        }




    // for UI to reset nested UI

    public static void RebuildLayoutBottomUp(RectTransform root)
    {
        //Debug.LogError($"Rebuilding layout for {root.name} and its children...");
        // Collect all RectTransforms in the hierarchy
        var rects = root.GetComponentsInChildren<RectTransform>(true);

        // Sort by depth (deepest first) so children are rebuilt before parents
        System.Array.Sort(rects, (a, b) =>
            GetDepth(b).CompareTo(GetDepth(a)));

        foreach (var rect in rects)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }

        // Final pass on root
        LayoutRebuilder.ForceRebuildLayoutImmediate(root);
    }

    public static int GetDepth(RectTransform rt)
    {
        int depth = 0;
        Transform t = rt;
        while (t.parent != null) { depth++; t = t.parent; }
        return depth;
    }
    // list utility
    public static bool HasDuplicates<T>(IEnumerable<T> collection)
    {
        HashSet<T> seen = new HashSet<T>();
        foreach (var item in collection)
        {
            if (!seen.Add(item)) // Add returns false if already exists
                return true;
        }
        return false;
    }

}
