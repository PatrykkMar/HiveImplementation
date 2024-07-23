using System;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Networking;

public class TokenService : MonoBehaviour
{
    [SerializeField] private string url = "http://localhost:7200/token/token";
    [SerializeField] private ClientStateMachine stateMachine;

    public IEnumerator GetToken(bool changeState)
    {
        Debug.Log("Get token method");
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
                if(changeState)
                    stateMachine.Fire(Trigger.ReceivedToken);
            }
        }
    }
}