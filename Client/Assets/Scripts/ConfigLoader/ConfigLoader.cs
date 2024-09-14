using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class ConfigLoader
{
    private const string ConfigFileName = "settings.config";

    private Dictionary<string, string> LoadConfig()
    {
        if (!File.Exists(ConfigPath))
        {
            throw new FileNotFoundException($"Config file not found at {ConfigPath}!");
        }

        var configValues = new Dictionary<string, string>();

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(ConfigPath);

        XmlNodeList nodes = xmlDoc.DocumentElement.ChildNodes;
        foreach (XmlNode node in nodes)
        {
            if (node.NodeType == XmlNodeType.Element)
            {
                configValues[node.Name] = node.InnerText;
            }
        }

        return configValues;
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

    private string ConfigPath
    {
        get
        {
#if UNITY_EDITOR
            return Path.Combine(Directory.GetCurrentDirectory(), ConfigFileName);
#else
        return Path.Combine(Application.dataPath, ConfigFileName);
#endif

        }
    }
}