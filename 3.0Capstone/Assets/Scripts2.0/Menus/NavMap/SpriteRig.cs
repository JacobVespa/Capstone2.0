using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpriteRig : MonoBehaviour
{
    [SerializeField] private Image rigImage;
    [SerializeField] private RectTransform mapContainer;

    [Header("Movement")]
    [SerializeField] private float moveDuration = 0.6f;
    [SerializeField] private float jumpHeight = 30f;       // Arc height during lerp
    [SerializeField] private float sideWobble = 20f;       // Side-to-side wobble amount

    [Header("Shake")]
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeMagnitude = 8f;

    private RectTransform rigRect;
    private bool isAnimating = false;

    public static SpriteRig Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        rigRect = rigImage.GetComponent<RectTransform>();
    }

    /// <summary>
    /// Snaps rig to a position with no animation. Used on map load/restore.
    /// </summary>
    public void SnapTo(Vector2 anchoredPosition)
    {
        rigRect.anchoredPosition = anchoredPosition;
        rigImage.gameObject.SetActive(true);
    }

    /// <summary>
    /// Moves rig to target, shakes, then invokes onComplete (used to launch level).
    /// </summary>
    public void MoveAndShake(Vector2 targetPosition, System.Action onComplete)
    {
        if (isAnimating) return;
        StartCoroutine(MoveShakeThenLaunch(targetPosition, onComplete));
    }

    private IEnumerator MoveShakeThenLaunch(Vector2 target, System.Action onComplete)
    {
        isAnimating = true;

        yield return StartCoroutine(SmoothMove(rigRect.anchoredPosition, target));
        yield return StartCoroutine(Shake());

        isAnimating = false;
        onComplete?.Invoke();
    }

    private IEnumerator SmoothMove(Vector2 from, Vector2 to)
    {
        float elapsed = 0f;
        //int wobbleDirection = 1;

        while (elapsed < moveDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);

            // Smooth step for easing
            float smoothT = t * t * (3f - 2f * t);

            // Base lerp position
            Vector2 basePos = Vector2.Lerp(from, to, smoothT);

            // Arc: parabola peaks in the middle
            float arc = Mathf.Sin(t * Mathf.PI) * jumpHeight;

            // Side wobble: oscillates left/right during travel
            float wobble = Mathf.Sin(t * Mathf.PI * 4f) * sideWobble;

            rigRect.anchoredPosition = new Vector2(basePos.x + wobble, basePos.y + arc);

            yield return null;
        }

        rigRect.anchoredPosition = to;
    }

    private IEnumerator Shake()
    {
        Vector2 origin = rigRect.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float strength = Mathf.Lerp(shakeMagnitude, 0f, elapsed / shakeDuration);

            rigRect.anchoredPosition = origin + Random.insideUnitCircle * strength;
            yield return null;
        }

        rigRect.anchoredPosition = origin;
    }
}