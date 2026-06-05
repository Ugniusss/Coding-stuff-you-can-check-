using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    public Transform redPathsParent;
    public Transform bluePathsParent;
    public Transform yellowPathsParent;

    private Dictionary<string, List<string>> redPaths = new();
    private Dictionary<string, List<string>> bluePaths = new();
    private Dictionary<string, List<string>> yellowPaths = new();

    public static PathFinder Instance;

    void Awake()
    {
        Instance = this;
        BuildPathMap(redPathsParent, redPaths, "Red");
        BuildPathMap(bluePathsParent, bluePaths, "Blue");
        BuildPathMap(yellowPathsParent, yellowPaths, "Yellow");
    }

    void BuildPathMap(Transform parent, Dictionary<string, List<string>> map, string color)
    {
        foreach (Transform pathObj in parent)
        {
            string[] split = pathObj.name.Split('_')[0].Split('-');
            if (split.Length != 2) continue;

            string a = split[0];
            string b = split[1];

            if (!map.ContainsKey(a)) map[a] = new List<string>();
            if (!map.ContainsKey(b)) map[b] = new List<string>();

            map[a].Add(b);
            map[b].Add(a);

            //Debug.Log($"{color} path added: {a} ↔ {b}");
        }
    }
    public List<string> GetConnections(string fromCity, string color)
    {
        Dictionary<string, List<string>> map = color switch
        {
            "Red" => redPaths,
            "Blue" => bluePaths,
            "Yellow" => yellowPaths,
            _ => null
        };

        if (map == null || !map.ContainsKey(fromCity))
            return null;

        return map[fromCity];
    }

    public string GetConnectedCity(string fromCity, string color)
    {
        Dictionary<string, List<string>> map = color switch
        {
            "Red" => redPaths,
            "Blue" => bluePaths,
            "Yellow" => yellowPaths,
            _ => null
        };

        if (map == null || !map.ContainsKey(fromCity) || map[fromCity].Count == 0)
        {
            //Debug.LogWarning($"No {color} path from {fromCity}");
            return null;
        }

        string next = map[fromCity][0]; // always pick the first connected for now
        //Debug.Log($"Moving from {fromCity} to {next} via {color}");
        return next;
    }
}
