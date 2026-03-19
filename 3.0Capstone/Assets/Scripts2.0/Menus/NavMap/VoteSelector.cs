using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class VoteSelector : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
    // Called by gamepad/keyboard navigation
    public void OnSelect(BaseEventData eventData)
    {
        int playerIndex = GetPlayerIndex(eventData);
        if (playerIndex < 0) return;

        if (CaveMapInputManager.Instance != null)
            CaveMapInputManager.Instance.SetPlayerSelection(playerIndex, gameObject);
    }

    // Called when the mouse hovers over this button
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CaveMapInputManager.Instance == null) return;

        // Don't move the cursor to disabled/inaccessible buttons
        Button button = GetComponent<Button>();
        if (button == null || !button.interactable) return;

        int keyboardPlayerIndex = CaveMapInputManager.Instance.KeyboardPlayerIndex;
        if (keyboardPlayerIndex < 0) return;

        CaveMapInputManager.Instance.SetPlayerSelection(keyboardPlayerIndex, gameObject);
    }

    private int GetPlayerIndex(BaseEventData eventData)
    {
        if (eventData.currentInputModule is not InputSystemUIInputModule uiModule)
            return -1;

        MultiplayerEventSystem mes = uiModule.GetComponent<MultiplayerEventSystem>();
        if (mes == null) return -1;

        if (mes.playerRoot != null)
        {
            PlayerInput pi = mes.playerRoot.GetComponent<PlayerInput>();
            if (pi != null) return pi.playerIndex;
        }

        string esName = mes.gameObject.name;
        if (esName.EndsWith("_P1")) return 0;
        if (esName.EndsWith("_P2")) return 1;

        return -1;
    }
}