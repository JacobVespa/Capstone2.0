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

    public void Start()
    {
        postProcessor = GameObject.FindWithTag("PP");

        if (postProcessor != null)
            volume = postProcessor.GetComponent<Volume>();

        if (volume != null)
            aberration = volume.GetComponent<ChromaticAberration>();

        if (aberration == null)
            Debug.Log("Aberration Not Found!");

    }

    public void ChangeSpeed()
    {
        StartCoroutine(Speed());
    }

    private IEnumerator Speed()
    {
        ChangeIntensity(SpeedUp);

        //yield return new WaitForSeconds();

        ChangeIntensity(SpeedDown);

        yield return null;
    }

    private void ChangeIntensity(float intensity)
    {
        aberration.intensity.Override(intensity);
    }

}
