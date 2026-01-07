using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    public struct Controls
    { 
        private Vector3 moveDirection;
        public Vector3 MoveDirection { get { return moveDirection; } set { moveDirection = value; } }

        private Vector2 lookDirection;
        public Vector2 LookDirection { get { return lookDirection; } set { lookDirection = value; } }

        private bool hasAttacked;
        public bool HasAttacked { get { return hasAttacked; } set { hasAttacked = value; } }

        private bool hasInteracted;
        public bool HasInteracted { get { return hasInteracted; } set { hasInteracted = value; } }

        private bool hasDisengaged;
        public bool HasDisengaged { get { return hasDisengaged; } set { hasDisengaged = value; } }

    }
    public Controls controlEvent;

    private void Awake()
    {
        controlEvent = new Controls();
    }

    public void MoveValue(InputAction.CallbackContext context)
    {
        Vector2 placeHolder = context.action.ReadValue<Vector2>();
        controlEvent.MoveDirection = new Vector2(placeHolder.x, placeHolder.y);
    }

    public void LookValue(InputAction.CallbackContext context)
    {
        controlEvent.LookDirection = context.action.ReadValue<Vector2>();
    }

    public void Attacked(InputAction.CallbackContext context)
    {
        controlEvent.HasAttacked = context.action.IsPressed();
    }

    public void Interacted(InputAction.CallbackContext context)
    {
        controlEvent.HasInteracted = context.action.WasPressedThisFrame();
    }

    public void Disengaged(InputAction.CallbackContext context)
    {
        controlEvent.HasDisengaged = context.action.WasPressedThisFrame();
    }
}
