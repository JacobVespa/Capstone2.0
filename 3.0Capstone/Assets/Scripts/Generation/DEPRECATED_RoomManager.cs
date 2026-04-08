using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

public class DEPRECATED_RoomManager : MonoBehaviour
{

    [SerializeField] GameObject chamber;
    [SerializeField] GameObject tunnel;
    public GameObject Tunnel => tunnel;

    public int roomSpacing = 10;

    private Room start;
    private Room end;

    static int FRONT_LAYER = 5;

    private List<Room> allRooms = new List<Room>();

    private void Start()
    {
        RoomGeneration();
        //DisableRooms();
    }

    #region RoomGeneration
    //
    // Generates rooms by making a diamond pattern. Starts with one node and ends with one node
    //
    public void RoomGeneration()
    {
        allRooms.Clear();
        start = CreateRoom(chamber, RoomType.Start, Vector3.zero);
        allRooms.Add(start);

        List<Room> lastLayer = new List<Room>();
        List<Room> currentLayer;
        lastLayer.Add(start);

        SpawnRoom(start);

        // First half - expanding diamond
        for (int i = 0; i < FRONT_LAYER; i++)
        {
            currentLayer = new List<Room>();

            int spaceIndex = 0;
            for (int j = i + 2; j > 0; j--)
            {
                Room newRoom = CreateRoom(chamber, RoomType.Default, new Vector3((i + 1) * roomSpacing, 0.0f, ((i + 1) * roomSpacing) - (spaceIndex * roomSpacing * 2)));

                currentLayer.Add(newRoom);
                allRooms.Add(newRoom);
                SpawnRoom(newRoom);

                spaceIndex++;
            }

            // Connect layers
            for (int last = 0; last < lastLayer.Count; last++)
            {
                if (last + 1 != lastLayer.Count)
                {
                    for (int cur = last; cur < last + 2; cur++)
                    {
                        lastLayer[last].next.Add(currentLayer[cur]);
                        currentLayer[cur].back.Add(lastLayer[last]);
                    }
                }
                else
                {
                    lastLayer[last].next.Add(currentLayer[currentLayer.Count - 1]);
                    lastLayer[last].next.Add(currentLayer[currentLayer.Count - 2]);
                }
            }

            lastLayer = currentLayer;
        }

        // Second half - converging diamond
        int layerOffset = 0; // Track how many layers we've added
        for (int i = FRONT_LAYER - 1; i >= 0; i--)
        {
            currentLayer = new List<Room>();

            int spaceIndex = 0;
            for (int j = i + 1; j > 0; j--) // One fewer room each layer
            {
                int xPos = (FRONT_LAYER + 1) + layerOffset;
                Room newRoom = CreateRoom(chamber, RoomType.Default, new Vector3(xPos * roomSpacing, 0.0f, (i * roomSpacing) - (spaceIndex * roomSpacing * 2)));

                currentLayer.Add(newRoom);
                allRooms.Add(newRoom);
                SpawnRoom(newRoom);

                spaceIndex++;
            }

            // Connect layers - REVERSED logic for converging
            for (int cur = 0; cur < currentLayer.Count; cur++)
            {
                if (cur + 1 != currentLayer.Count)
                {
                    for (int last = cur; last < cur + 2; last++)
                    {
                        lastLayer[last].next.Add(currentLayer[cur]);
                        currentLayer[cur].back.Add(lastLayer[last]);
                    }
                }
                else
                {
                    currentLayer[cur].back.Add(lastLayer[lastLayer.Count - 1]);
                    currentLayer[cur].back.Add(lastLayer[lastLayer.Count - 2]);
                    lastLayer[lastLayer.Count - 1].next.Add(currentLayer[cur]);
                    lastLayer[lastLayer.Count - 2].next.Add(currentLayer[cur]);
                }
            }

            lastLayer = currentLayer;
            layerOffset++; // Increment by 1 each iteration
        }

        // Connect to end room
        end = CreateRoom(chamber, RoomType.End, new Vector3((FRONT_LAYER + 1 + layerOffset) * roomSpacing, 0, 0));
        allRooms.Add(end);

        foreach (Room room in lastLayer)
        {
            room.next.Add(end);
            end.back.Add(room);
        }

        SpawnRoom(end);
    }

    private Room CreateRoom(GameObject r, RoomType t, Vector3 l)
    {
        Room newRoom = new Room();
        newRoom.room = r;
        newRoom.type = t;
        newRoom.location = l;

        return newRoom;
    }

    #endregion

    #region RoomCommands

    private void SpawnRoom(Room roomToSpawn)
    {
        GameObject spawnedRoom = Instantiate(roomToSpawn.room, roomToSpawn.location, Quaternion.identity);
        roomToSpawn.room = spawnedRoom;

        RoomScript trigger = spawnedRoom.GetComponent<RoomScript>();
        trigger.roomData = roomToSpawn;
    }

    public void DisableRooms()
    {
        foreach (Room r in allRooms)
        {
            if (r.room != null && r.type != RoomType.Start)
            {
                r.room.SetActive(false);
            }
        }
    }

    public void OnPlayerEnterRoom(Room room)
    {
        
    }
    
    public void OnPlayerExitRoom(Room room)
    {
        
    }

    public void EnableConnectedRooms(Room currentRoom)
    {
        if (currentRoom.next != null)
        {
            foreach (Room nextRoom in currentRoom.next)
            {
                nextRoom.room.SetActive(true);
            }
        }

        if (currentRoom.back != null)
        {
            foreach (Room backRoom in currentRoom.back)
            {
                backRoom.room.SetActive(true);
            }
        }
    }

    #endregion

    #region Debugging

    private void OnDrawGizmos()
    {
        if (allRooms == null || allRooms.Count == 0)
            return;

        Gizmos.color = Color.cyan;
        foreach (var room in allRooms)
        {
            // Draw node
            Gizmos.DrawSphere(room.location, 0.5f);

            // Draw connections
            if (room.next != null)
            {
                foreach (var next in room.next)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(room.location, next.location);
                    Gizmos.color = Color.cyan;
                }
            }
        }
    }

    #endregion

}

#region RoomParameters

public class Room // Add indexing to reference this!
{
    public GameObject room;
    public RoomType type;
    public List<Room> next = new List<Room>();
    public List<Room> back = new List<Room>();
    public Vector3 location;

}

public enum RoomType
{
    Start,
    Horde,
    Treasure,
    Resource,
    End,
    Default
}

#endregion