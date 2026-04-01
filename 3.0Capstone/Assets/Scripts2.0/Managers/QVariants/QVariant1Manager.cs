using UnityEngine;

public class QVariant1Manager : MonoBehaviour
{
    private bool buttonPressed = false;
    private AudioSource audioSource;
    [SerializeField] private ParticleSystem crystalShards;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    //public void Option1Press()
    //{
    //    if (buttonPressed) return;
    //
    //    Debug.Log("LEFT BUTTON PRESSED");
    //    audioSource.Play();
    //    crystalShards.Play();
    //    buttonPressed = true;
    //    GameManager.Instance.AddShards(50);
    //    GameflowManager flow = FindFirstObjectByType<GameflowManager>();
    //    flow.LevelRunning = false;
    //    GameManager.Instance.Victory();
    //}

    public void Option2Press()
    {
        Debug.Log("RIGHT BUTTON PRESSED");
        if (buttonPressed) return;

        buttonPressed = true;
        GameManager.Instance.cameFromQVariant = true;
        GameflowManager flow = FindFirstObjectByType<GameflowManager>();
        flow.LevelRunning = false;
        flow.StartScrollerLevel();
    }

}
