using HiveGame.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

//TODO: Add every event here
public class EventAggregator
{
    //information boxes
    public event Action<string> InformationTextReceived;
    public event Action<string, float?, float?> MinorInformationTextReceived;

    //board and insects
    public event Action<List<VertexDTO>> BoardUpdate;
    public event Action<Dictionary<InsectType, int>> PlayerInsectsUpdate;
    public event Action ClearSetInsect;

    //music
    public event Action PlaySound;

    //buttons
    public event Action<ButtonHelper> AddButton;
    public event Action<string> RemoveButton;

    public void InvokeInformationTextReceived(string text)
    {
        Debug.Log("InvokeInformationTextReceived");
        InformationTextReceived?.Invoke(text);
    }
    public void InvokeMinorInformationTextReceived(string text, float? delay = null, float? unstoppableTime = null)
    {
        Debug.Log("InvokeMinorInformationTextReceived");
        MinorInformationTextReceived?.Invoke(text, delay, unstoppableTime);
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

    public void InvokePlaySound()
    {
        PlaySound?.Invoke();
    }
    public void InvokeAddButton(ButtonHelper button)
    {
        AddButton?.Invoke(button);
    }
    public void InvokeRemoveButton(string buttonText)
    {
        RemoveButton?.Invoke(buttonText);
    }
    public void InvokeClearSetInsect()
    {
        ClearSetInsect?.Invoke();
    }

}