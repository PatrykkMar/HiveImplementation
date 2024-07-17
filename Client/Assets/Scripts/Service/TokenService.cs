using System;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Networking;

public class TokenService : MonoBehaviour
{
    [SerializeField] private string url = "http://localhost:7200/token/token";


    public IEnumerator GetToken()
    {
        Debug.Log(12);
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
        Debug.Log(2);
            yield return request.SendWebRequest();
        Debug.Log(3);
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error");
            }
            else
            {
                string token = request.downloadHandler.text;
                CurrentUser.Instance.Token = token;
                Debug.Log("Token: " + token);
            }
        }
    }
}