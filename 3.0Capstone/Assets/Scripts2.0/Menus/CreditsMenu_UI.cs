using UnityEngine;
using UnityEngine.EventSystems;

public class CreditsMenu_UI : MonoBehaviour
{
    [SerializeField] private GameObject backButton;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(backButton);
    }
}
