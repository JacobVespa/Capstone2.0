using UnityEngine;
using UnityEngine.EventSystems;

public class OptionsMenu_UI : MonoBehaviour
{
    [SerializeField] private GameObject optionsTab;
    [SerializeField] private GameObject controlsTab;
    [SerializeField] private GameObject optionsTabButton;
    [SerializeField] private GameObject controlsTabButton;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(optionsTabButton);
    }

    public void OptionsTabClicked()
    {
        optionsTab.SetActive(true);
        controlsTab.SetActive(false);
    }

    public void ControlsTabClicked()
    {
        controlsTab.SetActive(true);
        optionsTab.SetActive(false);
    }
}
