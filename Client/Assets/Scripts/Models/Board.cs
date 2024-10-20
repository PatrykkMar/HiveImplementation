using System.Collections;
using System.Collections.Generic;

public class Board
{
    private static Board _instance;

    public static Board Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Board();
            }
            return _instance;
        }
    }

    public List<VertexDTO> Hexes { get; private set; }

    private Board()
    {
        Hexes = new List<VertexDTO>();
    }

    public void SetBoard(List<VertexDTO> hexes, bool invokeEvent = true)
    {
        Hexes = hexes;
        if (invokeEvent)
        {
            ServiceLocator.Services.EventAggregator.InvokeBoardUpdate(hexes);
        }
    }
}