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
    public event Action<string> OnTokenReceived;

    public HttpService(ConfigLoader configLoader)
    {
        _configLoader = configLoader;
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
}