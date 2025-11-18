using UnityEngine;

public static class AppraisalService
{
    // Decide how many shards you get when dismantling.
    public static int GetShardPayout(int baseValue)
    {
        // Placeholder: 50% of value, or override with design values.
        return Mathf.Max(0, Mathf.RoundToInt(baseValue * 0.5f));
    }

    // Decide quality based on gem size.
    public static UpgradeQuality RollQuality(GemSize size)
    {
        float randomValue = Random.value;

        switch (size)
        {
            case GemSize.Small:
                return (randomValue < 0.8f) ? UpgradeQuality.Q1 : UpgradeQuality.Q2;

            case GemSize.Medium:
                if (randomValue < 0.4f) return UpgradeQuality.Q1;
                if (randomValue < 0.9f) return UpgradeQuality.Q2;
                return UpgradeQuality.Q3;

            case GemSize.Large:
                if (randomValue < 0.1f) return UpgradeQuality.Q1;
                if (randomValue < 0.7f) return UpgradeQuality.Q2;
                return UpgradeQuality.Q3;

            default:
                return UpgradeQuality.Q1;
        }
    }
}