using UnityEngine;

public class AppraisalStation : MonoBehaviour, IInteractable
{
    
    [SerializeField] Canvas appraisalCanvas;
    [SerializeField] private bool appraising = false;

    public void Interact(PlayerController interactor)
    {
        Debug.Log("Appraisal Station Interacted With");

        if (!appraising)
        {
            Enter();
        }
        else
        {
            Exit();
        }

    }

    private void Enter()
    {
        appraisalCanvas.enabled = true;
        appraising = true;
    }

    private void Exit()
    {
        appraisalCanvas.enabled = false;
        appraising = false;
    }

}
