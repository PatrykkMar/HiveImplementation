using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class HttpService
{
    private string Url
    {
        get
        {
            return _configLoader.GetConfigValue(ConfigLoaderConsts.HttpServiceUrlKey);
        }
    }

    private readonly ConfigLoader _configLoader;
    private readonly EventAggregator _eventAggregator;
    private readonly CurrentUser _currentUser;
    public event Action<string> OnTokenReceived;

    public HttpService(ConfigLoader configLoader, EventAggregator eventAggregator, CurrentUser currentUser)
    {
        _configLoader = configLoader;
        _eventAggregator = eventAggregator;
        _currentUser = currentUser;
    } 

    public IEnumerator GetToken()
    {
        Debug.Log("Get token method");

        using (UnityWebRequest request = UnityWebRequest.Get(Url))
        {
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Can't connect to server");
            }
            else
            {
                string token = request.downloadHandler.text;
                OnTokenReceived?.Invoke(token);
                Debug.Log("Token: " + token);
            }
        }
    }

    public IEnumerator GetTokenLoop()
    {
        Debug.Log("Attempting to get token...");

        if (_currentUser.HasToken) //there is no need to create new token after reconnecting
        {
            OnTokenReceived?.Invoke(_currentUser.Token);
            yield break;
        }

        while (true)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(Url))
            {
                _eventAggregator.InvokeInformationTextReceived("Connecting to server...");
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    _eventAggregator.InvokeInformationTextReceived("Can't connect to server, retrying in 10 seconds...");
                    yield return new WaitForSeconds(10f);
                }
                else
                {
                    string token = request.downloadHandler.text;
                    OnTokenReceived?.Invoke(token);
                    Debug.Log("Token: " + token);
                    yield break;
                }
            }
        }
    }
}