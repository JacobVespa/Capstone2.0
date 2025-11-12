using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class InteractorScript : MonoBehaviour
{
    [Header("Interaction Settings")]
    public Transform InteractorSource;
    public float InteractRange;
    public PlayerController playerController;

    [Header("Input Actions")]
    [SerializeField] private InputActionAsset inputActions;

    private void Awake()
    {
        playerController = this.GetComponent<PlayerController>();
    }
    /*
    private InputAction interactAction;

    private void OnEnable()
    {
        inputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        inputActions.FindActionMap("Player").Disable();
    }

    private void Awake()
    {
        interactAction = InputSystem.actions.FindAction("Interact");
    }

    private void Update()
    {
        if (interactAction.WasPressedThisFrame())
        {
            OnInteract();
        }
    }
    */
    public void OnInteract()
    {
        Ray r = new Ray(InteractorSource.position, InteractorSource.forward);
        if (Physics.Raycast(r, out RaycastHit hitInfo, InteractRange))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                interactObj.Interact(playerController);
            }
        }
    }
}
