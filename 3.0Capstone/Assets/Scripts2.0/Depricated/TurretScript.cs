using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class TurretScript : MonoBehaviour
{
    [SerializeField] private Transform barrelTransform;
    [SerializeField] private Transform seatTransform;
    [SerializeField] private Transform entryTransform;

    private List<GameObject> playersInRange = new List<GameObject>();
    private GameObject mountedPlayer;
    
    private CharacterController controller;
    private PlayerBody body;
    private PlayerInput input;

    [SerializeField] private Transform turretTransformOrigin;
    private Transform turretTransform;
    private Quaternion originalRotation;
    private float returnSpeed = 10f;

    private void Start()
    {
        turretTransform = turretTransformOrigin;
        originalRotation = turretTransform.localRotation;
    }

    void Update()
    {
        // If no one is mounted, check all players in range for interact input
        if (mountedPlayer == null)
        {
            foreach (GameObject player in playersInRange)
            {
                if (CheckPlayerInput(player))
                {
                    MountPlayer(player);                  
                    break; // Only let one player mount
                }
            }
        }
        // If someone is mounted, check if they want to exit
        else if (ButtonPressed())
        {
            StartCoroutine(TeleportAndUnmount());
        }

        if (mountedPlayer != null) UpdateMovement();
        else ResetTurretPosition();
    }

    private float yaw,pitch;

    private void UpdateMovement()
    {
        if (input != null)
        {
            Vector2 lookInput = input.actions["Look"].ReadValue<Vector2>();
            float sensitivity = body.LookSensitivity;

            yaw += lookInput.x * sensitivity;
            pitch -= lookInput.y * sensitivity;

            pitch = Mathf.Clamp(pitch, -20f, 80f);
            yaw = Mathf.Clamp(yaw, -80f, 80f);

            turretTransform.localRotation = Quaternion.Euler(pitch,yaw,0f);
        }
    }

    public void ResetTurretPosition()
    {
        if (turretTransform.localRotation != originalRotation)
        {
            // Smoothly return turret to original rotation
            turretTransform.localRotation = Quaternion.Lerp(
                turretTransform.localRotation,
                originalRotation,
                returnSpeed * Time.deltaTime
            );
        }
    }

    private bool CheckPlayerInput(GameObject player)
    {
        input = player.GetComponent<PlayerInput>();
        if (input == null) return false;
        return input.actions["Interact"].WasPerformedThisFrame();
    }

    private bool ButtonPressed() // Temp until a perminent control solution is figured out!
    {
        if (input == null) return false;
        return input.actions["Interact"].WasPerformedThisFrame();
    }

    private void MountPlayer(GameObject player)
    {
        yaw = 0; pitch = 0;

        mountedPlayer = player;
        controller = player.GetComponent<CharacterController>();
        body = player.GetComponent<PlayerBody>();
        input = player.GetComponent<PlayerInput>();
        
        // Stop player movement but allow looking
        if (body != null)
        {
            body.EnterStation();
        }

        // Parent player to turret
        player.transform.SetParent(turretTransform);

        // Enable shooting
        GunnerScript gunner = GetComponent<GunnerScript>();
        if (gunner != null)
        {
            gunner.SetPlayerInput(input);
        }
        
        StartCoroutine(Teleport(player, seatTransform, false));
    }

    private void UnmountPlayer()
    {
        // Disable shooting
        GunnerScript gunner = GetComponent<GunnerScript>();
        if (gunner != null)
        {
            gunner.ClearPlayerInput();
        }
        
        // Re-enable player movement before clearing reference
        if (body != null)
        {
            body.ExitStation();
        }

        // Unparent player
        if (mountedPlayer != null)
        {
            mountedPlayer.transform.SetParent(null);
        }

        mountedPlayer = null;
        controller = null;
        body = null;
        input = null;
        turretTransform = turretTransformOrigin;
    }

    private IEnumerator TeleportAndUnmount()
    {
        GameObject playerToUnmount = mountedPlayer;
        yield return StartCoroutine(Teleport(mountedPlayer, entryTransform, true));
        
        // Check if player is still in trigger after teleport
        // If not in trigger collider, OnTriggerExit won't fire, so remove manually
        if (!IsInTrigger(playerToUnmount))
        {
            playersInRange.Remove(playerToUnmount);
        }
        
        UnmountPlayer();
    }

    private bool IsInTrigger(GameObject player)
    {
        Collider triggerCollider = GetComponent<Collider>();
        if (triggerCollider == null) return false;
        
        Collider playerCollider = player.GetComponent<Collider>();
        if (playerCollider == null) return false;
        
        return triggerCollider.bounds.Intersects(playerCollider.bounds);
    }

    private IEnumerator Teleport(GameObject player, Transform location, bool gravity)
    {
        if (controller == null || player == null || body == null)
            yield break;

        controller.enabled = false;

        // Move player to the location
        player.transform.position = location.position;
        player.transform.rotation = location.rotation;
        body.EnableGravity(gravity);

        yield return null;

        controller.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!playersInRange.Contains(other.gameObject))
            {
                playersInRange.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playersInRange.Remove(other.gameObject);
            
            // If the mounted player physically left the trigger, force unmount
            if (mountedPlayer == other.gameObject)
            {
                UnmountPlayer();
            }
        }
    }
}