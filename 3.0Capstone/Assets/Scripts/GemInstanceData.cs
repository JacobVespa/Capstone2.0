using UnityEngine;

public class GemInstanceData
{
    public GemType type;
    public GemSize size;

    public int GetShardValue()
    {
        float multiplier;

        switch (size)
        {
            case GemSize.Small:
                multiplier = 0.5f;
                break;

            case GemSize.Medium:
                multiplier = 1.0f;
                break;

            case GemSize.Large:
                multiplier = 1.5f;
                break;

            default:
                multiplier = 1.0f;
                break;
        }

        return Mathf.RoundToInt(type.baseValue * multiplier);
    }
}