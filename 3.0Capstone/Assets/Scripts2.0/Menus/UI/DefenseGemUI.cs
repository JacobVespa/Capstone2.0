using System.Collections;
using UnityEngine;

public class DefenseGemUI : MonoBehaviour
{
    [SerializeField] private GameObject[] defenseGems;
    [SerializeField] private float popScale = 1.5f;
    [SerializeField] private float popDuration = 0.2f;
    [SerializeField] private float shrinkDuration = 0.15f;

    private int currentGems = 0;

    private void Start()
    {
        foreach (var gem in defenseGems)
            gem.SetActive(false);

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
            AnimateGem(defenseGems[i]);
        }
    }

    private void AnimateGem(GameObject gem)
    {
        StartCoroutine(GemAnimation(gem));
    }

    private IEnumerator GemAnimation(GameObject gem)
    {
        Vector3 originalScale = gem.transform.localScale;
        Vector3 bigScale = originalScale * popScale;

        // Scale up
        float elapsed = 0f;
        while (elapsed < popDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / popDuration);
            gem.transform.localScale = Vector3.Lerp(originalScale, bigScale, t);
            yield return null;
        }

        // Scale back down
        elapsed = 0f;
        while (elapsed < shrinkDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / shrinkDuration);
            gem.transform.localScale = Vector3.Lerp(bigScale, originalScale, t);
            yield return null;
        }

        gem.transform.localScale = originalScale;
    }
}