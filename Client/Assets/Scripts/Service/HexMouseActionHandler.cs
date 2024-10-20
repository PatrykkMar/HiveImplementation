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
        //StateStrategy.ON
    }

    void OnMouseExit()
    {
        //ServiceLocator.Services.EventAggregator.InvokeMovedMouseFromHex(Vertex);
    }
}