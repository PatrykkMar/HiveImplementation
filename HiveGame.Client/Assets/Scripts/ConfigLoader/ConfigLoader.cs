using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class ConfigLoader
{
    private Dictionary<string, string> LoadConfig()
    {



#if UNITY_EDITOR
        return WindowsConfig.LoadConfig();
#elif UNITY_WEBGL
            return WebGlConfig.Datas;
#else
        return WindowsConfig.LoadConfig(); //default config loading
#endif
    }

    public string GetConfigValue(string key)
    {
        var configValues = LoadConfig();

        if (configValues.TryGetValue(key, out string value))
        {
            return value;
        }
        else
        {
            throw new ArgumentException($"Config key '{key}' not found!");
        }
    }
}