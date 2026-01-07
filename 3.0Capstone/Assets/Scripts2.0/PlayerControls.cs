using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    // Stucture that stores all boolean and vector values for public access
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

    // Method that stores left stick and or WASD input as a Vector2
    public void MoveValue(InputAction.CallbackContext context)
    {
        Vector2 placeHolder = context.action.ReadValue<Vector2>();
        controlEvent.MoveDirection = new Vector2(placeHolder.x, placeHolder.y);
    }

    // Method that stores right stick or mouse input as a Vector2
    public void LookValue(InputAction.CallbackContext context)
    {
        Vector2 placeHolder = context.action.ReadValue<Vector2>();
        controlEvent.LookDirection = new Vector2(placeHolder.x, placeHolder.y);
    }

    // Method that tracks when attack button is pressed
    public void Attacked(InputAction.CallbackContext context)
    {
        controlEvent.HasAttacked = context.action.IsPressed();
    }

    // Method that tracks when the interact button is pressed
    public void Interacted(InputAction.CallbackContext context)
    {
        controlEvent.HasInteracted = context.action.WasReleasedThisFrame();
    }

    // Method that tracks when the disengage button is pressed
    public void Disengaged(InputAction.CallbackContext context)
    {
        controlEvent.HasDisengaged = context.action.WasReleasedThisFrame();
    }
}
