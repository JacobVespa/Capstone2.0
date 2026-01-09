using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public bool canMount = false;
    public bool canPickup = false;
    public bool canRepair = false;

    [SerializeField] private float interactRange = 1.2f;
    private SphereCollider interactCollider;

    public GameObject currentInteractObject;

    public GameObject wallSection;

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
            canMount = true;
            currentInteractObject = other.gameObject;
        }
        else if (other.CompareTag("Ammo"))
        {
            canPickup = true;
            currentInteractObject = other.gameObject;
        }
        else if (other.CompareTag("Repair"))
        {
            canRepair = true;
            currentInteractObject = other.gameObject;
        }
        else if(other.CompareTag("WallTrigger"))
        {
            Instantiate(wallSection, new Vector2(0, 83.75f), Quaternion.identity);
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
            canMount = false;
            currentInteractObject = other.gameObject;
        }
        else if (other.CompareTag("Ammo"))
        {
            canPickup = false;
            currentInteractObject = other.gameObject;
        }  
    }
}
