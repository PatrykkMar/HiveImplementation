using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

//TODO: Add every event here
public class EventAggregator
{
    public event Action<string> InformationTextReceived;
    public event Action<List<VertexDTO>> BoardUpdate;

    //hex mouse actions
    //public event Action<VertexDTO> HexClicked;
    //public event Action<VertexDTO> MovedMouseOnHex;
    //public event Action<VertexDTO> MovedMouseFromHex;

    public void InvokeInformationTextReceived(string text)
    {
        InformationTextReceived?.Invoke(text);
    }

    public void InvokeBoardUpdate(List<VertexDTO> hexes)
    {
        BoardUpdate?.Invoke(hexes);
    }

    //public void InvokeHexClicked(VertexDTO hex)
    //{
    //    var stateStrategy = StateStrategyFactory.GetCurrentStateStrategy();
    //    state
    //}

    //public void InvokeMovedMouseOnHex(VertexDTO hex)
    //{
    //    MovedMouseOnHex?.Invoke(hex);
    //}

    //public void InvokeMovedMouseFromHex(VertexDTO hex)
    //{
    //    MovedMouseFromHex?.Invoke(hex);
    //}
}