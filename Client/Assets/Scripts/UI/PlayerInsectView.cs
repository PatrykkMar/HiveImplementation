using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInsectView : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    private Dictionary<InsectType, Button> InsectButtonDict;
    public static InsectType? ChosenInsect;


    public void Awake()
    {
        buttons = gameObject.GetComponentsInChildren<Button>();
    }

    private void OnEnable()
    {
        ServiceLocator.Services.EventAggregator.PlayerInsectsUpdate += UpdatePlayerInsectView;
        ServiceLocator.Services.HubService.OnStateReceived += ClearSetInsect;
    }

    private void OnDisable()
    {
        ServiceLocator.Services.EventAggregator.PlayerInsectsUpdate -= UpdatePlayerInsectView;
        ServiceLocator.Services.HubService.OnStateReceived -= ClearSetInsect;
    }

    public void UpdatePlayerInsectView(Dictionary<InsectType, int> dict)
    {
        SetInsects(dict);
    }


    public void ClearSetInsect(ClientState trigger)
    {
        if (InsectButtonDict != null) 
            foreach (var ins in InsectButtonDict.Keys)
            {
                if (ins == InsectType.Nothing)
                    continue;

                var button = InsectButtonDict[ins];
                var buttonImage = button.GetComponent<Image>();
                buttonImage.color = Color.white;
            }
    }

    public void SetInsects(Dictionary<InsectType, int> insectDict)
    {
        InsectButtonDict = new Dictionary<InsectType, Button>();

        int buttonIndex = 0;
        foreach(var insect in (InsectType[])Enum.GetValues(typeof(InsectType)))
        {
            if (insect == InsectType.Nothing)
                continue;

            buttons[buttonIndex].gameObject.transform.GetChild(0).GetComponent<Text>().text = Enum.GetName(typeof(InsectType), insect) + ": " + insectDict[insect].ToString();
            buttons[buttonIndex].onClick.RemoveAllListeners();

            if (insectDict[insect] == 0)
            {
                buttons[buttonIndex].enabled = false;
            }
            else
            {
                buttons[buttonIndex].enabled = true;
                buttons[buttonIndex].onClick.AddListener(() => 
                {
                    ChooseInsect(insect, true);
                    StateStrategyFactory.GetCurrentStateStrategy().OnInsectButtonClick(insect);
                });
            }

            InsectButtonDict[insect] = buttons[buttonIndex];

            buttonIndex++;
        }
    }

    public void ChooseInsect(InsectType insect, bool stateAction = true)
    {
        ChosenInsect = insect;
        Debug.Log("Chosen insect: " + Enum.GetName(typeof(InsectType), insect));
        foreach (var ins in InsectButtonDict.Keys)
        {
            if (ins == InsectType.Nothing)
                continue;

            var button = InsectButtonDict[ins];
            var buttonImage = button.GetComponent<Image>();

            if (ins == insect)
            {
                buttonImage.color = Color.red;
            }
            else
            {
                buttonImage.color = Color.white;
            }

        }

        if(stateAction == true) 
        {
            var stateStrategy = StateStrategyFactory.GetCurrentStateStrategy();
            stateStrategy.OnInsectButtonClick(insect);
        }
    }
}
