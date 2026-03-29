using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    //[SerializeField] private GameObject selectedState;
    //[SerializeField] private GameObject deselectState;


    [SerializeField] private Animator buttonAnimator;

    public void OnSelect(BaseEventData eventData)
    {
        //selectedState.SetActive(true);
        //deselectState.SetActive(false);

        buttonAnimator.SetTrigger("Highlighted");
        buttonAnimator.ResetTrigger("Dehighlight");
    }

    public void OnDeselect(BaseEventData eventData)
    {
        buttonAnimator.SetTrigger("Dehighlight");
        buttonAnimator.ResetTrigger("Highlighted");
        //selectedState.SetActive(false);
        //deselectState.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonAnimator.SetTrigger("Highlighted");
        buttonAnimator.ResetTrigger("Dehighlight");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonAnimator.SetTrigger("Dehighlight");
        buttonAnimator.ResetTrigger("Highlighted");
    }
}
