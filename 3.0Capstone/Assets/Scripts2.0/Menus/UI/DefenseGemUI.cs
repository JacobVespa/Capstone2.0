using UnityEngine;

public class DefenseGemUI : MonoBehaviour
{
    [SerializeField] private GameObject[] defenseGems;
    private int currentGems = 0;

    private void Start()
    {
        foreach (var gem in defenseGems)
        {
            gem.SetActive(false);
        }

        UpdateGemCount();
    }

    public void UpdateGemCount()
    {
        if (GameManager.Instance != null)
            currentGems = GameManager.Instance.Gems;

        if (currentGems > defenseGems.Length) return;

        for (int i = 0; i < currentGems; i++)
        {
            defenseGems[i].SetActive(true);
        }
    }

    // TODO set the gems initial scale to larger than needed,
    // Then use coroutine so scale back to normal to create a pop-out effect
}
