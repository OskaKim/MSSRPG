using Monster;
using UnityEngine;

namespace Player
{
    public class PlayerAttackPerformer
    {
        public readonly Vector2 AttackSize = new Vector2(0.5f, 0.5f);
        private readonly Vector2 _attackOffset = new Vector2(0.5f, 0.5f); // 캐릭터 중심에서 얼마나 떨어져 있는지
        // public LayerMask targetLayer;

        public Vector2 Position { get; private set; }

        public void PerformAttack(Vector2 currentPosition, Vector2 direction)
        {
            Position = currentPosition + direction * _attackOffset;

            // _position: 판정 중심, 0f: 회전 각도, attackSize: 판정 크기, targetLayer: 대상 레이어
            var hitTargets = Physics2D.OverlapBoxAll(Position, AttackSize, 0f);

            foreach (var target in hitTargets)
            {
                // 몬스터에게 대미지 주는 로직 실행
                if (target.TryGetComponent<MonsterDamageReceiver>(out var monsterDamageReceiver))
                {
                    monsterDamageReceiver.TakeDamage(10);
                }
            }
        }
    }
}