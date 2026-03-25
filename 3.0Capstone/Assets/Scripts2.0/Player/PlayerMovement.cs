using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerControls playerControls;
    
    private PlayerInteract interactor;
    private InputControlManager inputControlManager;
    private Animator playerAnimator;
    private RigHealth rigHealth;
    private Engine engine;
    private Turret turret;
    private Turret mountedTurret;
    private AudioSource playerAudioSource;

    [Header("Sprites")]
    [SerializeField] private GameObject heldAmmo;
    [SerializeField] private GameObject heldRepair;

    [Header("Booleans")]
    public bool isHoldingAmmo;
    public bool isHoldingRepair;
    private bool canAttack = true;

    [Header("Custom Variables")]
    [SerializeField] private float movementSpeed = 5.0f;

    [Header("Debug Values")]
    [SerializeField] private bool isMounted = false;
    [SerializeField] private bool isHolding = false;
    public bool canMove = true;
    public bool canInteract = true;
    [SerializeField] float attackCooldown = 1.0f;

    //hammer ref
    private Hammer hammer;


    private int playerIndex; // Which player this is (0 or 1)

    

    private void Start()
    {
        playerControls = GetComponent<PlayerControls>();
        interactor = GetComponentInChildren<PlayerInteract>();
        inputControlManager = InputControlManager.Instance;
        rigHealth = FindFirstObjectByType<RigHealth>();
        engine = FindFirstObjectByType<Engine>();
        playerAudioSource = GetComponent<AudioSource>();

        // Get the index for this player instance
        playerIndex = inputControlManager.GetCurrentPlayerIndex();

        // Ensure playerIndex is within bounds
        if (playerIndex >= inputControlManager.Player.Length)
        {
            playerIndex = inputControlManager.Player.Length - 1;
        }

        //Spawn player animator/layers
        GameObject playerBody = Instantiate(inputControlManager.Player[playerIndex].PlayerObject, this.transform);

        playerAnimator = playerBody.GetComponentInChildren<Animator>();

        //teehee
        hammer = playerBody.GetComponentInChildren<Hammer>();
        hammer.gameObject.SetActive(false);

        // Teleport to spawn point
        Transform spawn = inputControlManager.GetCurrentSpawnPoint();
        StartCoroutine(Teleport(spawn));
        
        // Notify manager that this player has spawned
        inputControlManager.HasSpawned();

        isHoldingAmmo = false;
        isHoldingRepair = false;


    }

    private void Update()
    {
        Vector2 moveDirection = playerControls.controlEvent.MoveDirection;
        if (canMove)
        {
            HandleMovement(moveDirection);
        }

        HandleRotation(moveDirection);
        HandleInput();
    }

    private void HandleMovement(Vector3 direction)
    {
        
        transform.position = (transform.position + (direction * movementSpeed * Time.deltaTime));
        // Only set walk animation to true if actually moving
        //bool isMoving = moveDirection.magnitude > 0.1f;
        if (direction.magnitude > 0)
        {
            playerAnimator.SetBool("MoleWalk", true);
        }
        else
        {
            playerAnimator.SetBool("MoleWalk", false);
        }
    }

    private Quaternion rotateTo = Quaternion.Euler(0, 0, 0);
    private float rotationSpeed = 10f;
    private float lastDirectionX = 0f; // Track the last non-zero direction

    private void HandleRotation(Vector2 direction)
    {
        // Only update target rotation if there's significant horizontal input
        if (Mathf.Abs(direction.x) > 0.1f)
        {
            lastDirectionX = direction.x;
            
            if (direction.x < 0)
            {
                rotateTo = Quaternion.Euler(0, 180, 0);
            }
            else if (direction.x > 0)
            {
                rotateTo = Quaternion.Euler(0, 0, 0);
            }
        }
        
        // Always lerp towards the target rotation for smooth transitions
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotateTo, Time.deltaTime * rotationSpeed);
    }

    private void HandleInput()
    {
        if (canInteract && playerControls.controlEvent.HasInteracted)
        {
            if (interactor.canMount)
            {
                HandleMounting();
            }
            else if (interactor.canPickup)
            {
                HandlePickup();
            }
            else if(isHolding)
            {
                HandleDrop();
            }

            if (interactor.canRepair)
            {
                HandleRepair();
            }

        }
        else if(!canInteract && playerControls.controlEvent.HasInteracted)
        {
            HandleDismounting();
        }
        else if (playerControls.controlEvent.HasAttacked && canAttack && !isMounted)
        {
            StartCoroutine(HammerSwing());
        }
    }

    private void HandleRepair()
    {
        if (interactor.currentInteractObject.CompareTag("Repair"))
        {
            interactor.currentInteractObject.GetComponent<RepairStation>().PlayPickupSound();
            heldRepair.SetActive(true);
            isHoldingRepair = true;
        }

        if (interactor.currentInteractObject.CompareTag("Damaged") && isHoldingRepair)
        {
            RepairPatch repair = interactor.currentInteractObject.GetComponentInChildren<RepairPatch>();
            
            // Null check before accessing repair
            if (repair != null && !repair.isPatched)
            {
                repair.ActivatePatch();
                //interactor.currentInteractObject.GetComponent<SpriteRenderer>().enabled = false;
                interactor.canRepair = false;
                isHoldingRepair = false;
                heldRepair.SetActive(false);
                rigHealth.HealDamage();
            }
        }
    }

    private void HandleMounting()
    {
        /**
        if (isHolding) HandleDrop();

        canInteract = false; //TESTING
        isMounted = true;
        canMove = false;
        interactor.currentInteractObject.GetComponent<Turret>().Mount(this.gameObject);
        playerAnimator.SetBool("MoleWalk", false);
        **/
        if (isHolding) HandleDrop();

        mountedTurret = interactor.currentInteractObject.GetComponent<Turret>();
        if (mountedTurret == null)
        {
            Debug.LogWarning("Tried to mount, but no turret was found.");
            return;
        }

        canInteract = false;
        isMounted = true;
        canMove = false;

        mountedTurret.Mount(this.gameObject);
        playerAnimator.SetBool("MoleWalk", false);

        Debug.Log($"{name} mounted turret: {mountedTurret?.name}");
    }

    private void HandlePickup()
    {
        if (isHolding) HandleDrop();

        isHolding = true;
        interactor.currentInteractObject.GetComponent<AmmoBox>().SpawnAmmo();

        if (interactor.currentInteractObject.TryGetComponent<AmmoBox>(out var ammo))
        {
            heldAmmo.SetActive(true);
            isHoldingAmmo = true;
        }
    }

    private void HandleDismounting()
    {
        if (!isMounted) return;

        if (mountedTurret != null)
        {
            mountedTurret.Dismount();
            mountedTurret = null;
        }
        else
        {
            Debug.LogWarning("Player is marked mounted, but mountedTurret is null.");
        }

        StartCoroutine(MoveAgain());
        Debug.Log($"{name} dismounting turret: {mountedTurret?.name}");
    }

    private IEnumerator MoveAgain()
    {
        yield return new WaitForSeconds(0.25f);
        canInteract = true;
        isMounted = false;
        canMove = true;
    }

    public void HandleDrop()
    {
        if (!isHolding) return;
        
        isHolding = false;
        isHoldingAmmo = false;
        isHoldingRepair = false;
        heldAmmo.SetActive(false);
        heldRepair.SetActive(false);
    }

    private IEnumerator Teleport(Transform location)
    {
        
        this.transform.position = location.position;
        yield return null;
    }

    IEnumerator HammerSwing()
    {
        canAttack = false;
        //Debug.Log("Tried to swing the hammer...");
        playerAnimator.SetTrigger("HammerSwing");
        if(hammer.isPoweredUp)
        {
            hammer.CallStupidPulse();
        }
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}