using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class VertexDTO
{
    public long x { get; set; }
    public long y { get; set; }
    public long z { get; set; }
    public InsectType insect { get; set; }
    public bool isempty { get; set; }
    public bool highlighted { get; set; }
    public PlayerColor? playercolor { get; set; }
    public List<long> vertexidtomove { get; set; } = new List<long>();
    public List<long> vertexidtoput { get; set; } = new List<long>();
}