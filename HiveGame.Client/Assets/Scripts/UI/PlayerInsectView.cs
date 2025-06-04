using HiveGame.Core.Models;
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
        ServiceLocator.Services.HubService.OnStateFromServerReceived += ClearSetInsect;

		//TODO: Some information for player about his color
        //var image = GetComponent<Image>();

        //if (Board.Instance.PlayerColor == PlayerColor.White)
        //{
        //    image.color = Color.white;
        //}
        //else
        //{
        //    image.color = Color.black;
        //}
    }

    private void OnDisable()
    {
        ServiceLocator.Services.EventAggregator.PlayerInsectsUpdate -= UpdatePlayerInsectView;
        ServiceLocator.Services.HubService.OnStateFromServerReceived -= ClearSetInsect;
    }

    public void UpdatePlayerInsectView(Dictionary<InsectType, int> dict)
    {
        Debug.Log("PlayerInsectView: Updating insect view. Count: " + dict.Count);
        SetInsects(dict);
    }


    public void ClearSetInsect(ClientState trigger)
    {
        if (InsectButtonDict != null) 
            foreach (var ins in InsectButtonDict.Keys)
            {
                if (ins == InsectType.Nothing)
                    continue;
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

            if (!insectDict.ContainsKey(insect))
                insectDict.Add(insect, 0);

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
                    ChooseInsect(insect);
                    StateStrategyFactory.GetCurrentStateStrategy().OnInsectButtonClick(insect);
                });
            }

            InsectButtonDict[insect] = buttons[buttonIndex];

            buttonIndex++;
        }
    }

    public void ChooseInsect(InsectType insect)
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
    }
}
