using Unity.Netcode;
using UnityEngine;
using R3;

namespace Monster
{
    public class MonsterView : NetworkBehaviour
    {
        [SerializeField] private MonsterDamageReceiver damageReceiver;

        private readonly NetworkVariable<int> _hp = new NetworkVariable<int>(100);

        public Observable<Unit> OnDespawnAsObservable => despawnSubject;
        private readonly Subject<Unit> despawnSubject = new();

        private void Awake()
        {
            damageReceiver
                .OnTakeDamageAsObservable
                .Subscribe(OnTakeDamage)
                .AddTo(this);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _hp.OnValueChanged += OnHpChanged;
        }

        public override void OnNetworkDespawn()
        {
            _hp.OnValueChanged -= OnHpChanged;
            base.OnNetworkDespawn();
        }

        private void OnTakeDamage(int damage)
        {
            if (IsServer)
            {
                ProcessDamage(damage);
                return;
            }

            TakeDamageServerRpc(damage);
        }

        [ServerRpc(RequireOwnership = false)]
        private void TakeDamageServerRpc(int damage)
        {
            ProcessDamage(damage);
        }

        private void ProcessDamage(int damage)
        {
            _hp.Value = Mathf.Max(_hp.Value - damage, 0);

            if (_hp.Value <= 0)
            {
                despawnSubject.OnNext(Unit.Default);
                GetComponent<NetworkObject>().Despawn(true);
            }
        }

        private void OnHpChanged(int previousHp, int currentHp)
        {
            Debug.Log($"현재 HP : {currentHp}");
        }
    }
}