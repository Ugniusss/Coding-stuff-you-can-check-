/*
PLACES MOBPOSITION IN THE CIRCLES MIDDLE
*/
using UnityEngine;

public class MobSpotPlacer : MonoBehaviour
{
    public string visualMarkerName = "VisualMarker";

    public float heightOffset = 0.1f;

    void Start()
    {

        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("MobPos"))
                Destroy(child.gameObject);
        }

        Transform visualMarker = transform.Find(visualMarkerName);
        if (visualMarker == null)
        {
            //Debug.LogWarning("MobSpotPlacer: No VisualMarker found under " + gameObject.name);
            return;
        }

        float topY = visualMarker.position.y;
        MeshRenderer mr = visualMarker.GetComponent<MeshRenderer>();
        if (mr != null)
            topY = mr.bounds.max.y;

        Vector3 spawnPos = new Vector3(
            visualMarker.position.x,
            topY + heightOffset,
            visualMarker.position.z
        );

        GameObject mobSpot = new GameObject("MobPos1");
        mobSpot.transform.parent = transform;
        mobSpot.transform.position = spawnPos;

       // Debug.Log("MobPos1 created at " + spawnPos);
    }
}
