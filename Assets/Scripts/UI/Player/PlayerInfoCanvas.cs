using UnityEngine;

namespace UI.Player
{
    public class PlayerInfoCanvas : MonoBehaviour
    {
        private static PlayerInfoCanvas _singleton;
        public static PlayerInfoCanvas Get() => _singleton;

        private void Awake()
        {
            _singleton = this;
        }
    }
}