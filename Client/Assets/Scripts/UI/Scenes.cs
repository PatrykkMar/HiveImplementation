using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.AspNetCore.SignalR.Client;
using TMPro;
using Stateless;
using UnityEngine.Events;

public class Scenes
{
    public const string MenuScene = "MenuScene";
    public const string GameScene = "GameScene";

    public static string GetSceneByState(ClientState state)
    {
        switch(state)
        {
            case ClientState.Disconnected:
            case ClientState.Connected:
            case ClientState.WaitingForPlayers:
                return MenuScene;
            case ClientState.InGamePlayerMove:
            case ClientState.InGamePlayerFirstMove:
            case ClientState.InGameOpponentMove:
            case ClientState.GameOver:
                return GameScene;
            default:
                throw new NotImplementedException("Scene for this state not found");
        }
    }
}
