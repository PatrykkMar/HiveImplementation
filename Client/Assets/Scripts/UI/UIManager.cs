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

public class UIManager : MonoBehaviour
{
    public Button[] buttons;
    [SerializeField] private HubService hub;
    [SerializeField] private TokenService tokenService;
    [SerializeField] private Text informationText;
    [SerializeField] private HexGridGenerator _gridGenerator;
    [SerializeField] private PlayerInsectView _playerInsectView;

    private string textToChange;
    private string stateScene;
    private ClientState? stateToChange;

    public List<ButtonHelper> GetAvailableButtonsList(ClientState state)
    {
        var btnList = new List<ButtonHelper>();
        switch(state)
        {
            case ClientState.Disconnected:
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
                btnList = new List<ButtonHelper>
                {
                    new ButtonHelper("Leave the queue", async () => await hub.LeaveQueueAsync())
                };
                break;

            case ClientState.InGamePlayerMove:
                btnList = new List<ButtonHelper>
                {
                    new ButtonHelper("Put insect", async () => await hub.PutInsectAsync(InsectType.Nothing, null)), //TODO: Put method for client
                    new ButtonHelper("Move insect", async () => await hub.MoveInsectAsync()) //TODO: Move method for client
                };
                break;

            case ClientState.InGameOpponentMove:
                btnList = new List<ButtonHelper>
                {
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
            case ClientState.Disconnected:
                text = "You are disconnected. Click a button to get a token and connect";
                break;
            case ClientState.Connected:
                text = "You are connected. Click a button to join a queue";
                break;
            case ClientState.WaitingForPlayers:
                text = "You are in a queue, wait for another player. You can click a button to leave a queue";
                break;
            case ClientState.InGamePlayerMove:
                text = "Your move";
                break;
            case ClientState.InGameOpponentMove:
                text = "Opponent's move";
                break;

        }
        textToChange = text;
    }

    public void ConfigureUIForState(ClientState state)
    {
        stateScene = Scenes.GetSceneByState(state);
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

        if(_gridGenerator != null)
        {
            _gridGenerator.GenerateVertices(PlayerView.Board);
        }

        if (_playerInsectView != null)
        {
            _playerInsectView.SetInsects(PlayerView.PlayerInsects);
        }
    }

    private void Update()
    {
        if(stateScene != null) 
        {
            var currentScene = SceneManager.GetActiveScene().name;
            if (currentScene != stateScene)
            {
                SceneManager.LoadScene(stateScene);
            }
        }

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