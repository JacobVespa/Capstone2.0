using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(CharacterController))]
public class PlayerBody : MonoBehaviour
{
    [Header("Player Aspects")]
    [SerializeField] private PlayerController inputs;
    public PlayerController Inputs { get { return inputs; } }

    [SerializeField] private CharacterController bodyController;
    public CharacterController BodyController { get { return bodyController; } }

    [SerializeField] private Camera camera;
    public Camera Camera { get { return camera; } }

    [SerializeField] private PlayerUIManager UImanager;
    public PlayerUIManager UIManager { get { return UImanager; } }

    [Header("Player State")]
    [SerializeField] private PlayerState state;
    public PlayerState State { get { return state; } }


    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3;
    [SerializeField] private float gravity = 9.8f;
    private Vector3 inputDir = Vector3.zero;
    public Vector3 InputDir { get { return inputDir; } set { inputDir = value; } }

    private Vector3 motion = Vector3.zero;

    [Header("RIG reference")]
    [SerializeField] private Rigidbody RIGRigidbody;
    private Quaternion lastRIGRotation;
    private Vector3 lastRIGPosition;

    [Header("View Sensitivity")]
    [SerializeField] private float lookSensitivity = 2.0f;
    [SerializeField] private float verticalRange = 80.0f;
    [SerializeField] private float horizontalRange = 80.0f;
    private Vector3 viewDir = Vector3.zero;
    public Vector3 ViewDir { get { return viewDir; } set { viewDir = value; } }

    [Header("Interaction Settings")]
    public Transform InteractorSource;
    public float InteractRange;


    public enum PlayerState
    {
        Free = 0,
        Station = 1,
    }

    void Start()
    {
        if (RIGRigidbody == null)
            RIGRigidbody = GetComponentInParent<Rigidbody>();

        if (RIGRigidbody != null)
        {
            lastRIGRotation = RIGRigidbody.rotation;
            lastRIGPosition = RIGRigidbody.position;
        }
    }

    private void Update()
    {
        HandleRotation();
    }


    private void FixedUpdate()
    {
        ApplyShipMotion();
        UpdateMovement();
    }

    #region Movement
    private void UpdateMovement()
    {

        HandleMoveInputs();
        HandleGravity();


        
        if(state == PlayerState.Free) { bodyController.Move(motion * Time.fixedDeltaTime); }
        
        
    }

    private void HandleMoveInputs()
    {
        if (inputDir == Vector3.zero) { motion = Vector3.zero; return; }

        
        motion = transform.TransformDirection(InputDir) * moveSpeed;
    }

    private void HandleGravity()
    {
        if (bodyController.isGrounded)
        {
            motion.y = -1;
        }
        else if (!bodyController.isGrounded)
        {
            motion.y -= gravity;
        }
    }

    public void EnableGravity(bool isEnabled)
    {
        if (isEnabled)
        {
            gravity = 9.8f;
        }
        else
        {
            gravity = 0;
        }
    }

    private void HandleRotation()
    {

        Vector3 rotate = new Vector3(0, viewDir.x, 0);
        rotate *= lookSensitivity;
        transform.Rotate(rotate);

    }

    private void ApplyShipMotion()
    {
        if (!RIGRigidbody) return;

        if (state == PlayerState.Free)
        {
            Vector3 currentPos = RIGRigidbody.position;
            Quaternion currentRot = RIGRigidbody.rotation;

            Vector3 rigDeltaPos = currentPos - lastRIGPosition;
            Quaternion rigDeltaRot = currentRot * Quaternion.Inverse(lastRIGRotation);

            bodyController.Move(rigDeltaPos);

            transform.rotation = rigDeltaRot * transform.rotation;

            Vector3 offset = transform.position - RIGRigidbody.position;
            Vector3 rotatedOffset = rigDeltaRot * offset;
            Vector3 rotationDiff = rotatedOffset - offset;

            bodyController.Move(rotationDiff);

            motion = rigDeltaRot * motion;

            lastRIGPosition = currentPos;
            lastRIGRotation = currentRot;
        }
    }
    #endregion




    public void Interact()
    {
        Ray r = new Ray(InteractorSource.position, InteractorSource.forward);
        if (Physics.Raycast(r, out RaycastHit hitInfo, InteractRange))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                interactObj.Interact(this) ;
            }
        }
    }

    public void DisEngage()
    {
        Station station = GetComponentInParent<Station>();
        if(station != null)
        {
            UImanager.SetToFree();
            station.ExitStation(this);
        }
        else
        {
            Debug.LogError("STATION NOT IN PARENTS OF OBJECT");
        }
    }

    public void EnterStation()
    {
        state = PlayerState.Station;
        inputDir = Vector3.zero;
        ViewDir = Vector3.zero;
    }

    public void ExitStation()
    {
        state = PlayerState.Free;
    }
}