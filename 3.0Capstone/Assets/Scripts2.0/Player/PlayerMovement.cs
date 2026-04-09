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

    private Hammer hammer;

    private int playerIndex;

    private void Start()
    {
        playerControls = GetComponent<PlayerControls>();
        interactor = GetComponentInChildren<PlayerInteract>();
        inputControlManager = InputControlManager.Instance;
        rigHealth = FindFirstObjectByType<RigHealth>();
        engine = FindFirstObjectByType<Engine>();
        playerAudioSource = GetComponent<AudioSource>();

        playerIndex = inputControlManager.GetCurrentPlayerIndex();

        if (playerIndex >= inputControlManager.Player.Length)
        {
            playerIndex = inputControlManager.Player.Length - 1;
        }

        GameObject playerBody = Instantiate(inputControlManager.Player[playerIndex].PlayerObject, this.transform);

        playerAnimator = playerBody.GetComponentInChildren<Animator>();

        hammer = playerBody.GetComponentInChildren<Hammer>();
        hammer.gameObject.SetActive(false);

        Transform spawn = inputControlManager.GetCurrentSpawnPoint();
        StartCoroutine(Teleport(spawn));

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
    private float lastDirectionX = 0f;

    private void HandleRotation(Vector2 direction)
    {
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
            else if (isHolding)
            {
                HandleDrop();
            }

            if (interactor.canRepair || isHoldingRepair)
            {
                HandleRepair();
            }
        }
        else if (!canInteract && playerControls.controlEvent.HasInteracted)
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
        // Pick up repair kit if not already holding one
        if (!isHoldingRepair 
            && interactor.currentInteractObject != null
            && interactor.currentInteractObject.CompareTag("Repair"))
        {
            interactor.currentInteractObject.GetComponent<RepairStation>().PlayPickupSound();
            heldRepair.SetActive(true);
            isHoldingRepair = true;
            return; // don't attempt to repair in the same frame as pickup
        }

        // Apply repair kit to damaged wall
        if (isHoldingRepair && interactor.currentRepairTarget != null)
        {
            RepairPatch repair = interactor.currentRepairTarget.GetComponentInChildren<RepairPatch>();

            if (repair != null && !repair.isPatched)
            {
                repair.ActivatePatch();
                interactor.canRepair = false;
                isHoldingRepair = false;
                heldRepair.SetActive(false);
                rigHealth.HealDamage();
            }
        }
    }

    private void HandleMounting()
    {
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
        playerAnimator.SetTrigger("HammerSwing");

        if (ControllerVibrateManager.Instance != null)
        {
            int playerIndex = GetComponent<PlayerInput>().playerIndex;
            ControllerVibrateManager.Instance.Burst(playerIndex, lowFreq: 0.9f, highFreq: 0.3f, duration: 0.2f, delay: 0.3f);
        }

        if (hammer.isPoweredUp)
        {
            hammer.CallStupidPulse();
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}