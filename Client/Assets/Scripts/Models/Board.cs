using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
    public Dictionary<InsectType, int> PlayerInsects { get; private set; }

    private Board()
    {
        Hexes = new List<VertexDTO>();
    }

    public void SetBoard(List<VertexDTO> hexes, bool invokeEvent = true, long[] highlighted = null)
    {

        if (highlighted != null)
        {
            hexes.ForEach(x =>
            {
                if (hexes.Any(y => highlighted.Contains(y.id)))
                {
                    x.highlighted = true;
                }
            });
        }

        Hexes = hexes;


        if (invokeEvent)
        {
            ServiceLocator.Services.EventAggregator.InvokeBoardUpdate(hexes);
        }
    }

    public void HighlightHexes(long[] highlighted, bool invokeEvent = false)
    {
        SetBoard(Hexes, invokeEvent, highlighted);
    }

    public void SetPlayerInsects(Dictionary<InsectType, int> insects, bool invokeEvent = true)
    {
        PlayerInsects = insects;

        if (invokeEvent)
        {
            ServiceLocator.Services.EventAggregator.InvokePlayerInsectsUpdate(insects);
        }
    }
}