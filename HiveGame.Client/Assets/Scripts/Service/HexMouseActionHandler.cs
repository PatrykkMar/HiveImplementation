using UnityEngine;
using System;
using HiveGame.Core.Models;
using UnityEngine.UIElements;

public class HexMouseActionHandler : MonoBehaviour
{
    public VertexDTO Vertex { get; set; }

    private IHexEventsHandling StateStrategy
    {
        get
        {
            return StateStrategyFactory.GetCurrentStateStrategy();
        }
    }


    void OnMouseDown()
    {
        Debug.Log("Clicked hex: " + Vertex.id);
        StateStrategy.OnHexClick(Vertex);
    }
    void OnMouseEnter()
    {
        Debug.Log("Moved mouse on hex: " + $"Hex_{Vertex.x}_{Vertex.y}_{Vertex.z}_" + (Vertex.insect == InsectType.Nothing ? "no insect" : "insect") + (" id: " + Vertex.id));
        StateStrategy.OnHexMove(Vertex);
    }

    void OnMouseExit()
    {
        //ServiceLocator.Services.EventAggregator.InvokeMovedMouseFromHex(Vertex);
    }
}