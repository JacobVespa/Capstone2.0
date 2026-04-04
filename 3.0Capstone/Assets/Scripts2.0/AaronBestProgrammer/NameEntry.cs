using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NameEntry : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int maxNameLength = 4;
    [SerializeField] private float inputRepeatDelay = 0.35f;
    [SerializeField] private float navigationDelay = 0.15f;
    [SerializeField] private float stickThreshold = 0.5f;

    [Header("UI")]
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Transform keyboardGrid;
    [SerializeField] private GameObject keyButtonPrefab;

    [Header("Key Styling")]
    [SerializeField] private Color normalColor = new Color(0.2f, 0.2f, 0.2f);
    [SerializeField] private Color highlightColor = new Color(0.9f, 0.6f, 0.1f);
    [SerializeField] private Color normalTextColor = Color.white;
    [SerializeField] private Color highlightTextColor = Color.black;

    [Header("Events")]
    public UnityEvent<string> onNameConfirmed;

    private static readonly string[] KeyboardRows = new string[]
    {
        "A B C D E F G H I",
        "J K L M N O P Q R",
        "S T U V W X Y Z 0",
        "1 2 3 4 5 6 7 8 9",
        "DEL END"
    };

    private string[][] keys;
    private int cursorRow = 0;
    private int cursorCol = 0;
    private StringBuilder currentName = new StringBuilder();

    private Coroutine navCoroutine;
    private Vector2Int lastDir = Vector2Int.zero;
    private bool isActive = false;
    private bool keyboardMode = false;

    private List<List<KeyButton>> keyButtons = new();

    private struct KeyButton
    {
        public Image background;
        public TMP_Text label;
    }

    public string CurrentName => currentName.ToString();

    private void Awake()
    {
        BuildKeyArray();
        BuildKeyboardUI();

        if (nameInputField != null)
        {
            nameInputField.characterLimit = maxNameLength;
            nameInputField.onValueChanged.AddListener(OnInputFieldChanged);
        }
    }

    private void OnEnable()
    {
        currentName.Clear();
        if (nameInputField != null) currentName.Append(nameInputField.text);

        cursorRow = 0;
        cursorCol = 0;
        isActive = true;

        SyncInputField();
        ClearMessage();
        HighlightCursor();
    }

    private void OnDisable()
    {
        isActive = false;
        StopNav();
    }

    private void BuildKeyArray()
    {
        keys = new string[KeyboardRows.Length][];
        for (int r = 0; r < KeyboardRows.Length; r++)
            keys[r] = KeyboardRows[r].Split(' ');
    }

    private void BuildKeyboardUI()
    {
        if (keyboardGrid == null || keyButtonPrefab == null) return;

        foreach (Transform child in keyboardGrid)
            Destroy(child.gameObject);

        keyButtons.Clear();

        VerticalLayoutGroup vlg = keyboardGrid.GetComponent<VerticalLayoutGroup>();
        if (vlg == null) vlg = keyboardGrid.gameObject.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 4;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = true;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childAlignment = TextAnchor.UpperCenter;

        for (int r = 0; r < keys.Length; r++)
        {
            GameObject rowObj = new GameObject($"Row_{r}", typeof(RectTransform));
            rowObj.transform.SetParent(keyboardGrid, false);

            LayoutElement rowLayout = rowObj.AddComponent<LayoutElement>();
            rowLayout.flexibleWidth = 1;
            rowLayout.flexibleHeight = 1;

            HorizontalLayoutGroup hlg = rowObj.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 4;
            hlg.childForceExpandWidth = true;
            hlg.childForceExpandHeight = true;
            hlg.childControlWidth = true;
            hlg.childControlHeight = true;
            hlg.childAlignment = TextAnchor.MiddleCenter;

            List<KeyButton> rowList = new();

            for (int c = 0; c < keys[r].Length; c++)
            {
                string key = keys[r][c];
                GameObject obj = Instantiate(keyButtonPrefab, rowObj.transform, false);
                obj.name = $"Key_{key}";

                KeyButton kb = new KeyButton
                {
                    background = obj.GetComponent<Image>(),
                    label = obj.GetComponentInChildren<TMP_Text>()
                };

                kb.label.text = key;
                kb.background.color = normalColor;
                kb.label.color = normalTextColor;

                string capturedKey = key;
                obj.GetComponent<Button>().onClick.AddListener(() => OnKeyClicked(capturedKey));

                rowList.Add(kb);
            }

            keyButtons.Add(rowList);
        }

        HighlightCursor();
    }

    private void OnKeyClicked(string key)
    {
        if      (key == "DEL") Delete();
        else if (key == "END") Confirm();
        else                   TypeChar(key);
    }

    private void Update()
    {
        if (!isActive) return;

        var gamepad = Gamepad.current;
        var kb = Keyboard.current;

        if (!keyboardMode && kb != null && kb.anyKey.wasPressedThisFrame)
        {
            SetMode(true);
            return;
        }

        if (gamepad != null)
        {
            Vector2 stick = gamepad.leftStick.ReadValue();
            bool anyGamepadInput = gamepad.dpad.up.wasPressedThisFrame    ||
                                   gamepad.dpad.down.wasPressedThisFrame  ||
                                   gamepad.dpad.left.wasPressedThisFrame  ||
                                   gamepad.dpad.right.wasPressedThisFrame ||
                                   stick.magnitude > stickThreshold       ||
                                   gamepad.buttonSouth.wasPressedThisFrame;

            if (anyGamepadInput && keyboardMode) SetMode(false);
        }

        if (keyboardMode || gamepad == null) return;

        Vector2Int currentDir = Vector2Int.zero;
        Vector2 ls = gamepad.leftStick.ReadValue();

        if      (gamepad.dpad.up.isPressed    || ls.y >  stickThreshold) currentDir.y =  1;
        else if (gamepad.dpad.down.isPressed  || ls.y < -stickThreshold) currentDir.y = -1;

        if      (gamepad.dpad.right.isPressed || ls.x >  stickThreshold) currentDir.x =  1;
        else if (gamepad.dpad.left.isPressed  || ls.x < -stickThreshold) currentDir.x = -1;

        if (currentDir != Vector2Int.zero)
        {
            if (currentDir != lastDir)
            {
                lastDir = currentDir;
                StartNav(currentDir);
            }
        }
        else
        {
            lastDir = Vector2Int.zero;
            StopNav();
        }

        if (gamepad.buttonSouth.wasPressedThisFrame) PressCurrentKey();
        if (gamepad.buttonEast.wasPressedThisFrame)  Delete();
        if (gamepad.startButton.wasPressedThisFrame) Confirm();
    }

    private void SetMode(bool isKeyboard)
    {
        keyboardMode = isKeyboard;
        if (isKeyboard) nameInputField?.ActivateInputField();
        else            nameInputField?.DeactivateInputField();

        if (keyboardGrid != null)
            keyboardGrid.gameObject.SetActive(!isKeyboard);
    }

    private void OnInputFieldChanged(string value)
    {
        if (!keyboardMode) return;
        currentName.Clear();
        currentName.Append(value.ToUpper());
        SyncInputField();
    }

    private void SyncInputField()
    {
        if (nameInputField != null && nameInputField.text != CurrentName)
            nameInputField.SetTextWithoutNotify(CurrentName);
    }

    private void ShowMessage(string message)
    {
        if (messageText != null)
            messageText.text = message;
    }

    private void ClearMessage()
    {
        if (messageText != null)
            messageText.text = string.Empty;
    }

    private void MoveCursor(Vector2Int direction)
    {
        cursorRow = Mathf.Clamp(cursorRow - direction.y, 0, keys.Length - 1);
        cursorCol = Mathf.Clamp(cursorCol + direction.x, 0, keys[cursorRow].Length - 1);
        HighlightCursor();
    }

    private void HighlightCursor()
    {
        if (keyButtons.Count == 0) return;

        for (int r = 0; r < keyButtons.Count; r++)
        {
            for (int c = 0; c < keyButtons[r].Count; c++)
            {
                bool isSelected = r == cursorRow && c == cursorCol;
                keyButtons[r][c].background.color = isSelected ? highlightColor : normalColor;
                keyButtons[r][c].label.color      = isSelected ? highlightTextColor : normalTextColor;
            }
        }
    }

    private void PressCurrentKey()
    {
        string key = keys[cursorRow][cursorCol];
        if      (key == "DEL") Delete();
        else if (key == "END") Confirm();
        else                   TypeChar(key);
    }

    private void TypeChar(string character)
    {
        if (currentName.Length >= maxNameLength) return;
        currentName.Append(character);
        SyncInputField();
        ClearMessage();
    }

    private void Delete()
    {
        if (currentName.Length == 0) return;
        currentName.Remove(currentName.Length - 1, 1);
        SyncInputField();
    }

    private void Confirm()
    {
        if (currentName.Length == 0)
        {
            ShowMessage("Don't leave empty!");
            return;
        }

        isActive = false;
        StopNav();
        ClearMessage();
        onNameConfirmed?.Invoke(CurrentName);
        gameObject.SetActive(false);
    }

    private void StartNav(Vector2Int direction)
    {
        StopNav();
        navCoroutine = StartCoroutine(NavRepeat(direction));
    }

    private void StopNav()
    {
        if (navCoroutine != null) StopCoroutine(navCoroutine);
        navCoroutine = null;
    }

    private IEnumerator NavRepeat(Vector2Int direction)
    {
        MoveCursor(direction);
        yield return new WaitForSeconds(inputRepeatDelay);
        while (true)
        {
            MoveCursor(direction);
            yield return new WaitForSeconds(navigationDelay);
        }
    }
}