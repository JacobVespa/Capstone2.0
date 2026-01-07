using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerControls playerControls;
    private CharacterController player;
    private PlayerInteract interactor;

    private void Start()
    {
        playerControls = GetComponent<PlayerControls>();
        player = GetComponent<CharacterController>();
        interactor = GetComponentInChildren<PlayerInteract>();
    }


    [Header("Custom Varaibles")]
    [SerializeField] private float movementSpeed = 5.0f;

    [Header("Debug Values")]
    [SerializeField] private bool isMounted = false;
    [SerializeField] private bool isHolding = false;

    private void Update()
    {
        HandleMovement();
        HandleInput();
    }

    // Method that calculates player movement
    private void HandleMovement()
    {
        player.Move((playerControls.controlEvent.MoveDirection * movementSpeed) * Time.fixedDeltaTime);
    }

    // Method that determines what buttons are pressed and action results
    private void HandleInput()
    {
        if (playerControls.controlEvent.HasInteracted)
        {
            if (interactor.canMount) HandleMounting(); // If the player is able to mount and presses interact, mount
            else if (interactor.canPickup) HandlePickup(); // If the player is able to pickup and presses interact, interact
        }
        else if (playerControls.controlEvent.HasDisengaged)
        {
            HandleDismounting(); // Dismounts player
            HandleDrop(); // Drops pickup
        }
    }

    // Calls turret script and provides player gameobject to allow only one player to mount and control turret
    private void HandleMounting()
    {
        if (isHolding) HandleDrop();

        isMounted = true;
        interactor.currentInteractObject.GetComponent<Turret>().Mount(this.gameObject);
    }

    // Calls pickup script and provides...
    private void HandlePickup()
    {
        if (isHolding) HandleDrop();

        isHolding = true;
    }

    // Removes player referene from turret and resets script
    private void HandleDismounting()
    {
        isMounted = false;
        interactor.currentInteractObject.GetComponent<Turret>().Dismount();
    }

    // Drops what the player is holding
    private void HandleDrop()
    {
        isHolding = false;
    }

    private IEnumerator Teleport(Transform location)
    {
        player.enabled = false;

        this.transform.position = location.position;
        //this.transform.rotation = location.rotation;

        player.enabled = true;

        yield return null;
    }

}
