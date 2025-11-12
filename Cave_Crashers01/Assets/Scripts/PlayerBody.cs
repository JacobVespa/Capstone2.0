using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    [Header("Player Controller")]
    [SerializeField] private PlayerController controller;

    [Header("Player State")]
    [SerializeField] private PlayerState state;
    public enum PlayerState
    {
        Free = 0,
        Station = 1,
    }

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3;
    [SerializeField] private float gravity = 9.8f;

    [Header("View Sensitivity")]
    [SerializeField] private float lookSensitivity = 2.0f;
    [SerializeField] private float verticalRange = 80.0f;

}
