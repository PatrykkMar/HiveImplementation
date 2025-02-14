using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class WindowsConfig
{
    private const string ConfigFileName = "settings.config";

    static public Dictionary<string, string> LoadConfig()
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

    public static string ConfigPath
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