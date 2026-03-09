using UnityEngine;
using System.Collections;

public class CameraCinematic : MonoBehaviour
{
    private Camera mainCamera;

    [Header("Movement Settings")]
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool maintainZPosition = true;
    [SerializeField] private bool useSmoothDamp = false;

    [Header("Completion Settings")]
    [SerializeField] private float arrivalThreshold = 0.1f;
    [SerializeField] private bool autoMove = false;

    private bool isMoving = false;
    private Vector3 velocity = Vector3.zero;
    private float originalZ;

    private Coroutine shakeCoroutine;

    private void Awake()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = GetComponent<Camera>();
        }

        originalZ = mainCamera.transform.position.z;
    }

    private void Start()
    {
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

    public void MoveCamera()
    {
        isMoving = true;
    }

    public void StopCamera()
    {
        isMoving = false;
        velocity = Vector3.zero;
    }

    public void SetTargetPosition(Vector3 newTarget)
    {
        targetPosition = newTarget;
    }

    private void UpdateCameraMovement()
    {
        Vector3 target = targetPosition;

        if (maintainZPosition)
        {
            target.z = originalZ;
        }

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

        if (Vector3.Distance(mainCamera.transform.position, target) < arrivalThreshold)
        {
            mainCamera.transform.position = target;
            isMoving = false;
        }
    }

    public void MoveCameraTo(Vector3 destination, float customSpeed = -1f)
    {
        if (customSpeed > 0)
        {
            StartCoroutine(MoveCameraToPosition(destination, customSpeed));
        }
        else
        {
            targetPosition = destination;
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

    public bool IsMoving()
    {
        return isMoving;
    }

    public void SnapToTarget()
    {
        Vector3 target = targetPosition;
        if (maintainZPosition)
        {
            target.z = originalZ;
        }

        mainCamera.transform.position = target;
        isMoving = false;
    }

    public void ShakeCamera(float duration, float magnitude)
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }

        //shakeCoroutine = StartCoroutine(Shake(duration, magnitude));
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0f;

        Vector3 shakeOrigin = mainCamera.transform.position;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            mainCamera.transform.position = new Vector3(
                shakeOrigin.x + offsetX,
                shakeOrigin.y + offsetY,
                shakeOrigin.z
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Restore to where the camera actually was
        mainCamera.transform.position = shakeOrigin;
        shakeCoroutine = null;
    }

    public void MoveCameraToPosition(Vector3 newTarget)
    {
        SetTargetPosition(newTarget);
        MoveCamera();
    }



    public void PanOver(float pauseDuration, float speed, Vector3 location)
    {
        StartCoroutine(PanOverLocation(pauseDuration, speed, location));
    }

    private IEnumerator PanOverLocation(float duration, float speed, Vector3 location)
    {
        MoveCameraTo(location, speed);

        yield return new WaitForSeconds(duration);

        MoveCamera();
    }

}