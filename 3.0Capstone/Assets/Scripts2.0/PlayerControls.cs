using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    // Stucture that stores all boolean and vector values for public access
    public struct Controls
    { 
        private Vector2 moveDirection;
        public Vector2 MoveDirection { get { return moveDirection; } set { moveDirection = value; } }

        private Vector2 lookDirection;
        public Vector2 LookDirection { get { return lookDirection; } set { lookDirection = value; } }

        private bool hasAttacked;
        public bool HasAttacked { get { return hasAttacked; } set { hasAttacked = value; } }

        private bool hasInteracted;
        public bool HasInteracted { get { return hasInteracted; } set { hasInteracted = value; } }

        private bool hasDisengaged;
        public bool HasDisengaged { get { return hasDisengaged; } set { hasDisengaged = value; } }

        private bool heldInteracted;
        public bool HeldInteracted { get { return hasDisengaged; } set { hasDisengaged = value; } }

        private bool hasSwungHammer;
        public bool HasSwungHammer { get { return hasSwungHammer; } set { hasSwungHammer = value; } }

        // Method to reset all button states
        public void ResetButtons()
        {
            hasAttacked = false;
            hasInteracted = false;
            hasDisengaged = false;
            heldInteracted = false;
        }

    }
    public Controls controlEvent;

    private void Awake()
    {
        controlEvent = new Controls();
    }

    private void LateUpdate()
    {
        controlEvent.ResetButtons();
    }

    // Method that stores left stick and or WASD input as a Vector2
    public void MoveValue(InputAction.CallbackContext context)
    {
        controlEvent.MoveDirection = context.action.ReadValue<Vector2>();
    }

    // Method that stores right stick or mouse input as a Vector2
    public void LookValue(InputAction.CallbackContext context)
    {
        controlEvent.LookDirection = context.action.ReadValue<Vector2>();
    }

    // Method that tracks when attack button is pressed
    public void Attacked(InputAction.CallbackContext context)
    {
        controlEvent.HasAttacked = context.action.WasPressedThisFrame();
    }

    // Method that tracks when the interact button is pressed
    public void Interacted(InputAction.CallbackContext context)
    {
        controlEvent.HasInteracted = context.action.WasPressedThisFrame();
    }

    //// Method that tracks when the interact button is held
    //public void HeldInteracted(InputAction.CallbackContext context)
    //{
    //    controlEvent.HeldInteracted = context.action.WasHeld(-need to figure out how to do this);
    //}

    // Method that tracks when the disengage button is pressed
    public void Disengaged(InputAction.CallbackContext context)
    {
        controlEvent.HasDisengaged = context.action.WasPressedThisFrame();
    }

    public void SwingHammer(InputAction.CallbackContext context)
    {
        controlEvent.HasSwungHammer = context.action.WasPressedThisFrame();
    }

}
