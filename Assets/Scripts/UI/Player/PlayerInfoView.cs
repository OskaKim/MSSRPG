using Data.Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Player
{
    public class PlayerInfoView : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerNameText;
        [SerializeField] private Image playerHp;
        [SerializeField] private Vector3 worldOffset = new Vector3(0, 2f, 0); // 머리 위 위치

        private Camera _mainCamera;
        private Transform _target; // 따라갈 플레이어 위치
        private PlayerNetworkData _playerNetworkData;

        public void Initialize(PlayerNetworkData networkData, Transform target)
        {
            _playerNetworkData = networkData;
            _target = target;
            _mainCamera = Camera.main;

            // 초기 표시
            playerNameText.text = networkData.PlayerName.Value.ToString();
            playerHp.fillAmount = networkData.Hp.Value / PlayerNetworkData.MaxHp;

            // 네트워크 변수 변경 시 UI 업데이트
            networkData.PlayerName.OnValueChanged += OnNameChanged;
            networkData.Hp.OnValueChanged += OnHpChanged;
        }

        private void OnDestroy()
        {
            if (_playerNetworkData != null)
            {
                _playerNetworkData.PlayerName.OnValueChanged -= OnNameChanged;
                _playerNetworkData.Hp.OnValueChanged -= OnHpChanged;
            }
        }

        private void Update()
        {
            if (_target == null || _mainCamera == null) return;

            // 월드 좌표 → 스크린 좌표 변환
            Vector3 screenPos = _mainCamera.WorldToScreenPoint(_target.position + worldOffset);
            transform.position = screenPos;
        }

        private void OnNameChanged(Unity.Collections.FixedString32Bytes oldValue, Unity.Collections.FixedString32Bytes newValue)
        {
            playerNameText.text = newValue.ToString();
        }

        private void OnHpChanged(float oldValue, float newValue)
        {
            playerHp.fillAmount = newValue / PlayerNetworkData.MaxHp;
        }
    }
}