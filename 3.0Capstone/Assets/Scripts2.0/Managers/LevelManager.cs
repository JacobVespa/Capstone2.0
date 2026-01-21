using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    private bool isLoading = false;

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
        GameManager.Instance.ResumeGameTime();
    }

    public void LoadScene(int index)
    {
        if (isLoading) return;
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

    public void StartWindDownLevel(Level level)
    {
        StartCoroutine(WindDownRoutine(level));
    }

    private IEnumerator WindDownRoutine(Level level)
    {
        GameObject[] obs = (GameObject[])FindObjectsByType(typeof(GameObject), FindObjectsSortMode.None);

        //FadeImage(1f);
        yield return new WaitForSecondsRealtime(fadeTime + 0.1f);

        foreach (GameObject go in obs)
        {
            if (go.CompareTag("GameManager") || go.CompareTag("MainCamera")) continue;

            go.SetActive(false);

        }

        //FadeImage(0f);
        yield return new WaitForSecondsRealtime(fadeTime + 0.1f);
    }

    //private IEnumerator FadeImage(float target)
    //{
        //SpriteRenderer renderer = fadeOut.transform.GetComponent<SpriteRenderer>();
        //float startValue = renderer.color.a;

        //float elapsedTime = 0f;

        //while (elapsedTime < fadeTime)
        //{
            //elapsedTime += Time.deltaTime;
            //float newAlpha = Mathf.Lerp(startValue, target, elapsedTime/fadeTime);
            //renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, newAlpha);
            //yield return null;
        //}
    //}

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