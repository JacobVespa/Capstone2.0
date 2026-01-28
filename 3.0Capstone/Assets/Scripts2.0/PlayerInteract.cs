using System.Collections;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public bool canMount = false;
    public bool canPickup = false;
    public bool canRepair = false;
    public bool canEngine = false;

    [SerializeField] private float interactRange = 1.2f;
    private SphereCollider interactCollider;

    public GameObject currentInteractObject;

    public GameObject wallSection;

    private PlayerMovement playerMove;
    private Turret turret;
    private RepairStation repairBox;
    private AmmoBox ammoBox;
    private Engine engine;

    //temp
    private bool canSpawnWall = true;

    /*
     * Method that grabs a collider from the player that determines collisions
     * 
     * Param( interactCollider ) --> grabs collision from player
     * 
     * Param( interactRange ) --> determines the reach of the collision detection
     * 
     */

    private void Start()
    {
        interactCollider = GetComponent<SphereCollider>();
        interactCollider.radius = interactRange;
        playerMove = GetComponentInParent<PlayerMovement>();
    }


    /*
     * Method that determines what collider the player is currently in contact with
     * 
     * Param( canMount ) --> allows a player to mount a turret when pressing the interact button (When in the correct collider)
     * 
     * Param( canPickup ) --> allows a player to pickup an object when pressing the interact button (When in the correct collider)
     * 
     * Param( canRepair ) --> enables the player to repair a damaged area (When in the correct collider)
     * 
     * Object( currentInteractObject ) --> tracks which object collider the player is currently inside
     */

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Turret"))
        {
            turret = other.GetComponent<Turret>();
            turret.buttonPromptXB.SetActive(true);
            canMount = true;
            currentInteractObject = other.gameObject;
            //testing reload
            if(playerMove.isHoldingAmmo)
            {
                turret.RefillAmmo();
                playerMove.HandleDrop();
            }
        }
        else if (other.CompareTag("Ammo"))
        {
            ammoBox = other.GetComponent<AmmoBox>();
            ammoBox.buttonPromptXB.SetActive(true);
            canPickup = true;
            currentInteractObject = other.gameObject;
        }
        else if (other.CompareTag("Repair"))
        {
            repairBox = other.GetComponent<RepairStation>();
            repairBox.buttonPromptXB.SetActive(true);
            canRepair = true;
            currentInteractObject = other.gameObject;
        }
        else if (other.CompareTag("Damaged"))
        {
            currentInteractObject = other.gameObject;
        }
        else if (other.CompareTag("Engine"))
        {
            engine = other.GetComponent<Engine>();
            engine.buttonPromptXB.SetActive(true);
            canEngine = true;
            currentInteractObject = other.gameObject;
        }
    }

    /*
     * Method that determines when a player exits a collider, setting proper parameters to flase
     * 
     * Param( canMount ) --> allows a player to dismount a turret when pressing the disengage button
     * 
     * Param( canPickup ) --> allows a player to drop an object when pressing the disengage button
     * 
     */

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Turret"))
        {
            turret = other.GetComponent<Turret>();
            turret.buttonPromptXB.SetActive(false);
            canMount = false;
            currentInteractObject = other.gameObject;
        }
        else if (other.CompareTag("Ammo"))
        {
            ammoBox = other.GetComponent<AmmoBox>();
            ammoBox.buttonPromptXB.SetActive(false);
            canPickup = false;
            currentInteractObject = other.gameObject;
        }
        else if(other.CompareTag("Repair"))
        {
            repairBox = other.GetComponent<RepairStation>();
            repairBox.buttonPromptXB.SetActive(false);
        }
        else if(other.CompareTag("Engine"))
        {
            engine = other.GetComponent<Engine>();
            engine.buttonPromptXB.SetActive(false);
        }
    }


}
