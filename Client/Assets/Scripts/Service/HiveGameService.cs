using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class HiveGameService
{
    private const string apiUrl = "https://localhost:7200/HiveGame/grid";

    public async Task<List<VertexDTO>> GetVerticesDataAsync()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl))
        {
            var operation = webRequest.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);
                return null;
            }

            string jsonResponse = webRequest.downloadHandler.text;
            Debug.Log(jsonResponse);
            return JsonUtility.FromJson<VertexDTOList>(jsonResponse).vertices;
        }
    }
}

[System.Serializable]
public class VertexDTOList
{
    public List<VertexDTO> vertices;
}