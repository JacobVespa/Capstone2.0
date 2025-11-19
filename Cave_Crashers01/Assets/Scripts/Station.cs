using UnityEngine;
using System.Collections;


public class Station : MonoBehaviour, IInteractable
{
    [SerializeField] bool inUse;
    private PlayerBody currentUser;
    private Transform originalParent;
    //[SerializeField] InputActionAsset inputActions;

    //pilot seat references
    public GameObject pilotSeat;
    public GameObject stationSeat;
    public RigMovement rigMovement;

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

    // Double press prevention
    private bool canToggle = true;   // cooldown gate
    [SerializeField] private float toggleCooldown = 0.5f;

    private void Awake()
    {
        if (stationSeat == null)
        {
            stationSeat = GameObject.FindGameObjectWithTag("PilotSeat");
            //seatTransform = stationSeat.transform.Find("SeatTransform");
            //exitTransform = stationSeat.transform.Find("ExitTransform");
        }
        rigMovement.enabled = false;
        inUse = false;

        Debug.Log("Turned on " + transform.parent.gameObject);
    }

    private void OnDisable()
    {
        Debug.Log("Turned off" + transform.parent.gameObject);
        //this.enabled = true;
    }

    private void Start()
    {
        drillController = drill.GetComponent<DrillController>();
    }

    public void Interact(PlayerBody interactor)
    {
        
        if (!canToggle) return;      // block extra calls during cooldown
        StartCoroutine(ToggleWithCooldown(interactor));
    }

    public void Select(PlayerBody interactor)
    {
        //NOTHING
    }

    private IEnumerator ToggleWithCooldown(PlayerBody interactor)
    {
         canToggle = false;

        if (!inUse)
        {
            // Enter the correct station type based on tag
            if (CompareTag("PilotSeat"))
            {
                SeatPlayer(interactor);
                rigMovement.enabled = true;
            }
            else if (CompareTag("DrillButton"))
            {
                // Drill station logic
                playerCamera = interactor.GetComponentInChildren<Camera>().gameObject;
                playerCamera.SetActive(false);
                SeatPlayer(interactor);
                drillController.EnableDrill();
            }
        }
        else if (interactor == currentUser) // <---- ALWAYS FALSE
        {
            Debug.Log("Exiting station");
            ExitStation(interactor);
        }
        else
        {
            Debug.Log("Station is occupied");
        }

        yield return new WaitForSeconds(toggleCooldown);
        canToggle = true;
    }

    public void SeatPlayer(PlayerBody pc)
    {
        if (inUse) return;
        

        characterController = pc.GetComponent<CharacterController>();

        FreezeMovement(pc);

        // Save the original parent so we can restore it later
        originalParent = pc.transform.parent;

        // Snap the player exactly to the seat position and rotation
        pc.transform.SetParent(seatTransform);
        pc.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity); // Set to exact transforms later
        pc.transform.localScale = Vector3.one;

        currentUser = pc;
        inUse = true;
    }

    public void ExitStation(PlayerBody pc)
    {
        if (!inUse || pc != currentUser) return;

        characterController = pc.GetComponent<CharacterController>();

        // Unparent back to original parent
        pc.transform.SetParent(originalParent);

        // Move to exit point
        if (exitTransform != null)
        {
            pc.transform.SetPositionAndRotation(exitTransform.position, exitTransform.rotation);
        }

        if(CompareTag("DrillButton"))
        {
            playerCamera.SetActive(true);
            drillController.DisableDrill();
        }

        //reset the scale of the player, this was causing issues
        pc.transform.localScale = Vector3.one;

        UnFreezeMovement(pc);

        currentUser = null;
        inUse = false;
        rigMovement.enabled = false; //This should not be here, but I am running out of ideas
    }

    private void FreezeMovement(PlayerBody pc)
    {
        //Disable moveement while at the station
        Debug.Log("freeze");
        if (pc) pc.Inputs.enabled = false; pc.EnterStation();
        if (characterController) characterController.enabled = false;
    }

    private void UnFreezeMovement(PlayerBody pc)
    {
        //Enable moveement while at the station
        if (pc) pc.Inputs.enabled = true; pc.ExitStation();
        if (characterController) characterController.enabled = true;
    }
}
