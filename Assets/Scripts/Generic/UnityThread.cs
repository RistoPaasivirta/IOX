using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows systems to send actions to the main thread
/// </summary>
public class UnityThread : MonoBehaviour
{
    private static List<Action> queuedActions = new List<Action>();
    private List<Action> copiedActions = new List<Action>();
    private volatile static bool noActions = true;

    //call this to send an action to main thread
    public static void ExecuteInUpdate(Action action)
    {
        lock (queuedActions)
        {
            queuedActions.Add(action);
            noActions = false;
        }
    }

    //if there are actions pending to be processed, invoke them and clear the list
    private void Update()
    {
        if (noActions)
            return;

        copiedActions.Clear();
        lock (queuedActions)
        {
            copiedActions.AddRange(queuedActions);
            queuedActions.Clear();
            noActions = true;
        }

        for (int i = 0; i < copiedActions.Count; i++)
            copiedActions[i].Invoke();
    }
}