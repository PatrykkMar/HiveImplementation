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
}