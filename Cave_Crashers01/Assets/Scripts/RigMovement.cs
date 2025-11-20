using UnityEngine;
using UnityEngine.InputSystem;

public class RigMovement : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rigBody;
    public GameObject rig;

    [Header("Movement Settings")]
    public float rigMoveSpeed = 20.0f;
    public float rigTurnSpeed = 60.0f; // degrees per second

    [Header("Input")]
    [SerializeField] InputActionAsset inputActions;

    private InputAction rigMoveAction;
    private Vector2 RIGMoveInput;

    private void Awake()
    {
        var map = inputActions.FindActionMap("Rig", throwIfNotFound: true);
        rigMoveAction = map.FindAction("Move", throwIfNotFound: true);
    }

    private void OnEnable() => rigMoveAction.Enable();
    private void OnDisable() => rigMoveAction.Disable();

    private void FixedUpdate()
    {
        RIGMoveInput = rigMoveAction.ReadValue<Vector2>();

        RigDrive();
        RigTurn();
    }

    public void RigDrive()
    {
        float moveInput = RIGMoveInput.y;

        Vector3 move = rig.transform.forward * moveInput * rigMoveSpeed * Time.fixedDeltaTime;
        rigBody.MovePosition(rigBody.position + move);
    }

    public void RigTurn()
    {
        float turnInput = RIGMoveInput.x;

        Quaternion deltaRot = Quaternion.Euler(0f,
            turnInput * rigTurnSpeed * Time.fixedDeltaTime,
            0f);

        rigBody.MoveRotation(rigBody.rotation * deltaRot);
    }




}
