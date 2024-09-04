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

public class MenuSceneUIManager : UIManager
{
    public override string Name
    {
        get
        {
            return "Menu";
        }
    }

    public override void ConfigureUIForState(ClientState state)
    {
        base.ConfigureUIForState(state);
    }

}