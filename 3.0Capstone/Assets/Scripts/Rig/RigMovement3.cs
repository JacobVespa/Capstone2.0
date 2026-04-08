using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class RigMovement3 : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rigBody;
    public GameObject rig;

    [Header("Movement Settings")]
    public float rigMoveSpeed = 15.0f;
    public float rigTurnSpeed = 60.0f; // degrees per second

    //[Header("Input")]
    //[SerializeField] InputActionAsset inputActions;

    private PlayerInput input;

    private void FixedUpdate()
    {
        RigDrive();
        RigTurn();
    }

    public void RigDrive()
    {
        Vector3 move = rig.transform.forward * rigMoveSpeed * Time.fixedDeltaTime;
        rigBody.MovePosition(rigBody.position + move);
    }

    public void RigTurn()
    {
        float turnInput = input.actions["Move"].ReadValue<Vector2>().x;

        Quaternion deltaRot = Quaternion.Euler(0f,
            turnInput * rigTurnSpeed * Time.fixedDeltaTime,
            0f);

        rigBody.MoveRotation(rigBody.rotation * deltaRot);
    }

    public void SetPlayerInput(PlayerInput playerInput)
    {
        input = playerInput;
    }

    public void ClearPlayerInput()
    {
        input = null;
    }
}
