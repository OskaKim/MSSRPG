using Data.Network;
using R3;
using Settings;
using UI.Player;
using Unity.Netcode;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerNetworkData))]
    public class PlayerView : NetworkBehaviour
    {
        private PlayerInputActions _actions;
        [SerializeField] private float speed = 5f;
        [SerializeField] private PlayerInfoView playerInfoViewPrefab;

        private PlayerInfoView _playerInfoView;

        private void Awake()
        {
            _actions = new PlayerInputActions();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsServer) return;
            _playerInfoView = Instantiate(playerInfoViewPrefab, PlayerInfoCanvas.Get().transform);
            _playerInfoView.Initialize(GetComponent<PlayerNetworkData>(), transform);
        }

        private void OnEnable() => _actions.Enable();
        private void OnDisable() => _actions.Disable();

        private void Update()
        {
            if (!IsOwner) return;

            var move = _actions.Player.Move.ReadValue<Vector2>();
            transform.Translate(new Vector3(move.x, move.y, 0f) * speed * Time.deltaTime);
        }
    }
}