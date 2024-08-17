using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInsectView : MonoBehaviour
{
    [SerializeField] private Button[] buttons;

    public void Awake()
    {
        buttons = gameObject.GetComponentsInChildren<Button>();
    }

    public void SetInsects(Dictionary<InsectType, int> insectDict)
    {
        Debug.Log(insectDict.Count);
        int buttonIndex = 0;
        foreach(var insect in (InsectType[])Enum.GetValues(typeof(InsectType)))
        {
            if(insect != InsectType.Nothing)
                buttons[buttonIndex++].gameObject.transform.GetChild(0).GetComponent<Text>().text = Enum.GetName(typeof(InsectType), insect) + ": " + insectDict[insect].ToString();
        }
    }
}
