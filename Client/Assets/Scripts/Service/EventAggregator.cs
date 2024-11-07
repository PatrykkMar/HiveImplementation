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
        InformationTextReceived?.Invoke(text);
    }
    public void InvokeMinorInformationTextReceived(string text, float? delay = null)
    {
        MinorInformationTextReceived?.Invoke(text, delay);
    }

    public void InvokeBoardUpdate(List<VertexDTO> hexes)
    {
        BoardUpdate?.Invoke(hexes);
    }

    public void InvokePlayerInsectsUpdate(Dictionary<InsectType, int> insects)
    {
        PlayerInsectsUpdate?.Invoke(insects);
    }
}