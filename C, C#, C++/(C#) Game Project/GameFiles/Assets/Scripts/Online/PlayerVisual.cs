using Photon.Pun;
using UnityEngine;

public class OnlinePlayerVisual : MonoBehaviourPun
{
    public GameObject modelP1;
    public GameObject modelP2;

    void Start()
    {
        // SAUGIKLIAI
        if (modelP1 == null || modelP2 == null)
        {
            Debug.LogError("[OnlinePlayerVisual] Model references missing");
            return;
        }

        modelP1.SetActive(true);
        modelP2.SetActive(true);

        if (photonView.Owner == null)
        {
            Debug.LogWarning("[OnlinePlayerVisual] Owner not ready, showing both models");
            return;
        }

        int actor = photonView.Owner.ActorNumber;
        Debug.Log("[OnlinePlayerVisual] ActorNumber = " + actor);

        if (actor == 1)
        {
            modelP1.SetActive(true);
            modelP2.SetActive(false);
        }
        else if (actor == 2)
        {
            modelP1.SetActive(false);
            modelP2.SetActive(true);
        }
        else
        {
            Debug.LogWarning("[OnlinePlayerVisual] Unknown actor, showing both");
            modelP1.SetActive(true);
            modelP2.SetActive(true);
        }
    }
}