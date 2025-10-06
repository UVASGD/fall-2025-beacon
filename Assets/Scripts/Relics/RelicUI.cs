using System.Text;
using UnityEngine;

public static class RelicUI
{
    // Classic: Common=grey, Uncommon=green, Rare=blue, Epic(Mythic)=purple, Legendary=orange
    public static Color GetRarityColor(RelicRarity r)
    {
        switch (r)
        {
            case RelicRarity.Common: return new Color32(128, 128, 128, 255); // gray
            case RelicRarity.Uncommon: return new Color32(67, 160, 71, 255); // green-ish
            case RelicRarity.Rare: return new Color32(30, 136, 229, 255); // blue
            case RelicRarity.Epic: return new Color32(171, 71, 188, 255); // purple (mythic)
            case RelicRarity.Legendary: return new Color32(255, 143, 0, 255); // orange/amber
            default: return Color.white;
        }
    }

    public static string BuildDescription(RelicInstance r, bool debug = false)
    {
        var sb = new StringBuilder();

        // Title
        sb.AppendLine($"<b>{r.displayName}</b>");
        sb.AppendLine($"{r.rarity} • {r.classId}");
        if (!string.IsNullOrEmpty(r.legendaryId))
            sb.AppendLine($"* {r.legendaryId}");
        sb.AppendLine();

        // Effects
        if (r.resolvedModifiers != null && r.resolvedModifiers.Count > 0)
        {
            sb.AppendLine("<u>Effects</u>");
            foreach (var m in r.resolvedModifiers)
            {
                string v = (Mathf.Abs(m.mul) > 0.0001f)
                    ? $"{m.mul * 100f:+0.#;-0.#}%"
                    : $"{m.add:+0.##;-0.##}";
                sb.AppendLine($"• {m.stat}: {v}");
            }
        }

        // Optional debug
        if (debug)
        {
            sb.AppendLine();
            sb.AppendLine("<u>Debug</u>");
            sb.AppendLine($"Seed: {r.seedHash}");
            var imps = r.implicitIds != null ? string.Join(", ", r.implicitIds) : "(none)";
            var affs = r.affixIds != null ? string.Join(", ", r.affixIds) : "(none)";
            sb.AppendLine($"Imp: {imps}");
            sb.AppendLine($"Aff: {affs}");
            if (!string.IsNullOrEmpty(r.legendaryId)) sb.AppendLine($"Leg: {r.legendaryId}");
        }

        return sb.ToString();
    }
}
