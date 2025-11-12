using UnityEngine;
using UnityEngine.InputSystem;

public class Station : MonoBehaviour, IInteractable
{
    [SerializeField] bool inUse;
    private PlayerController currentUser;
    private Transform originalParent;
    //[SerializeField] InputActionAsset inputActions;

    //pilot seat references
    public GameObject pilotSeat;
    public GameObject stationSeat;

    //player
    public GameObject player;

    public CharacterController characterController; 

    public Transform seatTransform;
    public Transform exitTransform;

    //player camera
    public GameObject playerCamera;

    //drill references
    public GameObject drill;
    private DrillController drillController;
    public GameObject drillButton;

    private void Awake()
    {
        if (stationSeat == null)
        {
            stationSeat = GameObject.FindGameObjectWithTag("PilotSeat");
            seatTransform = stationSeat.transform.Find("SeatTransform");
            exitTransform = stationSeat.transform.Find("ExitTransform");
        }

        inUse = false;
    }

    private void Start()
    {
        drillController = drill.GetComponent<DrillController>();
    }

    public void Interact(PlayerController interactor)
    {
        //handling pilot seat interactions
        Debug.Log("lol");
        if (!inUse && gameObject == drillButton)
        {
            //UNCOMMENT THIS AFTERWARDS, NEED TO FIND A WAY SO THAT YOU CAN DISTINGUISH BETWEEN STATIONS
            //SeatPlayer(interactor);

            //disable the player camera and go into mining mode
            playerCamera.SetActive(false);
            drillController.EnableDrill();
        }
        if(!inUse && gameObject == pilotSeat)
        {
            SeatPlayer(interactor);
        }
        else if (inUse && interactor == currentUser)
        {
            ExitStation(interactor);
        }
        else
        {
            Debug.Log("Station is occupied");
        }
        
    }

    public void SeatPlayer(PlayerController pc)
    {
        if (inUse) { return; }
        //inUse = true;

        characterController = pc.GetComponent<CharacterController>();

        FreezeMovement(pc);

        //Save Parent for later
        originalParent = pc.transform.parent;

        //Parent to the seat and zero local pose
        pc.transform.SetParent(seatTransform, worldPositionStays: false);
        pc.transform.localPosition = Vector3.zero;
        pc.transform.localRotation = Quaternion.identity;
        pc.transform.localScale = Vector3.one;

        currentUser = pc;
        inUse = true;
    }

    public void ExitStation(PlayerController pc)
    {
        if (!inUse || pc != currentUser) { return; }

        characterController = pc.GetComponent<CharacterController>();

        //Unparent and place at exit
        pc.transform.SetParent(originalParent, worldPositionStays: true);
        if (exitTransform)
        {
            pc.transform.SetPositionAndRotation(exitTransform.position, exitTransform.rotation);
        }

        //Re-enable movement and colliders
        if (pc) pc.enabled = true;
        if (characterController) characterController.enabled = true;

        currentUser = null;
        inUse = false;
    }

    private void FreezeMovement(PlayerController pc)
    {
        //Disable moveement while at the station
        if (pc) pc.enabled = false;
        if (characterController) characterController.enabled = false;
    }

}
