using System.Collections.Generic;
using UnityEngine;

public class PathHighlighter : MonoBehaviour
{
    public static PathHighlighter Instance;
    public Transform citiesParent;

    private readonly Dictionary<string, GameObject> highlights = new();
    private readonly Dictionary<string, Material>   materials  = new();

    void Awake()
    {
        Instance = this;
        BuildHighlights();
    }

    void BuildHighlights()
    {
        if (citiesParent == null) return;

        foreach (Transform city in citiesParent)
        {
            string cityName = city.name.Replace("City_", "");
            Transform marker = city.Find("PlayerPositions/VisualMarker");
            if (marker == null) continue;

            var disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Destroy(disc.GetComponent<Collider>());
            disc.transform.SetParent(marker, false);
            disc.transform.localPosition = new Vector3(0, -0.05f, 0);
            disc.transform.localScale    = new Vector3(1.4f, 0.04f, 1.4f);

            // Sprites/Default supports alpha transparency at runtime without extra setup
            var mat = new Material(Shader.Find("Sprites/Default"));
            mat.color = new Color(1f, 1f, 1f, 0f);

            // Assign ONCE and cache — never call .material again (it creates new instances)
            disc.GetComponent<Renderer>().sharedMaterial = mat;
            materials[cityName] = mat;

            disc.SetActive(false);
            highlights[cityName] = disc;
        }
    }

    public void ShowFor(string fromCity, string color)
    {
        HideAll();
        if (PathFinder.Instance == null) return;

        var connections = PathFinder.Instance.GetConnections(fromCity, color);
        if (connections == null) return;

        Color hlColor = color switch
        {
            "Red"    => new Color(1f, 0.3f, 0.3f, 0.6f),
            "Blue"   => new Color(0.2f, 0.55f, 1f, 0.6f),
            "Yellow" => new Color(1f, 0.85f, 0f, 0.6f),
            _        => new Color(1f, 1f, 1f, 0.4f)
        };

        foreach (string city in connections)
        {
            if (highlights.TryGetValue(city, out var disc) && materials.TryGetValue(city, out var mat))
            {
                mat.color = hlColor; // modify cached material — no new instance
                disc.SetActive(true);
            }
        }
    }

    public void HideAll()
    {
        foreach (var disc in highlights.Values)
            disc.SetActive(false);
    }
}
