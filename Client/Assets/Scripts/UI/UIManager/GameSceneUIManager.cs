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
    public override string Name
    {
        get
        {
            return "Game";
        }
    }

    public override void UpdateUI(ClientState state)
    {
        base.UpdateUI(state);
    }

}