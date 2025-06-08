using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventHandler
{
    public static Dictionary<string, Action> events = new Dictionary<string, Action>();

    public static void AddListener(string eventType, Action action)
    {
        if (!events.ContainsKey(eventType))
        {
            events.Add(eventType, null);
        }
        events[eventType] += action;
    }

    public static void RemoveListener(string eventType, Action action)
    {
        if (!events.ContainsKey(eventType))
        {
            return;
        }
        events[eventType] -= action;
    }

    public static void InvokeEvent(string eventType)
    {
        Debug.Log($"Invoking event: {eventType}");
        if (events.ContainsKey(eventType))
        {
            events[eventType]?.Invoke();
        }
    }
}

public static class EventHandler<T>
{
    public static Dictionary<string, Action<T>> events = new Dictionary<string, Action<T>>();

    public static void AddListener(string eventType, Action<T> action)
    {
        if (!events.ContainsKey(eventType))
        {
            events.Add(eventType, null);
        }
        events[eventType] += action;
    }

    public static void RemoveListener(string eventType, Action<T> action)
    {
        if (!events.ContainsKey(eventType))
        {
            return;
        }
        events[eventType] -= action;
    }

    public static void InvokeEvent(string eventType, T value)
    {
        Debug.Log($"Invoking event: {eventType}");
        if (events.ContainsKey(eventType))
        {
            events[eventType]?.Invoke(value);
        }
    }
}
