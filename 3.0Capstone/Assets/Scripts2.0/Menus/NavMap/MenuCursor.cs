using UnityEngine;
using UnityEngine.EventSystems;

public class MenuCursor : MonoBehaviour
{
    [Header("Circle")]
    [SerializeField] private Transform P1Circle_Sprite;  // the circle sprites
    [SerializeField] private Transform P2Circle_Sprite;
    [SerializeField] private Transform both_Sprite;

    [Header("Head")]
    [SerializeField] private Transform P1Head_Sprite; // Player sprites
    [SerializeField] private Transform P2Head_Sprite;

    [Header("Offset")]
    [SerializeField] private Vector3 offset;       // tweak in inspector if needed

    private void Update()
    {
        GameObject selected = EventSystem.current?.currentSelectedGameObject;

        if (selected == null) return;

        Vector3 targetPos = selected.transform.position + offset;
        targetPos.z = both_Sprite.position.z; // keep circle on correct z layer

        both_Sprite.position = targetPos;
        VotePlacement(targetPos);
    }

    //TODO detect individual input to move circles based on players vote/movement
    private void VotePlacement(Vector3 target)
    {
        P1Head_Sprite.parent = both_Sprite;
        P2Head_Sprite.parent = both_Sprite;

        P1Head_Sprite.position = target + new Vector3(-30, -30, 0);
        P2Head_Sprite.position = target + new Vector3(30, -30, 0);
    }

    //TODO add a timer that displays how long until a choice is made
    // Picks a random level if the vote is split
    // Picks the selected level if both votes exist on the same level
    // Timer ends if both players vote on the same level before it hits 0
}