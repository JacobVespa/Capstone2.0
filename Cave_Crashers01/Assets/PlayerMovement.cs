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

    [SerializeField] private float movementSpeed = 5.0f;
    [SerializeField] private Vector2 movementDirection;

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        player.Move((playerControls.controlEvent.MoveDirection * movementSpeed) * Time.fixedDeltaTime);
    }
}
