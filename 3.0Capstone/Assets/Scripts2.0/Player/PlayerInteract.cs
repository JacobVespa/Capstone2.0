using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public bool canMount = false;
    public bool canPickup = false;
    public bool canRepair = false;
    public bool canEngine = false;

    [SerializeField] private float interactRange = 1.2f;
    private CircleCollider2D interactCollider;

    public GameObject currentInteractObject;
    private List<GameObject> repairTargets = new List<GameObject>();
    public GameObject currentRepairTarget => GetClosestRepairTarget();

    public GameObject wallSection;

    private PlayerMovement playerMove;
    private Turret turret;
    private RepairStation repairBox;
    private AmmoBox ammoBox;
    private Engine engine;

    private void Start()
    {
        interactCollider = GetComponent<CircleCollider2D>();
        interactCollider.radius = interactRange;
        playerMove = GetComponentInParent<PlayerMovement>();
    }

    private GameObject GetClosestRepairTarget()
    {
        repairTargets.RemoveAll(t => t == null); // clean up destroyed objects
        if (repairTargets.Count == 0) return null;

        GameObject closest = null;
        float closestDist = Mathf.Infinity;

        foreach (GameObject target in repairTargets)
        {
            float dist = Vector2.Distance(transform.position, target.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = target;
            }
        }

        return closest;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Turret"))
        {
            turret = other.GetComponent<Turret>();
            if (turret.playerMounted) { return; }
            turret.buttonPromptXB.SetActive(true);
            canMount = true;
            currentInteractObject = other.gameObject;
        }
        else if (other.CompareTag("Ammo"))
        {
            ammoBox = other.GetComponent<AmmoBox>();
            ammoBox.buttonPromptXB.SetActive(true);
            canPickup = true;
            currentInteractObject = other.gameObject;
        }
        else if (other.CompareTag("Repair") && canMount != true)
        {
            repairBox = other.GetComponent<RepairStation>();
            repairBox.buttonPromptXB.SetActive(true);
            canRepair = true;
            currentInteractObject = other.gameObject;
        }
        else if (other.CompareTag("Damaged"))
        {
            repairTargets.Add(other.gameObject);
        }
        else if (other.CompareTag("Engine"))
        {
            engine = other.GetComponent<Engine>();
            engine.buttonPromptXB.SetActive(true);
            canEngine = true;
            currentInteractObject = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
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
        else if (other.CompareTag("Repair"))
        {
            repairBox = other.GetComponent<RepairStation>();
            repairBox.buttonPromptXB.SetActive(false);
            canRepair = false;
        }
        else if (other.CompareTag("Damaged"))
        {
            repairTargets.Remove(other.gameObject);
        }
        else if (other.CompareTag("Engine"))
        {
            engine = other.GetComponent<Engine>();
            engine.buttonPromptXB.SetActive(false);
            canEngine = false;
            currentInteractObject = other.gameObject;
        }
    }
}