using UnityEngine;

public class OptionsMenu_UI : MonoBehaviour
{
    [SerializeField] private GameObject optionsTab;
    [SerializeField] private GameObject controlsTab;

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
