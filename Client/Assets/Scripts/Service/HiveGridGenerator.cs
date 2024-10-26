using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

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
        materialDictionary = new Dictionary<(PlayerColor,InsectType),Material>();
        foreach(var pair in insectObjectPairs)
        {
            var tuple = (pair.PlayerColor, pair.InsectType);
            materialDictionary.Add(tuple, pair.UV);
        }
    }

    public void GenerateVertices(List<VertexDTO> vertices)
    {
        foreach(var genVertex in generatedVertices)
        {
            Destroy(genVertex);
        }
        generatedVertices.Clear();

        foreach (var vertex in vertices)
        {
            var name = $"Hex_{vertex.x}_{vertex.y}_{vertex.z}_" + (vertex.insect == InsectType.Nothing ? "no insect" : "insect");
            Vector3 position = CalculatePosition(vertex.x, vertex.y, vertex.z);
            GameObject hexPrism = Instantiate(hexPrismPrefab, position, Quaternion.identity);
            generatedVertices.Add(hexPrism);
            if(vertex.insect != InsectType.Nothing)
            {
                Renderer hexPrismRenderer = hexPrism.GetComponent<Renderer>();
                var tuple = (vertex.playercolor.Value, vertex.insect);
                hexPrismRenderer.material = materialDictionary[tuple];
            }
            else
            {
                if(!vertex.highlighted)
                {
#if UNITY_EDITOR
                    Renderer hexPrismRenderer = hexPrism.GetComponent<Renderer>();
                    hexPrismRenderer.material = halfTransparentGreyMaterial;
#else
                hexPrism.SetActive(false);
#endif
                }
                else
                {
                    Renderer hexPrismRenderer = hexPrism.GetComponent<Renderer>();
                    hexPrismRenderer.material = halfTransparentMaterial;
                }
            }

            hexPrism.name = name;

            if (!hexPrism.GetComponent<Collider>())
            {
                hexPrism.AddComponent<BoxCollider>();
            }

            hexPrism.AddComponent<HexMouseActionHandler>().Vertex = vertex;
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