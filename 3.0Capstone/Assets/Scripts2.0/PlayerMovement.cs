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

    private void HandleMovement()
    {
        player.Move((playerControls.controlEvent.MoveDirection * movementSpeed) * Time.fixedDeltaTime);
    }

    private void HandleInput()
    {
        if (playerControls.controlEvent.HasInteracted)
        {
            if (interactor.canMount) HandleMounting();
            else if (interactor.canPickup) HandlePickup();
        }
        else if (playerControls.controlEvent.HasDisengaged)
        {
            HandleDismounting();
            HandleDrop();
        }
    }

    private void HandleMounting()
    {
        if (isHolding) HandleDrop();

        isMounted = true;
        interactor.currentInteractObject.GetComponent<Turret>().Mount(this.gameObject);
    }

    private void HandlePickup()
    {
        if (isHolding) HandleDrop();

        isHolding = true;
        interactor.currentInteractObject.GetComponent<AmmoBox>().SpawnAmmo();
    }

    private void HandleDismounting()
    {
        isMounted = false;
        interactor.currentInteractObject.GetComponent<Turret>().Dismount();
    }

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
