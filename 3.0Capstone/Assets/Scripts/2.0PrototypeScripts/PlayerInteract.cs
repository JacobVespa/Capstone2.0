using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public bool canMount = false;
    public bool canPickup = false;

    [SerializeField] private float interactRange = 1.2f;
    private SphereCollider interactCollider;

    private GameObject currentInteractObject;

    private void Start()
    {
        interactCollider = GetComponent<SphereCollider>();
        interactCollider.radius = interactRange;
    }

    private void OnTriggerEnter(Collider other) // Handle teleporting on/off turret
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
    }

    private void OnTriggerExit(Collider other) // Removes ability to mount or pickup
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
