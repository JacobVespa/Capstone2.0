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

    private Vector2 rigMoveInput;

    private void Awake()
    {
        var map = inputActions.FindActionMap("Rig", throwIfNotFound: true);
        rigMoveAction = map.FindAction("Move", throwIfNotFound: true);

       //if (!rigBody)
       //{
       //    rigBody = GetComponent<Rigidbody>();
       //}
    }

    private void OnEnable()
    {
        rigMoveAction.Enable();
    }
    private void OnDisable()
    {
        rigMoveAction.Disable();
    }

    private void FixedUpdate()
    {
        rigMoveInput = rigMoveAction.ReadValue<Vector2>();

        RigDrive();
        RigTurn();
    }

    public void RigDrive()
    {
       float moveInput = rigMoveInput.y; //This is/was negative because the tranform on the Ship exterior prefab in the Rig Test scene is backwards

       Vector3 moveForce = rigBody.position + rig.transform.forward * moveInput * rigMoveSpeed * Time.fixedDeltaTime;

       rigBody.MovePosition(moveForce);
    }

    public void RigTurn()
    {
        float turnInput = rigMoveInput.x;

        Quaternion turnForce = Quaternion.Euler(0f, turnInput * rigTurnSpeed * Time.fixedDeltaTime, 0f);

        rigBody.MoveRotation(rigBody.rotation * turnForce);
    }

}
