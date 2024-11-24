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
    public PlayerColor PlayerColor { get; set; }

    public List<VertexDTO> Hexes { get; private set; }
    public Dictionary<InsectType, int> PlayerInsects { get; private set; }
    public bool QueenRuleMet {get;set;}

    public List<long> HexesToPutInsectIds { get; set; }

    private Board()
    {
        Hexes = new List<VertexDTO>();
    }

    public void SetBoardFromDTO(BoardDTO board, bool invokeEvent = true, long[] highlighted = null)
    {
        HexesToPutInsectIds = board.vertexidtoput;
        QueenRuleMet = board.queenrulemet;
        PlayerColor = board.playercolor;
        SetBoard(board.hexes, invokeEvent, highlighted);
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

        if (HexesToPutInsectIds!=null && HexesToPutInsectIds.Count > 0)
            HighlightHexes(HexesToPutInsectIds.ToArray(), invokeEvent);
    }

    public void HighlightHexesToMoveInsects(VertexDTO vertex, bool invokeEvent = true)
    {

        if (vertex.vertexidtomove != null && vertex.vertexidtomove.Count > 0)
            HighlightHexes(vertex.vertexidtomove.ToArray(), invokeEvent);
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