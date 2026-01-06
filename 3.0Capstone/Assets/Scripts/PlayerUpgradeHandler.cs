using UnityEngine;

public class PlayerUpgradeHandler : MonoBehaviour
{
    [Header("Debug view")]
    public float moveSpeedMultiplier = 1f;
    public float rateOfFireMultiplier = 1f;

    public void ApplyUpgrade(UpgradeDefinition upgrade)
    {
        switch (upgrade.category)
        {
            case UpgradeCategory.Stat:
                ApplyStatUpgrade(upgrade);
                break;
            case UpgradeCategory.Attribute:
                ApplyAttributeUpgrade(upgrade);
                break;
            case UpgradeCategory.Consumable:
                ApplyConsumableUpgrade(upgrade);
                break;
        }
    }

    void ApplyStatUpgrade(UpgradeDefinition upgrade)
    {
        // Example: treat "speed" upgrades as move speed buffs
        // In future this would talk to the actual player stats / weapons.
        moveSpeedMultiplier += upgrade.magnitude;
        Debug.Log($"Applied stat upgrade: {upgrade.displayName}, new speed mult = {moveSpeedMultiplier}");
    }

    void ApplyAttributeUpgrade(UpgradeDefinition upgrade)
    {
        // e.g., more inventory slots, more health, etc.
        Debug.Log($"Applied attribute upgrade: {upgrade.displayName}");
    }

    void ApplyConsumableUpgrade(UpgradeDefinition upgrade)
    {
        // e.g., one-time heal, shield, temporary buff
        Debug.Log($"Applied consumable upgrade: {upgrade.displayName}");
    }
}