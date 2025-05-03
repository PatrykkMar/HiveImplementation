using HiveGame.Core.Models;
using HiveGame.Core.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;



[Serializable]
public class BoardDTOForUnitySerialization
{
    public PlayerColor playercolor;
    public List<VertexDTOForUnitySerialization> hexes;
    public List<long> vertexidtoput;
    public bool queenrulemet;
}

[Serializable]
public class VertexDTOForUnitySerialization
{
    public long id;
    public int x;
    public int y;
    public int z;
    public InsectType insect;
    public bool highlighted;
    public bool isempty;
    public bool isthisplayerinsect;
    public PlayerColor playercolor;
    public List<long> vertexidtomove;
    public string reasonwhymoveimpossible;
}

[Serializable]
public class PlayerViewDTOForUnitySerialization
{
    public BoardDTOForUnitySerialization board;
    public List<PlayerInsectTypePairDTOForUnitySerialization> playerInsectTypePairs;
}

[Serializable]
public class ReceiveMessageRequestForUnitySerialization
{
    public string playerId;
    public string message;
    public int state;
    public PlayerViewDTOForUnitySerialization playerView;

    public static ReceiveMessageRequest ConvertToOriginal(ReceiveMessageRequestForUnitySerialization serialized)
    {
        return new ReceiveMessageRequest(
            serialized.playerId,
            serialized.message,
            (serialized.state == default ? null : (ClientState?)serialized.state),
            new PlayerViewDTO
            {
                Board = new BoardDTO
                {
                    playercolor = serialized.playerView.board.playercolor,
                    hexes = serialized.playerView.board.hexes?.ConvertAll(v => new VertexDTO
                    {
                        id = v.id,
                        x = v.x,
                        y = v.y,
                        z = v.z,
                        insect = v.insect,
                        highlighted = v.highlighted,
                        isempty = v.isempty,
                        isthisplayerinsect = v.isthisplayerinsect,
                        playercolor = v.playercolor,
                        vertexidtomove = v.vertexidtomove,
                        reasonwhymoveimpossible = v.reasonwhymoveimpossible
                    }),
                    vertexidtoput = serialized.playerView.board.vertexidtoput,
                    queenrulemet = serialized.playerView.board.queenrulemet
                },
                PlayerInsectTypePairs = serialized.playerView.playerInsectTypePairs.Select(x=>new PlayerInsectTypePairDTO { amount=x.amount, type=x.type }).ToList()
            }
        );
    }
}
[Serializable]
public class PlayerInsectTypePairDTOForUnitySerialization
{
    public InsectType type;
    public int amount;
}