using HiveGame.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

//TODO: Add every event here
public class EventAggregator
{
    public event Action<string> InformationTextReceived;
    public event Action<string, float?> MinorInformationTextReceived;
    public event Action<List<VertexDTO>> BoardUpdate;
    public event Action<Dictionary<InsectType, int>> PlayerInsectsUpdate;

    public void InvokeInformationTextReceived(string text)
    {
        Debug.Log("InvokeInformationTextReceived");
        InformationTextReceived?.Invoke(text);
    }
    public void InvokeMinorInformationTextReceived(string text, float? delay = null)
    {
        Debug.Log("InvokeMinorInformationTextReceived");
        MinorInformationTextReceived?.Invoke(text, delay);
    }

    public void InvokeBoardUpdate(List<VertexDTO> hexes)
    {
        Debug.Log("InvokeBoardUpdate");
        BoardUpdate?.Invoke(hexes);
    }

    public void InvokePlayerInsectsUpdate(Dictionary<InsectType, int> insects)
    {
        Debug.Log("InvokePlayerInsectsUpdate");
        PlayerInsectsUpdate?.Invoke(insects);
    }
}