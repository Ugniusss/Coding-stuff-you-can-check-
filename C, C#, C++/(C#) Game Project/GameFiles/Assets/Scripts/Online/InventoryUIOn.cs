using System.Collections;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class InventoryUIOn : MonoBehaviour
{
    public enum TargetActor
    {
        Player1 = 1,
        Player2 = 2
    }

    [Header("Target Actor")]
    public TargetActor targetActor;

    [Header("UI Texts")]
    public TMP_Text goldText;
    public TMP_Text swordText;
    public TMP_Text shieldText;
    public TMP_Text bowText;

    private InventoryOn inventory;

    void Start()
    {
        StartCoroutine(WaitForInventory());
    }

    IEnumerator WaitForInventory()
    {
        while (inventory == null)
        {
            inventory = FindTargetInventory();
            yield return null;
        }

        // Prisiregistruojam prie event
        inventory.OnInventoryChanged += Refresh;

        Refresh();
    }

    void OnDestroy()
    {
        if (inventory != null)
            inventory.OnInventoryChanged -= Refresh;
    }

    void Refresh()
    {
        goldText.text = inventory.GetItemCount(InventoryOn.ItemType.Gold).ToString();
        swordText.text = inventory.GetItemCount(InventoryOn.ItemType.Sword).ToString();
        shieldText.text = inventory.GetItemCount(InventoryOn.ItemType.Shield).ToString();
        bowText.text = inventory.GetItemCount(InventoryOn.ItemType.Bow).ToString();
    }

    InventoryOn FindTargetInventory()
    {
        foreach (InventoryOn inv in FindObjectsOfType<InventoryOn>())
        {
            PlayerMoverOn mover = inv.GetComponent<PlayerMoverOn>();
            if (mover == null) continue;

            if ((int)targetActor == mover.ownerActor)
                return inv;
        }
        return null;
    }
}
