using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class WebGlConfig
{
    public static Dictionary<string, string> Datas
    {
        get
        {
            var dic = new Dictionary<string, string>();
            dic.Add("HttpServiceUrl", "http://localhost:7200/token/token");
            dic.Add("MatchmakingHubUrl", "http://localhost:7200/matchmakinghub");
            //dic.Add("HttpServiceUrl", "https://www.hivegame.somee.com/token/token");
            //dic.Add("MatchmakingHubUrl", "https://www.hivegame.somee.com/matchmakinghub");
            return dic;
        }
    }
}