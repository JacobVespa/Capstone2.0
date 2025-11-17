using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class RigMovement : MonoBehaviour
{
    public GameObject player;
    public GameObject pilotSeat;
    public GameObject rig;
    public Rigidbody rigBody;
    public float rigSpeed = 5.0f;

    [SerializeField] InputActionAsset inputActions;
    private InputAction rigMoveAction;
    private Vector2 rigMoveInput;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rig = GameObject.FindGameObjectWithTag("Rig");
        rigBody = rig.GetComponent<Rigidbody>();
        var map = inputActions.FindActionMap("Rig", throwIfNotFound: true);
        rigMoveAction = map.FindAction("Move", throwIfNotFound: true);
    }

    private void OnEnable()
    {
        rigMoveAction.Enable();
    }

    private void OnDisable()
    {
        rigMoveAction.Disable();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rigMoveInput = rigMoveAction.ReadValue<Vector2>();

        DriveRig();
    }

  // public void Interact()
  // {
  //     //Set Players Transform to Pilot Seat Transform
  //     player.transform.position = pilotSeat.transform.position; 
  //     //Debug.Log("Here");
  //
  //     //UI Transition?
  //
  //     //Movement Controls
  //     //Get rigidbody of the ship 
  //     //AddForce to the Rig as neccesary
  //
  //     //trying to figure out how to use the new input system with this
  //
  //
  //     
  //
  // }

    public void DriveRig()
    {
        Vector3 input = new Vector3(rigMoveInput.x, 0f, rigMoveInput.y);
        Vector3 planar = transform.TransformDirection(input) * rigSpeed;
    }

}
