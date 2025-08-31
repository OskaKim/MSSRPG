using Player;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Data.Network
{
    public class PlayerNetworkData : NetworkBehaviour
    {
        public NetworkVariable<FixedString32Bytes> PlayerName { get; } = new("플레이어1");

        public NetworkVariable<float> Hp = new(100f, writePerm: NetworkVariableWritePermission.Owner);

        [ServerRpc(RequireOwnership = false)]
        public void SetPlayerNameServerRpc(FixedString32Bytes playerName)
        {
            PlayerName.Value = playerName;
        }

        public const float MaxHp = 100f;
    }
}