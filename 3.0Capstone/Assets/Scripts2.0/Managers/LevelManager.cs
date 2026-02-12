using System;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    private bool isLoading = false;
    private Action onSceneLoadedCallback;

    [SerializeField] private float fadeTime = 2.0f;
    [SerializeField] private GameObject fadeOut;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnEnable()
    {
        // Subscribe to scene loaded event to reset the loading flag
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset the loading flag whenever a scene finishes loading
        isLoading = false;
        if (GameManager.Instance != null) GameManager.Instance.ResumeGameTime();
        
        // Invoke callback if set
        onSceneLoadedCallback?.Invoke();
        onSceneLoadedCallback = null; // Clear after invoking
    }

    public void LoadScene(int index, Action onLoadComplete = null)
    {
        if (isLoading) return;
        onSceneLoadedCallback = onLoadComplete;
        StartCoroutine(LoadSceneAsync(index));
    }

    private IEnumerator LoadSceneAsync(int index)
    {
        isLoading = true;

        AsyncOperation operation = SceneManager.LoadSceneAsync(index);
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.5f);

        operation.allowSceneActivation = true;

        while (!operation.isDone)
        {
            yield return null;
        }
    }

    public void StartWindDownLevel(bool goNext)
    {
        StartCoroutine(WindDownRoutine(goNext));
    }

    private IEnumerator WindDownRoutine(bool goNext)
    {
        GameObject[] obs = (GameObject[])FindObjectsByType(typeof(GameObject), FindObjectsSortMode.None);

        // Fade to black
        yield return StartCoroutine(FadeImage(1f));

        // Disable all objects except GameManager and MainCamera
        foreach (GameObject go in obs)
        {
            if (go.CompareTag("GameManager") || go.CompareTag("MainCamera")) continue;
            go.SetActive(false);
        }

        if (goNext)
        {
            SoundManager.Instance.PlayBGM("Navigation");
            ShowEndScreen(GameManager.Instance.NavigationScreenIndex);
        }
        else
        {
            if (GameManager.Instance.GameOverStatus)
            {
                SoundManager.Instance.PlayBGM("LoseTheme");
            }
            else
            {
                SoundManager.Instance.PlayBGM("WinTheme");
            }
            ShowEndScreen(GameManager.Instance.ResultScreenIndex);
        }

        // Fade back in
        yield return StartCoroutine(FadeImage(0f));
    }

    private IEnumerator FadeImage(float targetAlpha)
    {
        // Make sure fadeOut is active
        if (!fadeOut.activeSelf)
            fadeOut.SetActive(true);

        SpriteRenderer renderer = fadeOut.GetComponent<SpriteRenderer>();

        float startAlpha = renderer.color.a;
        float elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.unscaledDeltaTime; // Use unscaledDeltaTime in case time is paused
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeTime);
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, newAlpha);
            yield return null;
        }

        // Ensure we reach the exact target value
        renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, targetAlpha);
    }

    public void ShowEndScreen(int screenSceneIndex)
    {
        if (isLoading) return;
        StartCoroutine(LoadAdditiveScene(screenSceneIndex));
    }

    private IEnumerator LoadAdditiveScene(int index)
    {
        isLoading = true;

        AsyncOperation operation = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);

        while (!operation.isDone)
            yield return null;

        isLoading = false;
    }

    public void HideEndScreen(int screenSceneIndex)
    {
        StartCoroutine(UnloadAdditiveScene(screenSceneIndex));
    }

    private IEnumerator UnloadAdditiveScene(int index)
    {
        AsyncOperation operation = SceneManager.UnloadSceneAsync(index);

        while (!operation.isDone)
            yield return null;
    }
}