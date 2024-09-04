using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.AspNetCore.SignalR.Client;
using TMPro;
using Stateless;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameSceneUIManager : UIManager
{
    [SerializeField] protected HexGridGenerator _gridGenerator;
    [SerializeField] protected PlayerInsectView _playerInsectView;

    public override string Name
    {
        get
        {
            return "Game";
        }
    }

    public override void ConfigureUIForState(ClientState state)
    {
        base.ConfigureUIForState(state);

        if (_gridGenerator != null)
        {
            _gridGenerator.GenerateVertices(PlayerView.Board);
        }

        if (_playerInsectView != null)
        {
            _playerInsectView.SetInsects(PlayerView.PlayerInsects);
        }
    }

}