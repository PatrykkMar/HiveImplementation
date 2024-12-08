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

public class UIManager : MonoBehaviour
{
    public Button[] buttons;
    protected string textToChange;

    public virtual string Name
    {
        get
        {
            return "Base";
        }
    }

    private void OnEnable()
    {
        ServiceLocator.Services.ClientStateMachine.OnStateChanged += UpdateUI;
    }

    private void OnDisable()
    {
        ServiceLocator.Services.ClientStateMachine.OnStateChanged -= UpdateUI;
    }

    public List<ButtonHelper> GetAvailableButtonsList(ClientState state)
    {

        var btnList = StateStrategyFactory.GetStrategy(state).GetAvailableButtonsList();

        return btnList;
    }

    public void SetInformationText(ClientState state)
    {
        string text = StateStrategyFactory.GetStrategy(state).InformationText;
        Debug.Log("Text to change: " + text);
        ServiceLocator.Services.EventAggregator.InvokeInformationTextReceived(text);
    }

    public virtual void UpdateUI(ClientState state)
    {
        SetInformationText(state);
        List<ButtonHelper> btnHelperList = GetAvailableButtonsList(state);
        for(int i = 0; i<buttons.Length; i++)
        {
            if(i<btnHelperList.Count)
            {
                buttons[i].gameObject.transform.GetChild(0).GetComponent<Text>().text = btnHelperList[i].Name;
                buttons[i].onClick.RemoveAllListeners();
                buttons[i].onClick.AddListener(btnHelperList[i].Action);
                buttons[i].gameObject.SetActive(true);
            }
            else
            {
                buttons[i].gameObject.SetActive(false);
            }
        }
    }
}
