using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BoardDTO
{
    public List<VertexDTO> hexes { get; set; }
    public List<long> vertexidtoput { get; set; }
}