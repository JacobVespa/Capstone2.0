using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private string[] sceneName;

    public GameObject loadingScreen;
    public UnityEngine.UI.Slider progressBar;

    public static LevelManager Instance { get; private set; }

    private Level currentLevel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scene loads
        }
    }

    public void LoadScene(int index)
    {
        if (index < 0 || index >= sceneName.Length) index = 0;

        StartCoroutine(LoadSceneAsync(index));
    }

    private IEnumerator LoadSceneAsync(int index)
    {
        if (loadingScreen != null)
            loadingScreen.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName[index]);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            // Progress goes from 0 to 0.9 before activation
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (progressBar != null)
                progressBar.value = progress;

            // When loading is done, activate scene
            if (operation.progress >= 0.9f)
            {
                // Small delay
                yield return new WaitForSeconds(0.5f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    
    }

    public void StartScrollerLevel()
    {
        currentLevel = new ScrollerLevel(1);
        currentLevel.StartLevel();
    }

    public void RestartLevel()
    {
        currentLevel.RestartLevel();
    }

    public void EndLevel()
    {
        currentLevel.EndLevel();
    }

}
