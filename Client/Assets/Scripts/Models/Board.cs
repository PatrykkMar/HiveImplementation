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

    public List<long> HexesToPutInsectIds { get; set; }

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
                if (highlighted.Contains(x.id))
                {
                    x.highlighted = true;
                }
                else
                {
                    x.highlighted = false;
                }
            });
        }

        Hexes = hexes;


        if (invokeEvent)
        {
            ServiceLocator.Services.EventAggregator.InvokeBoardUpdate(hexes);
        }
    }

    public void HighlightHexesToPutInsects(bool invokeEvent = true)
    {

        if (Hexes != null && Hexes.Count != 0)
            HighlightHexes(Hexes[0].vertexidtoput.ToArray(), invokeEvent);
    }

    public void HighlightHexes(long[] highlighted, bool invokeEvent = true)
    {
        SetBoard(Hexes, invokeEvent, highlighted);
    }

    public void CancelHighlighing(bool invokeEvent = true)
    {
        HighlightHexes(new long[] { }, invokeEvent);
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