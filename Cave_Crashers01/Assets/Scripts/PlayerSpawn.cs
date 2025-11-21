using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawn : MonoBehaviour
{
    public Transform[] spawnPoints;
    private int playerCount = 0;

    [SerializeField]
    private Rect[] viewports =
    {
        new Rect(0f, 0.5f, 1f, 0.5f), // P1 top
        new Rect(0f, 0f,   1f, 0.5f), // P2 bottom
    };

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        StartCoroutine(SetSpawn(playerInput));
    }

    private IEnumerator SetSpawn(PlayerInput playerInput)
    {
        yield return null;

        playerInput.gameObject.transform.position = spawnPoints[playerCount].transform.position;
        Camera cam = playerInput.GetComponentInChildren<Camera>();
        if (cam == null)
        {
            yield break;
        }

        if (playerCount == 0)
        {
            cam.cullingMask = ~LayerMask.GetMask("p2");
            
        }
        else
        {
            cam.cullingMask = ~LayerMask.GetMask("p1");
        }

        cam.rect = viewports[playerCount];
        playerCount++;
    }
}
