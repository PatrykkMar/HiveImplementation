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
using System.Linq;

public class UIManager : MonoBehaviour
{
    public Button[] buttonsToFill;
    public List<ButtonHelper> btnHelperList;
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
        ServiceLocator.Services.ClientStateMachine.OnStateChanged += SetUIAfterStateChange;
        ServiceLocator.Services.EventAggregator.AddButton += OnAddButton;
        ServiceLocator.Services.EventAggregator.RemoveButton += OnRemoveButton;
    }

    private void OnDisable()
    {
        ServiceLocator.Services.ClientStateMachine.OnStateChanged -= SetUIAfterStateChange;
        ServiceLocator.Services.EventAggregator.AddButton -= OnAddButton;
        ServiceLocator.Services.EventAggregator.RemoveButton -= OnRemoveButton;
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

    public virtual void SetUIAfterStateChange(ClientState state)
    {
        SetInformationText(state);
        btnHelperList = GetAvailableButtonsList(state);
        UpdateUIButtons();
    }

    public void OnAddButton(ButtonHelper button)
    {
        if(btnHelperList.FirstOrDefault(x=>x.Name == button.Name) == null)
        { 
            btnHelperList.Add(button);
            UpdateUIButtons();
        }
    }

    public void OnRemoveButton(string buttonText)
    {
        var button = btnHelperList.FirstOrDefault(x=>x.Name == buttonText);
        if (button != null)
        {
            btnHelperList.Remove(button);
            UpdateUIButtons();
        }
    }

    public virtual void UpdateUIButtons()
    {
        for(int i = 0; i<buttonsToFill.Length; i++)
        {
            if(i<btnHelperList.Count)
            {
                buttonsToFill[i].gameObject.transform.GetChild(0).GetComponent<Text>().text = btnHelperList[i].Name;
                buttonsToFill[i].onClick.RemoveAllListeners();
                buttonsToFill[i].onClick.AddListener(btnHelperList[i].Action);
                buttonsToFill[i].gameObject.SetActive(true);
            }
            else
            {
                buttonsToFill[i].gameObject.SetActive(false);
            }
        }
    }
}
