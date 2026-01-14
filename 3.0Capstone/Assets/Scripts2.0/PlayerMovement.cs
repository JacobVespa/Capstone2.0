using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerControls playerControls;
    private CharacterController player;
    private PlayerInteract interactor;
    private InputControlManager inputControlManager;

    [Header("Sprites")]
    [SerializeField] private GameObject heldAmmo; //sprite for player holding ammo
    [SerializeField] private GameObject heldRepair; //sprite for player holding biotape
    private Sprite playerSprite;
    private Animator playerAnimator;
    private GameObject damagedArea;

    [Header("Booleans")]
    public bool isHoldingAmmo;
    private bool isHoldingRepair;

    private void Start()
    {
        playerControls = GetComponent<PlayerControls>();
        player = GetComponent<CharacterController>();
        interactor = GetComponentInChildren<PlayerInteract>();
        inputControlManager = InputControlManager.Instance;

        Transform spawn = inputControlManager.SpawnPoints[0];
        StartCoroutine(Teleport(spawn));
        inputControlManager.HasSpawned();

        isHoldingAmmo = false;
        isHoldingRepair = false;
    }


    [Header("Custom Varaibles")]
    [SerializeField] private float movementSpeed = 5.0f;

    [Header("Debug Values")]
    [SerializeField] private bool isMounted = false;
    [SerializeField] private bool isHolding = false;
    [SerializeField] private bool canMove = true; //testing for now

    private void Update()
    {
        if(canMove)
        {
            HandleMovement();
        }
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
            else if (interactor.canRepair) HandleRepair();
        }
        else if (playerControls.controlEvent.HasDisengaged)
        {
            HandleDismounting(); // Dismounts player
            HandleDrop(); // Drops pickup
        }
    }

    //There might be a better way to do this
    private void HandleRepair()
    {
        if (interactor.currentInteractObject.CompareTag("Repair"))
        {
            heldRepair.SetActive(true);
            isHoldingRepair = true;
        }

        if (isHoldingRepair && interactor.currentInteractObject.CompareTag("Damaged"))
        {
            interactor.currentInteractObject.SetActive(false);
            interactor.canRepair = false;
            isHoldingRepair = false;
            heldRepair.SetActive(false);
        }

    }

    // Calls turret script and provides player gameobject to allow only one player to mount and control turret
    private void HandleMounting()
    {
        if (isHolding) HandleDrop();

        isMounted = true;
        canMove = false; //testing for now
        interactor.currentInteractObject.GetComponent<Turret>().Mount(this.gameObject);
    }

    // Calls pickup script and provides...
    private void HandlePickup()
    {
        if (isHolding) HandleDrop();

        isHolding = true;
        interactor.currentInteractObject.GetComponent<AmmoBox>().SpawnAmmo();

        //this is temp
        if (interactor.currentInteractObject.TryGetComponent<AmmoBox>(out var ammo))
        {
            heldAmmo.SetActive(true);
            isHoldingAmmo = true;
        }
        
    }

    // Removes player referene from turret and resets script
    private void HandleDismounting()
    {
        isMounted = false;
        canMove = true; //testing for now
        interactor.currentInteractObject.GetComponent<Turret>().Dismount();
    }

    // Drops what the player is holding
    public void HandleDrop() //can probably switch back to private later, check reloading in player interact
    {
        isHolding = false;

        //this is temporary
        isHoldingAmmo = false;
        isHoldingRepair = false;
        heldAmmo.SetActive(false);
        heldRepair.SetActive(false);
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
