using System;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using R3;

namespace Monster
{
    public class MonsterGenerator : MonoBehaviour
    {
        [SerializeField] private MonsterView monsterViewPrefab;

        private void Start()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                SpawnMonster();
            }
        }

        private void SpawnMonster()
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                Debug.LogWarning("클라이언트에서는 몬스터를 생성할 수 없습니다.");
                return;
            }

            Debug.Log("몬스터 생성");

            var monsterInstance = Instantiate(
                monsterViewPrefab,
                transform.position,
                Quaternion.identity
            );

            var networkObject = monsterInstance.GetComponent<NetworkObject>();
            networkObject.Spawn();

            monsterInstance
                .OnDespawnAsObservable
                .Subscribe(_ => OnDespawnMonster())
                .AddTo(monsterInstance.gameObject);
        }

        private void OnDespawnMonster()
        {
            RespawnAsync().Forget();
        }

        private async UniTask RespawnAsync()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(5));
            SpawnMonster();
        }
    }
}