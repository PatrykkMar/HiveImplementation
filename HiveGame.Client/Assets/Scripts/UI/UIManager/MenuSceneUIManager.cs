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
using HiveGame.Core.Models;

public class MenuSceneUIManager : UIManager
{
    public InputField nickText;
    void Start()
    {
        nickText.onValueChanged.AddListener(OnNickChanged);
    }

    private void OnNickChanged(string newNick)
    {
        Board.Instance.PlayerNick = newNick;
    }
    public override string Name
    {
        get
        {
            return "Menu";
        }
    }

    public override void UpdateUI(ClientState state)
    {
        base.UpdateUI(state);
    }

}