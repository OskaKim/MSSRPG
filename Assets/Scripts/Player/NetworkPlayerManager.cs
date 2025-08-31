using System.Collections.Generic;
using Data.Network;

namespace Player
{
    public class NetworkPlayerManager
    {
        private static NetworkPlayerManager _instance;

        public static NetworkPlayerManager Instance => _instance ??= new NetworkPlayerManager();

        private readonly Dictionary<ulong, PlayerController> _players = new();

        private NetworkPlayerManager()
        {
        }

        public void Register(PlayerController root)
        {
            var clientId = root.OwnerClientId;
            _players.TryAdd(clientId, root);
        }

        public void Unregister(PlayerController root)
        {
            var clientId = root.OwnerClientId;
            if (_players.ContainsKey(clientId))
            {
                _players.Remove(clientId);
            }
        }

        public PlayerController GetPlayer(ulong clientId)
        {
            _players.TryGetValue(clientId, out var root);
            return root;
        }
    }
}