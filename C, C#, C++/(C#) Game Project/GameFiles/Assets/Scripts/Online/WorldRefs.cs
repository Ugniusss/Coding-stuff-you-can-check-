using UnityEngine;

public class WorldRefs : MonoBehaviour
{
    public static WorldRefs Instance;

    public Transform cities;
    public Transform redPaths;
    public Transform bluePaths;
    public Transform yellowPaths;

    void Awake()
    {
        Instance = this;
    }
}
