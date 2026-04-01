using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessManager : MonoBehaviour
{
    public static PostProcessManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private GameObject postProcessor;
    private Volume volume;
    private ChromaticAberration aberration;

    [SerializeField] private float SpeedUp = 0.109f;
    [SerializeField] private float SpeedDown = 0f;

    [SerializeField] private float transitionSpeed = 2f;

    public void Start()
    {
        postProcessor = GameObject.FindWithTag("PP");

        if (postProcessor != null)
            volume = postProcessor.GetComponent<Volume>();

        if (volume != null && volume.profile != null)
            volume.profile.TryGet(out aberration);

        if (aberration == null)
            Debug.LogWarning("Chromatic Aberration not found in Volume Profile!");
        else
            ChangeIntensity(SpeedDown);

    }

    public void ChangeSpeed()
    {
        StopAllCoroutines();
        StartCoroutine(FadeToIntensity(SpeedUp));
    }

    private IEnumerator FadeToIntensity(float target)
    {
        if (aberration == null)
            yield break;

        float current = aberration.intensity.value;

        while (Mathf.Abs(current - target) > 0.001f)
        {
            current = Mathf.MoveTowards(current, target, transitionSpeed * Time.deltaTime);
            ChangeIntensity(current);
            yield return null;
        }

        ChangeIntensity(target);
    }

    private void ChangeIntensity(float intensity)
    {
        aberration.intensity.Override(intensity);
    }

    public void ResetEffect()
        {
            StopAllCoroutines();
            StartCoroutine(FadeToIntensity(SpeedDown));
    }

}
