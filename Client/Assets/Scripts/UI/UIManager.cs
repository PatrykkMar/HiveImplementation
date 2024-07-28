using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.AspNetCore.SignalR.Client;
using TMPro;
using Stateless;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class UIManager : MonoBehaviour
{
    public Button[] buttons;
    [SerializeField] private MatchmakingHubService hub;
    [SerializeField] private TokenService tokenService;
    [SerializeField] private Text informationText;
    private string? textToChange;
    private ClientState? stateToChange;

    public List<ButtonHelper> GetAvailableButtonsList(ClientState state)
    {
        var btnList = new List<ButtonHelper>();
        switch(state)
        {
            case ClientState.Nothing:
                btnList = new List<ButtonHelper>
                {
                    new ButtonHelper("Get token", () => tokenService.GetToken(true))
                };
                break;
            case ClientState.Connected:
                btnList = new List<ButtonHelper>
                {
                    new ButtonHelper("Join the queue", async () => await hub.JoinQueueAsync())
                };

                break;
            case ClientState.WaitingForPlayers:
                //TODO: Leave the queue method and button
                btnList = new List<ButtonHelper>
                {
                    new ButtonHelper("Leave the queue", async () => await hub.LeaveQueueAsync())
                };
                break;
                        
        }
        return btnList;
    }

    public void SetInformationText(ClientState state)
    {
        string text = "";
        switch (state)
        {
            case ClientState.Nothing:
                text = "You are disconnected. Click a button to get a token and connect";
                break;
            case ClientState.Connected:
                text = "You are connected. Click a button to join a queue";
                break;
            case ClientState.WaitingForPlayers:
                text = "You are in a queue, wait for another player. You can click a button to leave a queue";
                break;

        }
        textToChange = text;
    }

    public void ConfigureUIForState(ClientState state)
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

    public void SetStateToChange(ClientState state)
    {
        stateToChange = state;
    }

    private void Update()
    {
        if(textToChange!= null) 
        {
            informationText.text = textToChange;
            textToChange = null;
        }

        if(stateToChange!= null)
        {
            ConfigureUIForState(stateToChange.Value);
            stateToChange = null;
        }
    }

}