using Data.Network;
using Unity.Netcode;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerView), typeof(PlayerNetworkData))]
    public class PlayerController : NetworkBehaviour
    {
        public PlayerView PlayerView { get; private set; }
        public PlayerNetworkData PlayerNetworkData { get; private set; }

        private void Awake()
        {
            PlayerView = GetComponent<PlayerView>();
            PlayerNetworkData = GetComponent<PlayerNetworkData>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            NetworkPlayerManager.Instance.Register(this);
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            NetworkPlayerManager.Instance.Unregister(this);
        }
    }
}