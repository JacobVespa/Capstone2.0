using UnityEngine;
using System.Collections;

public class CameraCinematic : MonoBehaviour
{
    private Camera mainCamera;

    [Header("Movement Settings")]
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool useTransform = true; // Use transform or direct Vector3
    [SerializeField] private bool maintainZPosition = true;
    [SerializeField] private bool useSmoothDamp = false; // Use SmoothDamp instead of Lerp

    [Header("Completion Settings")]
    [SerializeField] private float arrivalThreshold = 0.1f; // How close to consider "arrived"
    [SerializeField] private bool autoMove = false; // Automatically move on Start
    
    private bool isMoving = false;
    private Vector3 velocity = Vector3.zero; // For SmoothDamp
    private float originalZ;

    private void Start()
    {
        // Cache the main camera
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = GetComponent<Camera>();
        }

        originalZ = mainCamera.transform.position.z;

        if (autoMove)
        {
            MoveCamera();
        }
    }

    private void Update()
    {
        if (isMoving)
        {
            UpdateCameraMovement();
        }
    }

    // Start moving the camera
    public void MoveCamera()
    {
        if (useTransform && targetTransform == null)
        {
            Debug.LogWarning("Target Transform is not assigned!");
            return;
        }

        isMoving = true;
    }

    // Stop the camera movement
    public void StopCamera()
    {
        isMoving = false;
        velocity = Vector3.zero;
    }

    // Set a new target position directly
    public void SetTargetPosition(Vector3 newTarget)
    {
        targetPosition = newTarget;
        useTransform = false;
    }

    // Set a new target transform
    public void SetTargetTransform(Transform newTarget)
    {
        targetTransform = newTarget;
        useTransform = true;
    }

    private void UpdateCameraMovement()
    {
        Vector3 target = GetTargetPosition();

        if (maintainZPosition)
        {
            target.z = originalZ;
        }

        // Choose movement method
        if (useSmoothDamp)
        {
            mainCamera.transform.position = Vector3.SmoothDamp(
                mainCamera.transform.position, 
                target, 
                ref velocity, 
                1f / moveSpeed
            );
        }
        else
        {
            mainCamera.transform.position = Vector3.Lerp(
                mainCamera.transform.position, 
                target, 
                moveSpeed * Time.deltaTime
            );
        }

        // Check if arrived
        if (Vector3.Distance(mainCamera.transform.position, target) < arrivalThreshold)
        {
            mainCamera.transform.position = target;
            isMoving = false;
        }
    }

    private Vector3 GetTargetPosition()
    {
        if (useTransform && targetTransform != null)
        {
            return targetTransform.position;
        }
        return targetPosition;
    }

    // Move to position with custom speed (one-shot)
    public void MoveCameraTo(Vector3 destination, float customSpeed = -1f)
    {
        if (customSpeed > 0)
        {
            StartCoroutine(MoveCameraToPosition(destination, customSpeed));
        }
        else
        {
            targetPosition = destination;
            useTransform = false;
            MoveCamera();
        }
    }

    private IEnumerator MoveCameraToPosition(Vector3 destination, float speed)
    {
        if (maintainZPosition)
        {
            destination.z = originalZ;
        }

        Vector3 localVelocity = Vector3.zero;

        while (Vector3.Distance(mainCamera.transform.position, destination) > arrivalThreshold)
        {
            if (useSmoothDamp)
            {
                mainCamera.transform.position = Vector3.SmoothDamp(
                    mainCamera.transform.position, 
                    destination, 
                    ref localVelocity, 
                    1f / speed
                );
            }
            else
            {
                mainCamera.transform.position = Vector3.Lerp(
                    mainCamera.transform.position, 
                    destination, 
                    speed * Time.deltaTime
                );
            }

            yield return null;
        }

        mainCamera.transform.position = destination;
    }

    // Check if camera is currently moving
    public bool IsMoving()
    {
        return isMoving;
    }

    // Instantly snap to target
    public void SnapToTarget()
    {
        Vector3 target = GetTargetPosition();
        if (maintainZPosition)
        {
            target.z = originalZ;
        }
        mainCamera.transform.position = target;
        isMoving = false;
    }
}