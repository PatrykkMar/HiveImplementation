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
    [SerializeField] private Text informationText;
    protected string textToChange;
    protected string stateScene;
    protected ClientState? stateToChange;
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
        textToChange = text;
    }

    public virtual void UpdateUI(ClientState state)
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
    }

    private void Update()
    {
        if(stateScene != null) 
        {
            var currentScene = SceneManager.GetActiveScene().name;
            if (currentScene != stateScene)
            {
                SceneManager.LoadScene(stateScene);
                return;
            }
        }

        if(textToChange!= null) 
        {
            Debug.Log("Text to change: "+textToChange);
            informationText.text = textToChange;
            textToChange = null;
        }

        if(stateToChange!= null)
        {
            Debug.Log("State to change: " + Enum.GetName(typeof(ClientState), stateToChange.Value));
            UpdateUI(stateToChange.Value);
            stateToChange = null;
        }
    }

}
