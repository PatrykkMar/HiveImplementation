using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    public GameObject popupWindow; 
    public Button openButton;     
    public Button exitButton; 

    private void Start()
    {

        popupWindow.SetActive(false);

        openButton.onClick.AddListener(() =>
        {
            popupWindow.SetActive(true); 
        });

        exitButton.onClick.AddListener(() =>
        {
            popupWindow.SetActive(false);
        });
    }
}