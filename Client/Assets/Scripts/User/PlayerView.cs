using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using UnityEngine;

public class PlayerView
{
    private PlayerView() { }
    public static List<VertexDTO> Board { get; set; } = new List<VertexDTO>();
    public static Dictionary<InsectType, int> PlayerInsects { get; set; } = new Dictionary<InsectType, int>();
    public static InsectType ChosenInsect { get; set; }
}
