using UnityEngine;

public enum GemSize { Small, Medium, Large }

[CreateAssetMenu(menuName = "Appraisal/Gem Type")]
public class GemType : ScriptableObject
{
    public string displayName;
    public int baseValue = 100; // for shards
}