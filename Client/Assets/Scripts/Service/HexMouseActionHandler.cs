using UnityEngine;
using System;

public class HexMouseActionHandler : MonoBehaviour
{
    public VertexDTO Vertex { get; set; }

    private IStateStrategy StateStrategy
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
        Debug.Log("Moved mouse on hex: " + Vertex.Details);
        StateStrategy.OnHexMove(Vertex);
    }

    void OnMouseExit()
    {
        //ServiceLocator.Services.EventAggregator.InvokeMovedMouseFromHex(Vertex);
    }
}