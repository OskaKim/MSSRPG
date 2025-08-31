using Unity.Netcode;
using UnityEngine;

public class PotionUser : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        Debug.Log("OnNetworkSpawn");
        if (IsClient && IsOwner)
        {
            Debug.Log("Client connected. Sending potion request...");
            UsePotionServerRpc();
        }
    }

    [ServerRpc]
    private void UsePotionServerRpc(ServerRpcParams rpcParams = default)
    {
        Debug.Log($"[SERVER] Player {OwnerClientId} used a potion!");
    }
}