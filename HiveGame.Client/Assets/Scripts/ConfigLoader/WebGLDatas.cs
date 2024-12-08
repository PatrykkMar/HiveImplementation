using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class WebGlDatas
{
    public static string Datas
    {
        get
        {
            return "<Configuration> " +
                "       < HttpServiceUrl > https://www.hivegame.somee.com/token/token</HttpServiceUrl> " +
                "       < MatchmakingHubUrl > https://www.hivegame.somee.com/matchmakinghub</MatchmakingHubUrl> " +
                "</ Configuration >";
        }
    }
}