using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using HiveGame.Core.Models;
using Unity.VisualScripting;

public class HexGridGenerator : MonoBehaviour
{
    public GameObject hexPrismPrefab;
    public float radius = 1.0f;
    private List<GameObject> generatedVertices = new List<GameObject>();
    public List<InsectObjectPair> insectObjectPairs;
    private Dictionary<(PlayerColor,InsectType),Material> materialDictionary;
    [SerializeField] private Material halfTransparentMaterial;
    [SerializeField] private Material halfTransparentGreyMaterial;


    private void OnEnable()
    {
        ServiceLocator.Services.EventAggregator.BoardUpdate += GenerateVertices;
    }

    private void OnDisable()
    {
        ServiceLocator.Services.EventAggregator.BoardUpdate -= GenerateVertices;
    }


    void Start()
    {
        CreateMaterialDictionary();
    }

    private void CreateMaterialDictionary()
    {
        Debug.Log("Creating material dictionary");
        materialDictionary = new Dictionary<(PlayerColor,InsectType),Material>();
        foreach(var pair in insectObjectPairs)
        {
            var tuple = (pair.PlayerColor, pair.InsectType);
            materialDictionary.Add(tuple, pair.UV);
        }
        Debug.Log("Material dictionary created. Count: " + materialDictionary.Count);
    }

    public void GenerateVertices(List<VertexDTO> vertices)
    {
        Debug.Log($"Vertices are creating. Number of vertices: {vertices.Count}");
        foreach(var genVertex in generatedVertices)
        {
            Destroy(genVertex);
        }
        generatedVertices.Clear();

        foreach (var vertex in vertices)
        {
            var name = $"Hex_{vertex.x}_{vertex.y}_{vertex.z}_" + 
                Enum.GetName(typeof(PlayerColor), vertex.playercolor) + "_" +
                (vertex.insect == InsectType.Nothing ? "no insect" : "insect") + 
                (" id: " + vertex.id);
            Debug.Log($"Creating: {name}");
            Vector3 position = CalculatePosition(vertex.x, vertex.y, vertex.z);
            GameObject hexPrism = Instantiate(hexPrismPrefab, position, Quaternion.identity);
            generatedVertices.Add(hexPrism);
            if(vertex.insect != InsectType.Nothing)
            {
                Renderer hexPrismRenderer = hexPrism.GetComponent<Renderer>();
                var tuple = (vertex.playercolor, vertex.insect);
                hexPrismRenderer.material = materialDictionary[tuple];
                AddCollider(hexPrism);
            }
            else
            {
                if (!vertex.highlighted)
                {
#if UNITY_EDITOR
                    Renderer hexPrismRenderer = hexPrism.GetComponent<Renderer>();
                    hexPrismRenderer.material = halfTransparentGreyMaterial;
                    RemoveCollider(hexPrism);
#else
                hexPrism.SetActive(false);
#endif
                }
                else
                {
                    Renderer hexPrismRenderer = hexPrism.GetComponent<Renderer>();
                    hexPrismRenderer.material = halfTransparentMaterial;
                    AddCollider(hexPrism);
                }
            }
            hexPrism.name = name;

            hexPrism.AddComponent<HexMouseActionHandler>().Vertex = vertex;
            Debug.Log($"Vertices created. Number of vertices: {vertices.Count}");
            WarningIfDuplicates(vertices);
        }
    }

    private void WarningIfDuplicates(List<VertexDTO> vertices)
    {
        for(int i=0; i<vertices.Count; i++)
        {
            for (int j = 0; j <vertices.Count; j++)
            {
                if (i!=j && vertices[i].x == vertices[j].x && vertices[i].y == vertices[j].y && vertices[i].z == vertices[j].z)
                    Debug.LogWarning("Created duplicated hexes");
            }
        }
    }

    private void AddCollider(GameObject obj)
    {
        Debug.Log("Add colider");
        if (!obj.GetComponent<Collider>())
        {
            obj.AddComponent<BoxCollider>();
        }
    }

    private void RemoveCollider(GameObject obj)
    {
        var collider = obj.GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider);
        }
    }


    private Vector3 CalculatePosition(long x, long y, long z)
    {
        float posX = radius * 3f / 2f * x;
        posX/=10;
        float posY = radius * Mathf.Sqrt(3) * (y + x / 2f);
        posY/=10;
        return new Vector3(posX, z * 0.5f, posY); // Assuming Z is the vertical axis in Unity
    }
}