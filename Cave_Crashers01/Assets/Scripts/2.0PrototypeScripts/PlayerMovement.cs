using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerControls playerControls;
    private CharacterController player;

    private void Start()
    {
        playerControls = GetComponent<PlayerControls>();
        player = GetComponent<CharacterController>();
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
            //Collider Check Here
            HandleMounting();
            HandlePickup();
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
    }

    private void HandleDismounting()
    {
        isMounted = false;
    }

    private void HandlePickup()
    {
        if (isHolding) HandleDrop();

        isHolding = true;
    }

    private void HandleDrop()
    {
        isHolding = false;
    }
}
