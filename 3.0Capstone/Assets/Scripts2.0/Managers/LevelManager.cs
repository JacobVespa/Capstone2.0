using System;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Xml.Serialization;

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
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isLoading = false;
        if (GameManager.Instance != null) GameManager.Instance.ResumeGameTime();

        onSceneLoadedCallback?.Invoke();
        onSceneLoadedCallback = null;
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
            yield return null;

        yield return new WaitForSecondsRealtime(0.5f);

        operation.allowSceneActivation = true;

        while (!operation.isDone)
            yield return null;
    }

    public void StartWindDownLevel(bool goNext)
    {
        StartCoroutine(WindDownRoutine(goNext));
    }

    private IEnumerator WindDownRoutine(bool goNext)
    {
        GameObject[] obs = (GameObject[])FindObjectsByType(typeof(GameObject), FindObjectsSortMode.None);

        // Disable enemies and spawners immediately so they don't interfere
        foreach (GameObject go in obs)
        {
            if (go.CompareTag("Spawner"))
                go.SetActive(false);

            if (go.CompareTag("Enemy"))  
            {
                EnemyBody enemy = go.GetComponent<EnemyBody>();
                enemy.InstantKill();
            }
        }

        // Pan camera BEFORE stopping time — PanOver uses Time.deltaTime
        // so Time.timeScale must still be 1 during the pan
        CameraCinematic cam = GameObject.FindFirstObjectByType<CameraCinematic>();
        if (cam != null && goNext)
        {
            cam.PanOver(20f, 0.3f, new Vector3(0, -60, 0));
            yield return new WaitForSecondsRealtime(4f); // match PanOver duration
        }

        GameManager.Instance.EmptyStorage();

        if (goNext)
        {
            SoundManager.Instance.PlayBGM("Navigation");
            ShowEndScreen(GameManager.Instance.NavigationScreenIndex);
        }
        else
        {
            if (GameManager.Instance.GameOverStatus)
                SoundManager.Instance.PlayBGM("LoseTheme");

            ShowEndScreen(GameManager.Instance.ResultScreenIndex);
        }

        DisableLevelObjects(obs);
    }

    private void DisableLevelObjects(GameObject[] obs)
    {
        foreach (GameObject go in obs)
        {
            if (go == null) continue; // Skip destroyed objects

            if (!go.CompareTag("GameManager") && !go.CompareTag("MainCamera"))
            {
                go.SetActive(false);
            }
        }
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