using UnityEngine;
using System.Collections;

public class Station : MonoBehaviour, IInteractable
{
    [SerializeField] bool inUse;
    private PlayerController currentUser;
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
    }

    private void Start()
    {
        drillController = drill.GetComponent<DrillController>();
    }

    public void Interact(PlayerController interactor)
    {
        if (!canToggle) return;      // block extra calls during cooldown
        StartCoroutine(ToggleWithCooldown(interactor));
    }

    private IEnumerator ToggleWithCooldown(PlayerController interactor)
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
                playerCamera.SetActive(false);
                drillController.EnableDrill();
            }
        }
        else if (interactor == currentUser) // <---- ALWAYS FALSE
        {
            Debug.Log("Exiting station");
            ExitStation(interactor);
            rigMovement.enabled = false;
        }
        else
        {
            Debug.Log("Station is occupied");
        }

        yield return new WaitForSeconds(toggleCooldown);
        canToggle = true;
    }

    public void SeatPlayer(PlayerController pc)
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

    public void ExitStation(PlayerController pc)
    {
        if (!inUse || pc != currentUser) return;

        characterController = pc.GetComponent<CharacterController>();

        // Unparent back to original parent
        pc.transform.SetParent(originalParent);

        // Move to exit point
        if (exitTransform)
        {
            pc.transform.SetPositionAndRotation(exitTransform.position, exitTransform.rotation);
        }

        UnFreezeMovement(pc);

        currentUser = null;
        inUse = false;
    }

    private void FreezeMovement(PlayerController pc)
    {
        //Disable moveement while at the station
        if (pc) pc.enabled = false;
        if (characterController) characterController.enabled = false;
    }

    private void UnFreezeMovement(PlayerController pc)
    {
        //Enable moveement while at the station
        if (pc) pc.enabled = true;
        if (characterController) characterController.enabled = true;
    }
}
