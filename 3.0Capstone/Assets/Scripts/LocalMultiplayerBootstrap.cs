using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class LocalMultiplayerBootstrap : MonoBehaviour
{
    [SerializeField]
    private Rect[] viewports =
    {
        new Rect(0f, 0.5f, 1f, 0.5f), // P1 top
        new Rect(0f, 0f,   1f, 0.5f), // P2 bottom
    };

    private readonly List<PlayerInput> players = new();

    public void OnPlayerJoined(PlayerInput p)
    {
        players.Add(p);
        int i = Mathf.Clamp(players.Count - 1, 0, viewports.Length - 1);
        var cam = p.GetComponentInChildren<Camera>();
        if (cam != null) cam.rect = viewports[i];

    }
}