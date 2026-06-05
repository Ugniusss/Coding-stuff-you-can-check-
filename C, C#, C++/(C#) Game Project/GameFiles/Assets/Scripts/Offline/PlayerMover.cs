using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    public string currentCityName;
    public float moveSpeed = 2f;
    public PlayerClass playerClass = PlayerClass.Merchant;

    public Transform citiesParent;
    public Transform redPathsParent;
    public Transform bluePathsParent;
    public Transform yellowPathsParent;

    private string previousCity = null;
    //-----------TESTAS SU 1
    public IEnumerator FollowColorPathSequence(string[] pathColors, bool[] doInteractions)
    {

        yield return StartCoroutine(MoveAlongPath(pathColors, doInteractions));
    }


   
    IEnumerator MoveAlongPath(string[] pathColors, bool[] doInteractions)
    {
        string current = currentCityName;

        for (int i = 0; i < pathColors.Length; i++)
        {
            string color = pathColors[i];
            string nextCity = FindNextCityByColor(current, color);

            if (string.IsNullOrEmpty(nextCity)) continue;

            Transform nextMarker = FindCityMarker(nextCity);
            if (nextMarker != null)
            {
                //Debug.Log($"Moving from {current} to {nextCity} via {color}");
                yield return SlideTo(nextMarker.position);

                previousCity = current;
                current = nextCity;
                if (doInteractions[i])
                {
                    CheckForChestAtCity(current, true);
                }
            }
        }
        //Debug.Log("Movement complete");

        currentCityName = current;

    }
    public void CheckForChestAtCurrentCity(bool wantsInteraction)
    {
        // Bandit Den always triggers on landing — no player choice
        if (TryTriggerBanditDen(currentCityName)) return;

        if (!wantsInteraction) return;

        if (TryInteractWithBuilding(currentCityName)) return;
        bool chestFound = TryInteractWithChest(currentCityName);
        if (!chestFound)
            TryInteractWithTower(currentCityName);
    }

    bool TryTriggerBanditDen(string cityName)
    {
        if (!MobObjectSpawner.buildingCities.Contains(cityName)) return false;

        Transform city = citiesParent.Find("City_" + cityName);
        if (city == null) return false;
        Transform mobPos = city.Find("MobPositions");
        if (mobPos == null) return false;

        foreach (Transform child in mobPos)
        {
            BuildingInteraction building = child.GetComponent<BuildingInteraction>();
            if (building == null || building.buildingType != BuildingType.BanditDen) continue;

            if (ClassData.BanditDenImmune(playerClass))
            {
                Debug.Log("[BanditDen] Wanderer is immune.");
                return true;
            }

            building.OnPlayerEnter(this, GetPlayerIndex());
            return true;
        }
        return false;
    }

    bool TryInteractWithBuilding(string cityName)
    {
        if (!MobObjectSpawner.buildingCities.Contains(cityName)) return false;

        Transform city = citiesParent.Find("City_" + cityName);
        if (city == null) return false;

        Transform mobPos = city.Find("MobPositions");
        if (mobPos == null) return false;

        foreach (Transform child in mobPos)
        {
            BuildingInteraction building = child.GetComponent<BuildingInteraction>();
            if (building == null) continue;

            // Wanderer is immune to Bandit Den
            if (building.buildingType == BuildingType.BanditDen && ClassData.BanditDenImmune(playerClass))
            {
                Debug.Log("[BanditDen] Wanderer is immune.");
                return true;
            }

            int playerIndex = GetPlayerIndex();
            building.OnPlayerEnter(this, playerIndex);
            return true;
        }
        return false;
    }

    int GetPlayerIndex()
    {
        if (GameManager.Instance?.players == null) return 0;
        for (int i = 0; i < GameManager.Instance.players.Length; i++)
            if (GameManager.Instance.players[i] == this) return i;
        return 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Inventory inv = GetComponent<Inventory>();
            if (inv != null)
            {
                inv.ModifyItem(Inventory.ItemType.Gold, 3);
                inv.ModifyItem(Inventory.ItemType.Sword, 3);
                inv.ModifyItem(Inventory.ItemType.Shield, 3);
                inv.ModifyItem(Inventory.ItemType.Bow, 3);
            }
        }

        if (Input.GetKeyDown(KeyCode.P) && GetPlayerIndex() == 0)
        {
            foreach (PlayerMover p in GameManager.Instance.players)
            {
                Inventory inv = p.GetComponent<Inventory>();
                if (inv != null)
                {
                    inv.ModifyItem(Inventory.ItemType.Gold, 10);
                    inv.ModifyItem(Inventory.ItemType.Sword, 10);
                    inv.ModifyItem(Inventory.ItemType.Shield, 10);
                    inv.ModifyItem(Inventory.ItemType.Bow, 10);
                }
                AbilityManager am = p.GetComponent<AbilityManager>();
                if (am != null)
                {
                    foreach (AbilityType type in System.Enum.GetValues(typeof(AbilityType)))
                    {
                        am.GrantFreeAbilityCharge(type);
                        am.GrantFreeAbilityCharge(type);
                    }
                }
            }
            AbilityUI.Instance?.RefreshUI();
        }
    }

    void CheckForChestAtCity(string cityName, bool wantsToInteract)
    {
        if (!wantsToInteract)
        {
            return;
        }

        //Debug.Log("Checking for chest in city: " + cityName);

        if (MobObjectSpawner.chestCities.Contains(cityName))
        {
            //Debug.Log($"Chest found in city {cityName}! Applying reward...");
            ApplyRandomChestReward();
           

            Transform city = citiesParent.Find("City_" + cityName);
            if (city != null)
            {
                Transform mobPos = city.Find("MobPositions");
                if (mobPos != null)
                {
                    bool destroyed = false;
                    foreach (Transform child in mobPos)
                    {
                        //Debug.Log($"MobPos child: {child.name}, Tag: {child.tag}");

                        if (child.CompareTag("Chest"))
                        {
                            //Debug.Log($"Destroying chest: {child.name}");
                            Destroy(child.gameObject);
                            destroyed = true;
                            break;
                        }
                    }

                    if (!destroyed)
                    {
                        //Debug.LogWarning($"⚠No tagged 'Chest' found in {cityName}'s MobPositions.");
                    }
                }
            }

            MobObjectSpawner.chestCities.Remove(cityName);
            MobObjectSpawner.Instance.SpawnChestAtFreeCity();
        }



        else
        {
            //Debug.Log($"No chest to collect in city {cityName}.");
        }
    }



    void ApplyRandomChestReward()
    {
        Inventory inv = GetComponent<Inventory>();
        if (inv == null) return;

        int pIdx = GetPlayerIndex();

        if (Random.value < 0.15f)
        {
            AbilityManager am = GetComponent<AbilityManager>();
            if (am != null)
            {
                am.GrantFreeAbilityCharge();
                GameLog.Instance?.Add($"P{pIdx+1} opened chest — Ability Scroll");
                return;
            }
        }

        var items = new List<Inventory.ItemType> {
            Inventory.ItemType.Gold,
            Inventory.ItemType.Sword,
            Inventory.ItemType.Shield,
            Inventory.ItemType.Bow
        };

        Inventory.ItemType randomItem = items[Random.Range(0, items.Count)];
        int[] weights = { 1, 1, 2, 2, 3 };
        int amount = weights[Random.Range(0, weights.Length)];

        inv.ModifyItem(randomItem, amount);
        string sign = amount >= 0 ? "+" : "";
        GameLog.Instance?.Add($"P{pIdx+1} opened chest — {randomItem}");
    }

    public IEnumerator SlideTo(Vector3 targetPos)
    {
        Vector3 start = transform.position;
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(start, targetPos, t);
            yield return null;
        }

        transform.position = targetPos;
    }

    public string FindNextCityByColor(string fromCity, string color)
    {
        List<string> connections = PathFinder.Instance.GetConnections(fromCity, color);

        if (connections == null || connections.Count == 0)
        {
            //Debug.LogWarning($"No {color} connection from city '{fromCity}'");
            return null;
        }

        foreach (string city in connections)
        {
            if (city != previousCity)
            {
                //Debug.Log($"➡Found next {color} path from {fromCity} to {city}");
                return city;
            }
        }

        //Debug.LogWarning($"⚠All {color} paths from {fromCity} lead back to previous city {previousCity}. Using fallback.");
        return connections[0];
    }


    public Transform FindCityMarker(string cityName)
    {
        Transform city = citiesParent.Find("City_" + cityName);
        if (city == null)
        {
            //Debug.LogWarning("City not found: City_" + cityName);
            return null;
        }

        return city.Find("PlayerPositions/VisualMarker");
    }
    bool TryInteractWithTower(string cityName)
    {
        if (!MobObjectSpawner.towerCities.Contains(cityName))
        {
            //Debug.Log($"No tower in city {cityName}.");
            return false;
        }

        //Debug.Log($"Tower found in city {cityName}. Checking inventory...");

        Inventory inv = GetComponent<Inventory>();
        if (inv == null)
        {
            //Debug.LogWarning("No Inventory component found on player.");
            return false;
        }

        int swords = inv.GetItemCount(Inventory.ItemType.Sword);
        int shields = inv.GetItemCount(Inventory.ItemType.Shield);
        int bows = inv.GetItemCount(Inventory.ItemType.Bow);

        int threshold = ClassData.TowerThreshold(playerClass);
        if (swords >= threshold && shields >= threshold && bows >= threshold)
        {
            inv.ModifyItem(Inventory.ItemType.Sword, -threshold);
            inv.ModifyItem(Inventory.ItemType.Shield, -threshold);
            inv.ModifyItem(Inventory.ItemType.Bow, -threshold);
            inv.ModifyItem(Inventory.ItemType.Gold, 10);
            GameLog.Instance?.Add($"P{GetPlayerIndex()+1} defeated tower in {cityName}! (+10g)");
            DestroyTowerInCity(cityName);
            MobObjectSpawner.towerCities.Remove(cityName);

            //Debug.Log($"Tower taken down in city {cityName}, rewarded 10 gold.");
        }
        else
        {
            inv.ModifyItem(Inventory.ItemType.Gold, -5);
            GameLog.Instance?.Add($"P{GetPlayerIndex()+1} failed tower in {cityName} (-5g)");

        }
        
        return true;
    }

    bool TryInteractWithChest(string cityName)
    {
        if (!MobObjectSpawner.chestCities.Contains(cityName))
        {
            //Debug.Log($"No chest to collect in city {cityName}.");
            return false;
        }

        //Debug.Log($"Chest found in city {cityName}! Applying reward...");
        ApplyRandomChestReward();

        Transform city = citiesParent.Find("City_" + cityName);
        if (city != null)
        {
            Transform mobPos = city.Find("MobPositions");
            if (mobPos != null)
            {
                foreach (Transform child in mobPos)
                {
                    if (child.CompareTag("Chest"))
                    {
                        //Debug.Log($"Destroying chest: {child.name}");
                        Destroy(child.gameObject);
                        break;
                    }
                }
            }
        }

        MobObjectSpawner.chestCities.Remove(cityName);
        MobObjectSpawner.Instance.SpawnChestAtFreeCity();
        return true;
    }

    void DestroyTowerInCity(string cityName)
    {
        Transform city = citiesParent.Find("City_" + cityName);
        if (city == null) return;

        Transform mobPos = city.Find("MobPositions");
        if (mobPos == null) return;

        foreach (Transform child in mobPos)
        {
            if (child.CompareTag("Tower")) 
            {
                //Debug.Log($"Destroying tower: {child.name}");
                Destroy(child.gameObject);
                break;
            }
        }
    }

}
