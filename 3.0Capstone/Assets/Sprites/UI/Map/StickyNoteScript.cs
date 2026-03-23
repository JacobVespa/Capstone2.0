using UnityEngine;
using UnityEngine.UI;

public class StickyNoteScript : MonoBehaviour
{
    [SerializeField] private Sprite[] stickynoteOptions;
    private int selectedOption;
    [SerializeField] private Image stickynoteBG;
    [SerializeField] private Image stickynoteShadow;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        selectedOption = Random.Range(0, stickynoteOptions.Length);
        stickynoteBG.sprite = stickynoteOptions[selectedOption];
        stickynoteShadow.sprite = stickynoteOptions[selectedOption];

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
