using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.AspNetCore.SignalR.Client;
using TMPro;
using Stateless;
using UnityEngine.Events;

public class ButtonHelper
{
    public string Name {get;set;}
    public UnityAction Action {get;set;}
    
    public ButtonHelper(string name, UnityAction func)
    {
        Name = name;
        Action = func;
    }

}