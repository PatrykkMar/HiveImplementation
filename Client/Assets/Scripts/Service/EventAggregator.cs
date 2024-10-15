using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

//TODO: Add every event here
public class EventAggregator
{
    public event Action<string> InformationTextReceived;

    public void InvokeInformationTextReceived(string text)
    {
        InformationTextReceived?.Invoke(text);
    }
}