using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerViewDTO
{
    public List<VertexDTO> Board { get; set; }
    public Dictionary<InsectType, int> PlayerInsects { get; set; }
}