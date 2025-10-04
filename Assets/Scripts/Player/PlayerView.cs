using Character;
using Data.Network;
using Helper.System;
using Settings;
using UI.Player;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using R3;

namespace Player
{
    public enum PlayerState
    {
        Idle,
        Walk,
        Run,
        Attack
    }

    [RequireComponent(typeof(PlayerNetworkData))]
    public class PlayerView : NetworkBehaviour
    {
        private static readonly int AnimationX = Animator.StringToHash("X");
        private static readonly int AnimationY = Animator.StringToHash("Y");
        private static readonly int AnimationMoving = Animator.StringToHash("Moving");
        private static readonly int AnimationAttackTrigger = Animator.StringToHash("Attack");

        [SerializeField] private float walkSpeed = 1f;
        [SerializeField] private float runSpeed = 2f;
        [SerializeField] private PlayerInfoView playerInfoViewPrefab;
        [SerializeField] private NetworkAnimator networkAnimator;
        [SerializeField] private CharacterAnimationView CharacterAnimationView;

        private readonly NetworkVariable<PlayerState> _currentState = new(PlayerState.Idle);

        private PlayerInfoView _playerInfoView;
        private PlayerInputActions _actions;
        private Vector3 _previousPosition = Vector3.zero;
        private PlayerAttackPerformer _attackPerformer = new();

        private void Awake()
        {
            _previousPosition = transform.localPosition;
            _actions = new PlayerInputActions();

            // 캐릭터 생성 직후엔 아래를 본 상태로 설정.
            DirectionAnimationSetAs(0f, -1f);

            CharacterAnimationView
                .OnAnimationFinishedAsObservable
                .Subscribe(_ => OnAttackAnimationFinished())
                .AddTo(this);
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
            }

            if (_currentState.Value == PlayerState.Attack)
            {
                // 공격 모션 재생중.
                return;
            }

            OwnerUpdate();
        }

        private void OwnerUpdate()
        {
            if (!IsOwner)
            {
                return;
            }

            if (InputActionSystemHelper.HasPress(_actions.Player.Attack))
            {
                UpdateStateServerRpc(PlayerState.Attack);
                networkAnimator.SetTrigger(AnimationAttackTrigger);
                _attackPerformer.PerformAttack(transform.position, _lastMoveVector);
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

            if (InputActionSystemHelper.HasPress(_actions.Player.Sprint))
            {
                state = PlayerState.Run;
            }

            return state;
        }

        // 공격 애니메이션 종료
        private void OnAttackAnimationFinished()
        {
            if (!IsOwner)
            {
                return;
            }

            if (_currentState.Value != PlayerState.Attack)
            {
                return;
            }

            // idle상태로 되돌림
            const PlayerState newState = PlayerState.Idle;
            ApplyStateToAnimation(_currentState.Value, newState);
            UpdateStateServerRpc(newState);
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

            CharacterAnimationView.Animator.SetBool(AnimationMoving, state is PlayerState.Walk or PlayerState.Run);
        }

        private Vector3 _lastMoveVector;

        // 이동 방향을 감지하여 애니메이션 갱신
        private void ApplyDirectionToAnimation()
        {
            var moveVector = (transform.localPosition - _previousPosition).normalized;
            if (moveVector == Vector3.zero) return;

            _lastMoveVector = moveVector;
            _previousPosition = transform.localPosition;
            DirectionAnimationSetAs(_lastMoveVector.x, _lastMoveVector.y);
        }

        private void DirectionAnimationSetAs(float x, float y)
        {
            CharacterAnimationView.Animator.SetFloat(AnimationX, x);
            CharacterAnimationView.Animator.SetFloat(AnimationY, y);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(_attackPerformer.Position, _attackPerformer.AttackSize);
        }
    }
}