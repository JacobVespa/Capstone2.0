using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScenes : MonoBehaviour
{
    [SerializeField] string sceneName;
    [SerializeField] int sceneIndex;
    [SerializeField] Collider collider;

    public void LoadSceneByName()
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadSceneByIndex()
    {
        SceneManager.LoadScene(sceneIndex);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Debug.Log("lol");
            LoadSceneByName();
        }
    }

    public void OnClick()
    {
        LoadSceneByName();
    }

}
