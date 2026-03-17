using UnityEngine;
using UnityEngine.EventSystems;

public class MenuCursor : MonoBehaviour
{
    [SerializeField] private Transform cursorSprite;  // the circle sprite
    [SerializeField] private float moveSpeed = 0f;   // set to 0 for instant snap
    [SerializeField] private Vector3 offset;          // tweak in inspector if needed

    private void Update()
    {
        GameObject selected = EventSystem.current?.currentSelectedGameObject;

        if (selected == null) return;

        Vector3 targetPos = selected.transform.position + offset;
        targetPos.z = cursorSprite.position.z; // keep circle on correct z layer

        if (moveSpeed <= 0)
        {
            cursorSprite.position = targetPos;
        }
        else
        {
            cursorSprite.position = Vector3.Lerp(
                cursorSprite.position,
                targetPos,
                moveSpeed * Time.unscaledDeltaTime
            );
        }
    }
}