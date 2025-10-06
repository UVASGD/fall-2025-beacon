using UnityEngine;

public static class RelicUIColor
{
    public static Color ColorFor(RelicRarity r)
    {
        switch (r)
        {
            case RelicRarity.Common: return new Color32(128, 128, 128, 255);
            case RelicRarity.Uncommon: return new Color32(67, 160, 71, 255);
            case RelicRarity.Rare: return new Color32(30, 136, 229, 255);
            case RelicRarity.Epic: return new Color32(171, 71, 188, 255);
            case RelicRarity.Legendary: return new Color32(255, 143, 0, 255);
            default: return Color.white;
        }
    }
}
