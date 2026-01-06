using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CharacterController))]
public class OldPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float gravity = 9.8f;

    [Header("Look Sensitivity")]
    [SerializeField] private float lookSensitivity = 2.0f;
    [SerializeField] private float verticalRange = 80.0f;

    [Header("Player Camera")]
    [SerializeField] private Camera mainCamera;

    [Header("Input Actions")]
    [SerializeField] private InputActionAsset playerControls;

    private bool isMoving;
    private PlayerInput playerInput;
    private CharacterController characterController;

    private InputAction moveAction;
    private InputAction lookAction;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private Vector3 velocity;
    private float verticalRotation;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        var map = playerInput.actions.FindActionMap("Player", throwIfNotFound: true);
        moveAction = map.FindAction("Move", throwIfNotFound: true);
        lookAction = map.FindAction("Look", throwIfNotFound: true);

        if (mainCamera == null)
            mainCamera = GetComponentInChildren<Camera>();
    }

    private void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
    }

    private void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        lookInput = lookAction.ReadValue<Vector2>();

        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        // Convert input to world space relative to facing
        Vector3 input = new Vector3(moveInput.x, 0f, moveInput.y);
        Vector3 planar = transform.TransformDirection(input) * walkSpeed;

        // Grounding & gravity
        if (characterController.isGrounded)
            velocity.y = -1f; // keep grounded
        else
            velocity.y -= gravity * Time.deltaTime;

        Vector3 motion = new Vector3(planar.x, velocity.y, planar.z);
        characterController.Move(motion * Time.deltaTime);
    }

    private void HandleRotation()
    {
        float yaw = lookInput.x * lookSensitivity;
        transform.Rotate(0, yaw, 0);

        verticalRotation -= lookInput.y * lookSensitivity * Time.deltaTime;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalRange, verticalRange);
        if(mainCamera != null)
        {
            mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
        }

    }
}