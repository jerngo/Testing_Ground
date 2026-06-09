using System;
using System.Collections.Generic;

public static class DialogueEventChannel
{
    private static Dictionary<string, Action> events = new();

    public static void Register(string key, Action callback)
    {
        if (!events.ContainsKey(key))
            events[key] = null;
        events[key] += callback;
    }

    public static void Unregister(string key, Action callback)
    {
        if (events.ContainsKey(key))
            events[key] -= callback;
    }

    public static void Invoke(string key)
    {
        if (string.IsNullOrEmpty(key)) return;
        if (events.TryGetValue(key, out var action))
            action?.Invoke();
    }
}