using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Rendering;

public class PowerUpButton : MonoBehaviour
{
    [SerializeField] private SpriteRenderer buttonOutline;
    [SerializeField] private SpriteRenderer button;
    [SerializeField] private CircleCollider2D circleCollider;
    [SerializeField] private SpriteRenderer exclamationOutline;
    [SerializeField] private SpriteRenderer exclamation;

    private void Awake()
    {
        GameManager.Instance.powerUp = this;
    }
    public void EnableButton()
    {
        Debug.Log("TURN THE STUPID BUTTON ON");
        buttonOutline.enabled = true;
        button.enabled = true;
        circleCollider.enabled = true;
        exclamationOutline.enabled = true;
        exclamation.enabled = true;
    }

    public void DisableButton()
    {
        buttonOutline.enabled = false;
        button.enabled = false;
        circleCollider.enabled = false;
        exclamationOutline.enabled = false;
        exclamation.enabled = false;
    }

}
