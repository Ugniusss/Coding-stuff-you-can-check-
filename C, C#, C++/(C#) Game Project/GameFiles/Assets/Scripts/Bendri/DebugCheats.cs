using Photon.Pun;
using UnityEngine;

public class DebugCheats : MonoBehaviour
{
    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.P)) return;

        // Offline (vs Player / vs Bot)
        if (GameManager.Instance?.players != null)
        {
            foreach (var player in GameManager.Instance.players)
            {
                if (player == null) continue;
                var inv = player.GetComponent<Inventory>();
                var mgr = player.GetComponent<AbilityManager>();

                if (inv != null)
                {
                    inv.ModifyItem(Inventory.ItemType.Gold,   10);
                    inv.ModifyItem(Inventory.ItemType.Sword,  10);
                    inv.ModifyItem(Inventory.ItemType.Shield, 10);
                    inv.ModifyItem(Inventory.ItemType.Bow,    10);
                }

                if (mgr != null)
                    foreach (AbilityType t in System.Enum.GetValues(typeof(AbilityType)))
                        for (int i = 0; i < 3; i++) mgr.GrantFreeAbilityCharge(t);
            }
        }

        // Online — master fires RPCs, all clients receive
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
        {
            foreach (var kv in PlayerMoverOn.MoversByActor)
            {
                var mover = kv.Value;
                if (mover == null) continue;

                foreach (InventoryOn.ItemType t in System.Enum.GetValues(typeof(InventoryOn.ItemType)))
                    mover.photonView.RPC(nameof(PlayerMoverOn.RPC_ModifyItem), RpcTarget.All, (int)t, 10);

                foreach (AbilityType t in System.Enum.GetValues(typeof(AbilityType)))
                    for (int i = 0; i < 3; i++)
                        mover.photonView.RPC(nameof(PlayerMoverOn.RPC_GrantAbilityCharge), RpcTarget.All, (int)t);
            }
        }
    }
}
