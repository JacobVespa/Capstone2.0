using UnityEngine;

public enum UpgradeCategory { Stat, Attribute, Consumable }
public enum UpgradeQuality { Q1 = 1, Q2 = 2, Q3 = 3 }

[CreateAssetMenu(menuName = "Appraisal/Upgrade Definition")]
public class UpgradeDefinition : ScriptableObject
{
    public string displayName;
    public UpgradeCategory category;
    public UpgradeQuality baseQuality = UpgradeQuality.Q1;
    [TextArea] public string description;

    // Parameters for the effect – can be extended later
    public float magnitude = 1f;
}