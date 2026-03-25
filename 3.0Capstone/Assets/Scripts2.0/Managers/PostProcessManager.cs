using UnityEngine;

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

    public void SpeedUp()
    {

    }

}
