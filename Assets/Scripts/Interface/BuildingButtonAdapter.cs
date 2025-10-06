using System.Collections.Generic;
using UnityEngine;

public static class BuildingButtonAdapter
{
    public static UIButtonSpec ToSpec(
        Building b,
        System.Action<Building> onChoose,
        bool canAfford,
        Sprite icon = null)
    {
        var spec = UIButtonSpecDefaults.Make(b.name, () => onChoose?.Invoke(b));
        spec.subtitle = b.moneyCost > 0 ? $"Cost: {b.moneyCost}" : "";
        spec.badgeText = b.moneyCost > 0 ? $"${b.moneyCost}" : "";
        spec.badgeColor = canAfford ? new Color32(33, 150, 243, 255) : new Color32(211, 47, 47, 255);
        spec.interactable = canAfford;
        spec.icon = icon;
        spec.backgroundColor = canAfford ? Color.white : new Color(1f, 0.95f, 0.95f, 1f);
        spec.tooltipRichText = $"<b>{b.name}</b>\nBuild cost: {b.moneyCost}";
        return spec;
    }

    public static List<UIButtonSpec> ToSpecs(IEnumerable<Building> buildings, System.Func<Building, bool> canAfford, System.Action<Building> onChoose)
    {
        var list = new List<UIButtonSpec>();
        foreach (var b in buildings)
            list.Add(ToSpec(b, onChoose, canAfford?.Invoke(b) ?? true));
        return list;
    }
}
