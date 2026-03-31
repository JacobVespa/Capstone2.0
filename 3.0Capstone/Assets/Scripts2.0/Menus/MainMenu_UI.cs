using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//using UnityEngine.UI;

public class MainMenu_UI : MonoBehaviour
{
    [SerializeField] private Animator menuAnimator;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject controlsMenu;
    [SerializeField] private GameObject creditsMenu;
    [SerializeField] private GameObject startButton;

    [SerializeField] SoundManager soundManager;

    bool hasStarted = false;

    private void Start()
    {
        menuAnimator.SetTrigger("OpenMenu");
    }

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null && !hasStarted)
        {
            EventSystem.current.SetSelectedGameObject(startButton);
        }
    }

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(null);
        //EventSystem.current.SetSelectedGameObject(startButton);
        Animator startButtonAnimator = startButton.GetComponentInChildren<Animator>();

        startButtonAnimator.SetTrigger("Highlighted");
    }

    public void StartButtonClicked()
    {
        //GameflowManager gameflowManager = FindFirstObjectByType<GameflowManager>();
        if (LevelManager.Instance != null)
        {
            //gameflowManager.StartScrollerLevel();
            //LevelManager.Instance.StartWindDownLevel(true);
            menuAnimator.SetTrigger("CloseMenu");
            hasStarted = true;
        }
        else
        {
            Debug.LogError("GameflowManager not found in the scene.");
        }
    }

    public void TutorialButtonClicked()
    {
        GameflowManager gameflowManager = FindFirstObjectByType<GameflowManager>();
        if (gameflowManager != null)
        {
            gameflowManager.StartTutorialLevel();
        }
        else
        {
            Debug.LogError("GameflowManager not found in the scene.");
        }
    }

    public void QuestionButtonClicked()
    {
        GameflowManager gameflowManager = FindFirstObjectByType<GameflowManager>();
        if(gameflowManager != null)
        {
            gameflowManager.StartQVariant1Level();
        }
        else
        {
            Debug.LogError("GameflowManager not found in the scene.");
        }
    }

    public void OptionButtonClicked()
    {
        mainMenu.SetActive(false);
        controlsMenu.SetActive(false);
        creditsMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void ControlButtonClicked()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(false);
        creditsMenu.SetActive(false);
        controlsMenu.SetActive(true);
    }

    public void CreditsButtonClicked()
    {
        optionsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        mainMenu.SetActive(false);
        creditsMenu.SetActive(true);
    }
    public void BackButtonClicked()
    {
        optionsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        creditsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void QuitButtonClicked()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        
        LevelManager.Instance.StartWindDownLevel(true);
        
    }
}
