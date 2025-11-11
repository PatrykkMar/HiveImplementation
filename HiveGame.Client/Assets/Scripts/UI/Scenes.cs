using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.AspNetCore.SignalR.Client;
using TMPro;
using Stateless;
using UnityEngine.Events;
using HiveGame.Core.Models;

public class Scenes
{
    public const string MenuScene = "MenuScene";
    public const string GameScene = "GameScene";

    public static bool IsSceneChanging(ClientState? newState)
    {
        if (newState == null)
            return false;
        var strategyCurrentState = StateStrategyFactory.GetCurrentStateStrategy();
        var strategyNewState = StateStrategyFactory.GetStrategy(newState.Value);

        if(strategyCurrentState.Scene != strategyNewState.Scene)
            return true;

        return false;
    }
}
