using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInsectView : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    private Dictionary<InsectType, Button> InsectButtonDict;


    public void Awake()
    {
        buttons = gameObject.GetComponentsInChildren<Button>();
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
                buttons[buttonIndex].onClick.AddListener(() => ChooseInsect(insect));
            }

            InsectButtonDict[insect] = buttons[buttonIndex];

            buttonIndex++;
        }
    }

    public void ChooseInsect(InsectType insect)
    {
        PlayerView.ChosenInsect = insect;

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
