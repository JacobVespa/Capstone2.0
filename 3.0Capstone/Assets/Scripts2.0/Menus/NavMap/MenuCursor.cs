using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Attach to a GameObject in the CaveMap scene alongside MenuCursor's sprites.
public class MenuCursor : MonoBehaviour
{
    [Header("Circle Sprites")]
    [SerializeField] private Transform P1Circle_Sprite;
    [SerializeField] private Transform P2Circle_Sprite;

    [Header("Head Sprites")]
    [SerializeField] private Transform P1Head_Sprite;
    [SerializeField] private Transform P2Head_Sprite;

    [Header("Canvas Reference")]
    // Drag the root Canvas of your map UI here
    [SerializeField] private Canvas rootCanvas;

    [Header("Offset")]
    [SerializeField] private Vector2 offset;

    [Header("Vote Settings")]
    [SerializeField] private float autoSelectDelay = 10f;
    private float voteTimer = 0f;
    private bool timerRunning = false;
    private bool voteConfirmed = false;

    // Short cooldown on startup prevents an instant confirm when both
    // players are initialised on the same button
    [SerializeField] private float confirmCooldown = 0.5f;
    private float confirmCooldownTimer = 0f;
    [SerializeField] private TMP_Text countDownTimer;

    private void Start()
    {
        confirmCooldownTimer = confirmCooldown;
        voteTimer = autoSelectDelay;
    }

    private void Update()
    {
        if (confirmCooldownTimer > 0f)
        {
            confirmCooldownTimer -= Time.deltaTime;
            return;
        }

        GameObject sel0 = GetSelection(0);
        GameObject sel1 = GetSelection(1);

        UpdateButtonStates(sel0, sel1);

        MoveCursor(P1Circle_Sprite, P1Head_Sprite, sel0, new Vector2(-30, -30));
        MoveCursor(P2Circle_Sprite, P2Head_Sprite, sel1, new Vector2( 30, -30));

        if (!voteConfirmed)
            HandleVoteTimer(sel0, sel1);
    }

    // Reads from whichever input manager is present in this scene
    private static GameObject GetSelection(int playerIndex)
    {
        if (CaveMapInputManager.Instance != null)
            return CaveMapInputManager.Instance.GetPlayerSelection(playerIndex);

        return null;
    }

    private void MoveCursor(Transform circle, Transform head, GameObject selected, Vector2 headOffset)
    {
        if (circle == null || selected == null || rootCanvas == null) return;

        RectTransform circleRect = circle.GetComponent<RectTransform>();
        RectTransform headRect   = head.GetComponent<RectTransform>();
        RectTransform canvasRect = rootCanvas.GetComponent<RectTransform>();

        Camera cam = rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main;

        // Convert the button's world position into canvas local space
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, selected.transform.position);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenPoint, cam, out Vector2 localPoint
        );

        if (circleRect != null) circleRect.anchoredPosition = localPoint + offset;
        if (headRect   != null) headRect.anchoredPosition   = localPoint + offset + headOffset;
    }

    private void HandleVoteTimer(GameObject sel0, GameObject sel1)
    {
        if (sel0 == null || sel1 == null) 
        {
            countDownTimer.text = "Ready!";
            return;
        }

        if (sel0 == sel1)
        {
            timerRunning = false;
            countDownTimer.text = "Both Ready!";
        }
        else
        {
            if (!timerRunning)
            {
                timerRunning = true;
            }

            voteTimer -= Time.deltaTime;
            // TODO: push voteTimer to a UI countdown display here
            if (countDownTimer != null) 
                countDownTimer.text = Mathf.CeilToInt(voteTimer).ToString();
            else
                Debug.Log("Timer Object not present in inspector");

            if (voteTimer <= 0f)
            {
                timerRunning = false;
                ConfirmSelection(Random.value < 0.5f ? sel0 : sel1);
            }
        }
    }

    private void ConfirmSelection(GameObject node)
    {
        voteConfirmed = true;

        Button btn = node.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.Invoke();   // Simulates a real button press
        }
    }

    private void UpdateButtonStates(GameObject sel0, GameObject sel1)
    {
        MapButton[] allButtons = FindObjectsByType<MapButton>(FindObjectsSortMode.None);

        foreach (var mb in allButtons)
            mb.SetPressAllowed(false);

        bool singlePlayer = sel0 != null && sel1 == null;

        if (singlePlayer)
        {
            MapButton mb = sel0.GetComponent<MapButton>();
            if (mb != null) mb.SetPressAllowed(true);
            return;
        }

        if (sel0 != null && sel1 != null && sel0 == sel1)
        {
            MapButton mb = sel0.GetComponent<MapButton>();
            if (mb != null) mb.SetPressAllowed(true);
        }
    }
}