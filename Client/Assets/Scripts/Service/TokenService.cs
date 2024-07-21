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
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
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