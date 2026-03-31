using UnityEngine;
using System.Collections;

public class TransitionCanvas : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float startRotationZ = 15f;
    [SerializeField] private float rotationNormalizeDuration = 0.6f;
    [SerializeField] private float rotationStartDelay = 0f;

    [Header("Scale Settings")]
    [SerializeField] private float startScale = 1.4f;
    [SerializeField] private float scaleDuration = 0.6f;
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Slide Down Settings")]
    [SerializeField] private float slideDistance = 500f;
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private float slideStartDelay = 1.5f;

    [Header("Easing")]
    [SerializeField] private AnimationCurve rotationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve slideCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Vector3 originalScale;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
    }

    private void OnEnable()
    {
        originalPosition = rectTransform.anchoredPosition;

        rectTransform.localEulerAngles = new Vector3(0f, 0f, startRotationZ);
        rectTransform.localScale = Vector3.one * startScale;

        StopAllCoroutines();
        StartCoroutine(RunSequence());
    }

    private IEnumerator RunSequence()
    {
        // Run rotation and scale together, wait for the longer of the two
        Coroutine scaleCoroutine = StartCoroutine(NormalizeScale());
        yield return StartCoroutine(NormalizeRotation());
        yield return scaleCoroutine;

        yield return StartCoroutine(SlideDown());
    }

    private IEnumerator NormalizeRotation()
    {
        if (rotationStartDelay > 0f)
            yield return new WaitForSeconds(rotationStartDelay);

        float elapsed = 0f;

        while (elapsed < rotationNormalizeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / rotationNormalizeDuration);
            float curvedT = rotationCurve.Evaluate(t);
            float currentAngle = Mathf.LerpAngle(startRotationZ, 0f, curvedT);
            rectTransform.localEulerAngles = new Vector3(0f, 0f, currentAngle);
            yield return null;
        }

        rectTransform.localEulerAngles = Vector3.zero;
    }

    private IEnumerator NormalizeScale()
    {
        if (rotationStartDelay > 0f)
            yield return new WaitForSeconds(rotationStartDelay);

        float elapsed = 0f;

        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / scaleDuration);
            float curvedT = scaleCurve.Evaluate(t);
            rectTransform.localScale = Vector3.Lerp(Vector3.one * startScale, originalScale, curvedT);
            yield return null;
        }

        rectTransform.localScale = originalScale;
    }

    private IEnumerator SlideDown()
    {
        if (slideStartDelay > 0f)
            yield return new WaitForSeconds(slideStartDelay);

        float elapsed = 0f;
        Vector2 targetPosition = originalPosition + Vector2.down * slideDistance;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / slideDuration);
            float curvedT = slideCurve.Evaluate(t);
            rectTransform.anchoredPosition = Vector2.Lerp(originalPosition, targetPosition, curvedT);
            yield return null;
        }

        rectTransform.anchoredPosition = targetPosition;
    }
}