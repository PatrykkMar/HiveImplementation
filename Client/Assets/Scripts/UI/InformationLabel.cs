using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.AspNetCore.SignalR.Client;
using TMPro;
using Stateless;
using UnityEngine.Events;

public class InformationLabel : MonoBehaviour
{
    [SerializeField] private Text InformationText;

    private void OnEnable()
    {
        Debug.Log("InformationLabel enabled");
        ServiceLocator.Services.HubService.OnMessageReceived += SetText;
    }

    private void OnDisable()
    {
        Debug.Log("InformationLabel disabled");
        ServiceLocator.Services.HubService.OnMessageReceived -= SetText;
    }

    private void SetText(string text)
    {
        InformationText.text = text;
    }
}
