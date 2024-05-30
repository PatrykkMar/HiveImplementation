using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridGenerator : MonoBehaviour
{
    public GameObject hexPrismPrefab;
    public GameObject hexPrismInsectPrefab;
    public float radius = 1.0f;
    private HiveGameService gameService;

    async void Start()
    {
        gameService = new HiveGameService();
        List<VertexDTO> vertices = await gameService.GetVerticesDataAsync();
        if (vertices != null)
        {
            OnVerticesDataReceived(vertices);
        }
    }

    private void OnVerticesDataReceived(List<VertexDTO> vertices)
    {
        foreach (var vertex in vertices)
        {
            Debug.Log($"Hex_{vertex.x}_{vertex.y}_{vertex.insect}");
            Vector3 position = CalculatePosition(vertex.x, vertex.y);
            GameObject hexPrism = Instantiate(vertex.insect == 0 ? hexPrismPrefab : hexPrismInsectPrefab, position, Quaternion.identity);
            hexPrism.name = $"Hex_{vertex.x}_{vertex.y}";
        }
    }

    private Vector3 CalculatePosition(long x, long y)
    {
        float posX = radius * 3f / 2f * x;
        posX/=10;
        float posY = radius * Mathf.Sqrt(3) * (y + x / 2f);
        posY/=10;
        return new Vector3(posX, 0, posY); // Assuming Z is the vertical axis in Unity
    }
}