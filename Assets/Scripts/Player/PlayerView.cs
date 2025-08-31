using Data.Network;
using Settings;
using UI.Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public enum PlayerState
    {
        Idle,
        Walk,
        Run
    }

    [RequireComponent(typeof(PlayerNetworkData))]
    public class PlayerView : NetworkBehaviour
    {
        private static readonly int AnimationX = Animator.StringToHash("X");
        private static readonly int AnimationY = Animator.StringToHash("Y");
        private static readonly int AnimationMoving = Animator.StringToHash("Moving");

        [SerializeField] private float walkSpeed = 1f;
        [SerializeField] private float runSpeed = 2f;
        [SerializeField] private PlayerInfoView playerInfoViewPrefab;
        [SerializeField] private Animator animator;

        private readonly NetworkVariable<PlayerState> _currentState = new(PlayerState.Idle);
        private PlayerInfoView _playerInfoView;
        private PlayerInputActions _actions;
        private Vector3 _previousPosition = Vector3.zero;

        private void Awake()
        {
            _previousPosition = transform.localPosition;
            _actions = new PlayerInputActions();

            // 캐릭터 생성 직후엔 아래를 본 상태로 설정.
            DirectionAnimationSetAs(0f, -1f);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsServer) return;
            _playerInfoView = Instantiate(playerInfoViewPrefab, PlayerInfoCanvas.Get().transform);
            _playerInfoView.Initialize(GetComponent<PlayerNetworkData>(), transform);
            _currentState.OnValueChanged += OnStateChanged;
        }

        public override void OnNetworkDespawn()
        {
            _currentState.OnValueChanged -= OnStateChanged;
            base.OnNetworkDespawn();
        }

        private void OnEnable() => _actions.Enable();
        private void OnDisable() => _actions.Disable();

        private void Update()
        {
            if (!IsOwner)
            {
                ApplyDirectionToAnimation();
                return;
            }

            var moveDirection = CalcMoveDirection();
            var newState = CalcState(moveDirection);

            var speed = newState == PlayerState.Run ? runSpeed : walkSpeed;
            transform.Translate(moveDirection * speed * Time.deltaTime);

            if (newState != _currentState.Value)
            {
                ApplyStateToAnimation(_currentState.Value, newState);

                // 다른 유저의 출력 적용을 위해 rpc
                UpdateStateServerRpc(newState);
            }

            ApplyDirectionToAnimation();
        }

        private Vector2 CalcMoveDirection()
        {
            var rawInput = _actions.Player.Move.ReadValue<Vector2>();
            var moveDirection = Vector2.zero;

            if (Mathf.Abs(rawInput.x) > Mathf.Abs(rawInput.y))
            {
                moveDirection.x = rawInput.x;
            }
            else
            {
                moveDirection.y = rawInput.y;
            }

            return moveDirection;
        }

        private PlayerState CalcState(Vector2 moveDirection)
        {
            var state = moveDirection == Vector2.zero ? PlayerState.Idle : PlayerState.Walk;
            if (state is not (PlayerState.Idle or PlayerState.Walk))
            {
                return state;
            }

            var sprintPhase = _actions.Player.Sprint.phase;
            if (sprintPhase is InputActionPhase.Performed or InputActionPhase.Started)
            {
                state = PlayerState.Run;
            }

            return state;
        }

        [ServerRpc]
        private void UpdateStateServerRpc(PlayerState newState)
        {
            // 서버에서 NetworkVariable의 값을 변경합니다.
            // 이 값은 모든 클라이언트에게 자동으로 동기화됩니다.
            _currentState.Value = newState;
        }

        private void OnStateChanged(PlayerState previousState, PlayerState newState)
        {
            if (!IsOwner)
            {
                ApplyStateToAnimation(previousState, newState);
            }
        }

        private void ApplyStateToAnimation(PlayerState previousState, PlayerState state)
        {
            Debug.Log($"[ID:{OwnerClientId}] character apply state to animation : {previousState} => {state}");

            if (animator == null) return;
            animator.SetBool(AnimationMoving, state is PlayerState.Walk or PlayerState.Run);
        }

        // 이동 방향을 감지하여 애니메이션 갱신
        private void ApplyDirectionToAnimation()
        {
            if (animator == null) return;

            var moveVector = (transform.localPosition - _previousPosition).normalized;
            if (moveVector == Vector3.zero) return;

            _previousPosition = transform.localPosition;
            DirectionAnimationSetAs(moveVector.x, moveVector.y);
        }

        private void DirectionAnimationSetAs(float x, float y)
        {
            animator.SetFloat(AnimationX, x);
            animator.SetFloat(AnimationY, y);
        }
    }
}