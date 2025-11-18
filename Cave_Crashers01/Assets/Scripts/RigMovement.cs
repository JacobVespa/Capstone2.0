using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class RigMovement : MonoBehaviour
{
    public GameObject rig;
    public Rigidbody rigBody;
    public float rigSpeed = 50.0f;

    [SerializeField] InputActionAsset inputActions;
    private InputAction rigMoveAction;
    private Vector2 rigMoveInput;
    private void Awake()
    {
        //player = GameObject.FindGameObjectWithTag("Player");
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

    public void DriveRig()
    {
        Vector3 input = new Vector3(rigMoveInput.x, 0f, rigMoveInput.y);
        Vector3 planar = transform.TransformDirection(input) * rigSpeed;

        Vector3 motion = new Vector3(planar.x, -1f, planar.z);
        rigBody.MovePosition(transform.position + motion * Time.deltaTime * rigSpeed);
    }

}
