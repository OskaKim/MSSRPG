using UnityEngine;
using R3;

namespace Monster
{
    public class MonsterDamageReceiver : MonoBehaviour
    {
        public Observable<int> OnTakeDamageAsObservable => _takeDamageSubject;
        private readonly Subject<int> _takeDamageSubject = new();

        public void TakeDamage(int damage)
        {
            _takeDamageSubject.OnNext(damage);
        }
    }
}