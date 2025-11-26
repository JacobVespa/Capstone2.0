using System.Collections;
using UnityEngine;

public class TurretScript : MonoBehaviour
{
    [SerializeField] private Transform barrelTransform;
    [SerializeField] private Transform seatTransform;

    private GameObject currentPlayer;
    private CharacterController controller;
    private PlayerBody body;

    private bool canMount;
    private bool isMounted;

    void Update()
    {
        if (canMount && !isMounted)
        {
            StartCoroutine(Teleport());
            isMounted = true;
        }
    }

    private IEnumerator Teleport()
    {
        if (controller == null || currentPlayer == null)
            yield break;

        controller.enabled = false;

        // Move player to the turret seat
        currentPlayer.transform.position = seatTransform.position;
        currentPlayer.transform.rotation = seatTransform.rotation;
        body.EnableGravity(isMounted);

        yield return null;

        controller.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            currentPlayer = other.gameObject;
            controller = currentPlayer.GetComponent<CharacterController>();
            body = currentPlayer.GetComponent<PlayerBody>();
            canMount = true;
            isMounted = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            currentPlayer = null;
            controller = null;
            body = null;
            canMount = false;
        }
    }
}