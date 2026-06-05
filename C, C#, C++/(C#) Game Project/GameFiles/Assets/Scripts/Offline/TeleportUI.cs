using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeleportUI : MonoBehaviour
{
    public static TeleportUI Instance;

    public GameObject panel;
    public Button closeBtn;
    public Button chestBtn;
    public Button towerBtn;
    public Button gamblerBtn;
    public Button marketBtn;

    // Dynamic scroll-list mode (used when category buttons are absent)
    private Transform cityListContent;
    private GameObject cityBtnTemplate;

    // Set by Online system before OpenPicker — city stored here instead of sliding
    public static string OnlinePendingCity = null;
    public static bool IsPicking { get; private set; }

    private PlayerMover targetPlayer;

    void Awake() { Instance = this; }

    bool _started = false;
    void Start() => EnsureStarted();

    void EnsureStarted()
    {
        if (_started) return;
        _started = true;

        if (panel == null) { Debug.LogError("[TeleportUI] panel is null!"); return; }
        panel.SetActive(false);

        if (chestBtn   == null) chestBtn   = FindBtn("Chest")   ?? FindBtnInContainer("Chest");
        if (towerBtn   == null) towerBtn   = FindBtn("Tower")   ?? FindBtnInContainer("Tower");
        if (gamblerBtn == null) gamblerBtn = FindBtn("Gambler") ?? FindBtnInContainer("Gambler");
        if (marketBtn  == null) marketBtn  = FindBtn("Market")  ?? FindBtnInContainer("Market");
        if (closeBtn   == null) closeBtn   = FindBtn("CloseBtn") ?? FindBtnInContainer("CloseBtn");

        // Detect scroll-list layout when category buttons are absent
        if (chestBtn == null)
        {
            var t = panel.transform.Find("ScrollView/Viewport/Content");
            if (t != null)
            {
                cityListContent = t;
                var tmpl = t.Find("CityBtnTemplate");
                if (tmpl != null) { cityBtnTemplate = tmpl.gameObject; cityBtnTemplate.SetActive(false); }
            }
        }

        closeBtn?.onClick.AddListener(Close);
        chestBtn  ?.onClick.AddListener(() => HandleCategory(FindChestCities));
        towerBtn  ?.onClick.AddListener(() => HandleCategory(p => FindTaggedCities(p, MobObjectSpawner.towerCities)));
        gamblerBtn?.onClick.AddListener(() => HandleCategory(p => FindBuildingCities(p, BuildingType.WanderingGambler)));
        marketBtn ?.onClick.AddListener(() => HandleCategory(p => FindBuildingCities(p, BuildingType.Market)));

        bool dynamicMode = cityListContent != null;
        if (!dynamicMode)
        {
            if (chestBtn   == null) Debug.LogError("[TeleportUI] 'Chest' button not found");
            if (towerBtn   == null) Debug.LogError("[TeleportUI] 'Tower' button not found");
            if (gamblerBtn == null) Debug.LogError("[TeleportUI] 'Gambler' button not found");
            if (marketBtn  == null) Debug.LogError("[TeleportUI] 'Market' button not found");
        }
    }

    Button FindBtn(string btnName)
    {
        foreach (var b in GetComponentsInChildren<Button>(true))
            if (b.name == btnName) return b;
        return null;
    }

    // Fallback: find the first Button inside a container named containerName
    Button FindBtnInContainer(string containerName)
    {
        foreach (Transform t in GetComponentsInChildren<Transform>(true))
            if (t.name == containerName) return t.GetComponentInChildren<Button>(true);
        return null;
    }

    public void OpenPicker(PlayerMover player)
    {
        Instance = this;
        gameObject.SetActive(true);
        EnsureStarted();
        targetPlayer = player;

        if (player == null)
        {
            IsPicking = true;
            if (cityListContent != null)
                PopulateCityListOnline();
            else
                SetupCategoryButtonsOnline(); // category-button layout (Online scene)
        }
        else if (cityListContent != null)
        {
            PopulateCityList(player);
        }
        else
        {
            SetInteractable(chestBtn,   FindChestCities(player).Count > 0);
            SetInteractable(towerBtn,   FindTaggedCities(player, MobObjectSpawner.towerCities).Count > 0);
            SetInteractable(gamblerBtn, FindBuildingCities(player, BuildingType.WanderingGambler).Count > 0);
            SetInteractable(marketBtn,  FindBuildingCities(player, BuildingType.Market).Count > 0);
        }
        panel.SetActive(true);
    }

    void PopulateCityListOnline()
    {
        if (cityBtnTemplate == null || cityListContent == null) return;

        for (int i = cityListContent.childCount - 1; i >= 0; i--)
        {
            var child = cityListContent.GetChild(i);
            if (child.gameObject != cityBtnTemplate) Destroy(child.gameObject);
        }

        int localActor = Photon.Pun.PhotonNetwork.LocalPlayer.ActorNumber;
        if (!PlayerMoverOn.MoversByActor.TryGetValue(localActor, out var moverOn)) return;

        Transform cp = moverOn.citiesParent;
        if (cp == null) return;

        foreach (Transform city in cp)
        {
            string cityName = city.name.Replace("City_", "");
            if (cityName == moverOn.currentCityName) continue;

            GameObject btn = Instantiate(cityBtnTemplate, cityListContent);
            btn.SetActive(true);
            var label = btn.GetComponentInChildren<TMP_Text>();
            if (label != null) label.text = cityName;
            string captured = cityName;
            btn.GetComponent<Button>()?.onClick.AddListener(() =>
            {
                OnlinePendingCity = captured;
                Close();
            });
        }
    }

    void PopulateCityList(PlayerMover player)
    {
        if (cityBtnTemplate == null || cityListContent == null) return;

        for (int i = cityListContent.childCount - 1; i >= 0; i--)
        {
            var child = cityListContent.GetChild(i);
            if (child.gameObject != cityBtnTemplate) Destroy(child.gameObject);
        }

        if (player.citiesParent == null) return;

        foreach (Transform city in player.citiesParent)
        {
            string cityName = city.name.Replace("City_", "");
            if (cityName == player.currentCityName) continue;
            Transform marker = player.FindCityMarker(cityName);
            if (marker == null) continue;

            GameObject btn = Instantiate(cityBtnTemplate, cityListContent);
            btn.SetActive(true);
            var label = btn.GetComponentInChildren<TMP_Text>();
            if (label != null) label.text = cityName;

            string capturedCity = cityName;
            Vector3 capturedPos = marker.position;
            btn.GetComponent<Button>()?.onClick.AddListener(() =>
            {
                if (targetPlayer == null) return;
                targetPlayer.currentCityName = capturedCity;
                targetPlayer.StartCoroutine(targetPlayer.SlideTo(capturedPos));
                Close();
            });
        }
    }

    void HandleCategory(Func<PlayerMover, List<(string, Transform)>> finder)
    {
        if (targetPlayer == null) return;
        var targets = finder(targetPlayer);
        if (targets.Count > 0) TeleportToNearest(targets);
    }

    void SetInteractable(Button btn, bool on) { if (btn != null) btn.interactable = on; }

    List<(string name, Transform marker)> FindChestCities(PlayerMover player)
    {
        var result = new List<(string, Transform)>();
        if (player.citiesParent == null) return result;

        foreach (Transform city in player.citiesParent)
        {
            string cityName = city.name.Replace("City_", "");
            if (cityName == player.currentCityName) continue;

            Transform mobPos = city.Find("MobPositions");
            if (mobPos == null) continue;

            foreach (Transform child in mobPos)
            {
                if (!child.CompareTag("Chest")) continue;
                Transform marker = player.FindCityMarker(cityName);
                if (marker != null) result.Add((cityName, marker));
                break;
            }
        }
        return result;
    }

    List<(string name, Transform marker)> FindTaggedCities(PlayerMover player, HashSet<string> citySet)
    {
        var result = new List<(string, Transform)>();
        foreach (string cityName in citySet)
        {
            if (cityName == player.currentCityName) continue;
            Transform marker = player.FindCityMarker(cityName);
            if (marker != null) result.Add((cityName, marker));
        }
        return result;
    }

    List<(string name, Transform marker)> FindBuildingCities(PlayerMover player, BuildingType type)
    {
        var result = new List<(string, Transform)>();
        if (player.citiesParent == null) return result;

        foreach (Transform city in player.citiesParent)
        {
            string cityName = city.name.Replace("City_", "");
            if (cityName == player.currentCityName) continue;

            Transform mobPos = city.Find("MobPositions");
            if (mobPos == null) continue;

            foreach (Transform child in mobPos)
            {
                BuildingInteraction bi = child.GetComponent<BuildingInteraction>();
                if (bi == null || bi.buildingType != type) continue;
                Transform marker = player.FindCityMarker(cityName);
                if (marker != null) result.Add((cityName, marker));
                break;
            }
        }
        return result;
    }

    void TeleportToNearest(List<(string name, Transform marker)> targets)
    {
        if (targetPlayer == null) { Close(); return; }

        var closest = targets[0];
        float minDist = Vector3.Distance(targetPlayer.transform.position, closest.marker.position);
        foreach (var entry in targets)
        {
            float d = Vector3.Distance(targetPlayer.transform.position, entry.marker.position);
            if (d < minDist) { minDist = d; closest = entry; }
        }

        targetPlayer.currentCityName = closest.name;
        targetPlayer.StartCoroutine(targetPlayer.SlideTo(closest.marker.position));
        Close();
    }

    void SetupCategoryButtonsOnline()
    {
        int localActor = Photon.Pun.PhotonNetwork.LocalPlayer.ActorNumber;
        if (!PlayerMoverOn.MoversByActor.TryGetValue(localActor, out var moverOn)) return;
        Transform cp = moverOn.citiesParent;
        if (cp == null) return;

        Vector3 myPos = moverOn.transform.position;
        string myCity = moverOn.currentCityName;

        // Chests/towers: use global tag search — PhotonNetwork.Instantiate creates them on all clients
        // but SetParent (master-only) means they won't be in MobPositions on P2.
        // Buildings (gambler/market): use name search — spawned via RPC on all clients with correct parent.
        WireOnlineCategoryBtn(chestBtn,   FindNearestCityByGlobalTag(cp, myCity, myPos, "Chest"));
        WireOnlineCategoryBtn(towerBtn,   FindNearestCityByGlobalTag(cp, myCity, myPos, "Tower"));
        WireOnlineCategoryBtn(gamblerBtn, FindNearestCityByMobName(cp, myCity, myPos, "Gambler"));
        WireOnlineCategoryBtn(marketBtn,  FindNearestCityByMobName(cp, myCity, myPos, "Market"));
    }

    // Finds nearest city that has a tagged object near its MobPositions VisualMarker.
    // Uses global scene search — works on all clients regardless of parenting.
    string FindNearestCityByGlobalTag(Transform cp, string exclude, Vector3 from, string tag)
    {
        var taggedObjects = GameObject.FindGameObjectsWithTag(tag);
        if (taggedObjects.Length == 0) return null;

        string best = null;
        float bestDist = float.MaxValue;
        foreach (Transform cityT in cp)
        {
            string cityName = cityT.name.Replace("City_", "");
            if (cityName == exclude) continue;
            Transform mobMarker = cityT.Find("MobPositions/VisualMarker");
            if (mobMarker == null) continue;

            bool found = false;
            foreach (var go in taggedObjects)
                if (Vector3.Distance(go.transform.position, mobMarker.position) < 5f) { found = true; break; }
            if (!found) continue;

            Transform playerMarker = cityT.Find("PlayerPositions/VisualMarker");
            if (playerMarker == null) continue;
            float d = Vector3.Distance(from, playerMarker.position);
            if (d < bestDist) { bestDist = d; best = cityName; }
        }
        return best;
    }

    string FindNearestCityByMobName(Transform cp, string exclude, Vector3 from, string namePrefix)
    {
        string best = null;
        float bestDist = float.MaxValue;
        foreach (Transform cityT in cp)
        {
            string cityName = cityT.name.Replace("City_", "");
            if (cityName == exclude) continue;
            Transform mobPos = cityT.Find("MobPositions");
            if (mobPos == null) continue;
            bool found = false;
            foreach (Transform mob in mobPos)
                if (mob.name.StartsWith(namePrefix)) { found = true; break; }
            if (!found) continue;
            Transform marker = cityT.Find("PlayerPositions/VisualMarker");
            if (marker == null) continue;
            float d = Vector3.Distance(from, marker.position);
            if (d < bestDist) { bestDist = d; best = cityName; }
        }
        return best;
    }

    void WireOnlineCategoryBtn(Button btn, string cityName)
    {
        if (btn == null) return;
        btn.interactable = cityName != null;
        btn.onClick.RemoveAllListeners();
        if (cityName == null) return;
        string captured = cityName;
        btn.onClick.AddListener(() =>
        {
            OnlinePendingCity = captured;
            // Preview teleport locally so player sees new position before choosing moves
            int localActor = Photon.Pun.PhotonNetwork.LocalPlayer.ActorNumber;
            if (PlayerMoverOn.MoversByActor.TryGetValue(localActor, out var moverOn))
            {
                Transform m = moverOn.FindCityMarker(captured);
                if (m != null)
                {
                    moverOn.RPC_SlideTo(m.position); // direct call = local only, no network
                    moverOn.RPC_SetCity(captured);
                }
            }
            Close();
        });
    }

    void Close()
    {
        IsPicking = false;
        panel.SetActive(false);
        targetPlayer = null;
    }
}
