using UnityEngine;
using UnityEngine.EventSystems;

public class NavigationMenu_UI : MonoBehaviour
{
    private GameObject levelButton;

    public CaveMap caveMap;

    private void OnEnable()
    {
        //caveMap = GetComponent<CaveMap>();
        //levelButton = caveMap.activeRow[0].gameObject;

        //if (caveMap != null)
        //{
        //    Debug.Log("OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO");
        //    if (caveMap.activeRow.Count > 0)
        //    {
        //        EventSystem.current.SetSelectedGameObject(null);
        //        EventSystem.current.SetSelectedGameObject(levelButton);
        //    }
        //}
    }

    public void OnContinueButton()
    {
        GameflowManager gameflowManager = FindFirstObjectByType<GameflowManager>();
        if (gameflowManager != null)
        {
            gameflowManager.StartDefenseLevel();
            LevelManager.Instance.HideEndScreen(GameManager.Instance.NavigationScreenIndex);
        }
    }
}
