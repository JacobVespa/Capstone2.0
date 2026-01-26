using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerControls playerControls;
    private CharacterController player;
    private PlayerInteract interactor;
    private InputControlManager inputControlManager;
    private Animator playerAnimator;
    private RigHealth rigHealth;
    private Engine engine;
    private AudioSource playerAudioSource;

    [SerializeField] private AudioClip hammerMiss;

    [Header("Sprites")]
    [SerializeField] private GameObject heldAmmo;
    [SerializeField] private GameObject heldRepair;

    [Header("Booleans")]
    public bool isHoldingAmmo;
    private bool isHoldingRepair;
    private bool canAttack = true;

    [Header("Custom Variables")]
    [SerializeField] private float movementSpeed = 5.0f;

    [Header("Debug Values")]
    [SerializeField] private bool isMounted = false;
    [SerializeField] private bool isHolding = false;
    public bool canMove = true;
    public bool canInteract = true;
    [SerializeField] float attackCooldown = 1.0f;


    private int playerIndex; // Which player this is (0 or 1)

    private void Start()
    {
        playerControls = GetComponent<PlayerControls>();
        player = GetComponent<CharacterController>();
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

    private void HandleMovement(Vector2 direction)
    {
        player.Move((direction * movementSpeed) * Time.deltaTime);
        
        // Only set walk animation to true if actually moving
        //bool isMoving = moveDirection.magnitude > 0.1f;
        if(direction.magnitude > 0)
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
            else if (interactor.canRepair)
            {
                HandleRepair();
            }
            else if (interactor.canEngine)
            {
                HandleEngine();
            }
        }
        else if (playerControls.controlEvent.HasDisengaged)
        {
            HandleDismounting();
            HandleDrop();
        }
        else if(playerControls.controlEvent.HasSwungHammer && canAttack)
        {
            
            StartCoroutine(HammerSwing());
            
        }
    }

    private void HandleEngine()
    {
        if (interactor.currentInteractObject.CompareTag("Engine"))
        {
            engine.EngineRepair();
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

        if (isHoldingRepair && interactor.currentInteractObject.CompareTag("Damaged"))
        {
            interactor.currentInteractObject.SetActive(false);
            interactor.canRepair = false;
            isHoldingRepair = false;
            heldRepair.SetActive(false);
            rigHealth.HealDamage(2);
        }
    }

    private void HandleMounting()
    {
        if (isHolding) HandleDrop();

        isMounted = true;
        canMove = false;
        interactor.currentInteractObject.GetComponent<Turret>().Mount(this.gameObject);
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
        
        isMounted = false;
        canMove = true;
        interactor.currentInteractObject.GetComponent<Turret>().Dismount();
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
        player.enabled = false;
        this.transform.position = location.position;
        player.enabled = true;
        yield return null;
    }

    IEnumerator HammerSwing()
    {
        canAttack = false;
        playerAudioSource.clip = hammerMiss;
        playerAudioSource.Play();
        Debug.Log("Tried to swing the hammer...");
        playerAnimator.SetTrigger("HammerSwing");
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}