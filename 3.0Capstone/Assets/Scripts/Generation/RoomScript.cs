using UnityEditor;
using UnityEngine;

public class RoomScript : MonoBehaviour
{

    public Room roomData; //Assigned by roomManager when the prefab is created
    private DEPRECATED_RoomManager roomManager;

    public Transform northWall;
    private Transform[] tunnelSpawnNorth = new Transform[3];

    public Transform southWall;
    private Transform[] tunnelSpawnSouth = new Transform[3];

    private void Start()
    {
        tunnelSpawnNorth = GetImmediateChildren(northWall);
        tunnelSpawnSouth = GetImmediateChildren(southWall);

        roomManager = FindAnyObjectByType<DEPRECATED_RoomManager>(); // Because there is only one roomManager
        SpawnTunnel();
    }

    private void SpawnTunnel()
    {
        SpawnTunnel(tunnelSpawnNorth, roomData.next.Count, Vector3.forward);
        SpawnTunnel(tunnelSpawnSouth, roomData.back.Count, Vector3.back);
    }

    private void SpawnTunnel(Transform[] spawnPoints, int count, Vector3 forwardDir)
    {
        // Determine which indices to use based on count
        int[] indices = (count == 2) ? new[] { 0, 2 } : new[] { 1 };

        foreach (int i in indices)
        {
            Instantiate(
                roomManager.Tunnel,
                spawnPoints[i].position,
                Quaternion.LookRotation(forwardDir, Vector3.up)
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) roomManager.OnPlayerEnterRoom(roomData);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) roomManager.OnPlayerExitRoom(roomData);
    }

    private Transform[] GetImmediateChildren(Transform parent)
    {
        int count = parent.childCount;
        Transform[] children = new Transform[count];

        for (int i = 0; i < count; i++)
        {
            children[i] = parent.GetChild(i);
        }

        return children;
    }

}