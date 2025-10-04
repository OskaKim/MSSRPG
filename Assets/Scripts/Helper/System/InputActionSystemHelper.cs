using UnityEngine.InputSystem;

namespace Helper.System
{
    public static class InputActionSystemHelper
    {
        public static bool HasPress(InputAction inputAction)
        {
            return inputAction.phase is InputActionPhase.Performed or InputActionPhase.Started;
        }
    }
}