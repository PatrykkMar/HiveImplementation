using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class VertexDTO
{
    public long id { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public int z { get; set; }
    public InsectType insect { get; set; }
    public bool isempty { get; set; }
    public bool highlighted { get; set; }
    public bool isthisplayerinsect { get; set; }
    public PlayerColor? playercolor { get; set; }
    public List<long> vertexidtomove { get; set; }
}