using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerBody))]

public class NewPlayerController : MonoBehaviour
{

    [Header("Player Body")]
    [SerializeField] private PlayerBody body;

    [Header("Player Camera")]
    [SerializeField] private Camera mainCamera;



    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = GetComponentInChildren<Camera>();
    }

    private void Update()
    {

    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        Vector2 dir = context.action.ReadValue<Vector2>();
        body.InputDir = new Vector3(dir.x, 0, dir.y);
    }

    public void OnView(InputAction.CallbackContext context)
    {
        Vector2 view = context.action.ReadValue<Vector2>();
        body.ViewDir = new Vector3(view.x, view.y, 0);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            body.Interact();
        }
    }

    public void OnDisengage(InputAction.CallbackContext context)
    {

    }



}