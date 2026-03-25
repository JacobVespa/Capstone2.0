using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private GameObject selectedState;
    [SerializeField] private GameObject deselectState;

    public void OnSelect(BaseEventData eventData)
    {
        selectedState.SetActive(true);
        deselectState.SetActive(false);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        selectedState.SetActive(false);
        deselectState.SetActive(true);
    }
}
