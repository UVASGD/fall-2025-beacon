using System.Collections.Generic;
using UnityEngine;

public static class RelicButtonAdapter
{
    public static UIButtonSpec ToSpec(
        RelicInstance relic,
        System.Action<RelicInstance> onChoose,
        Sprite iconOverride = null)
    {
        var spec = UIButtonSpecDefaults.Make(relic.displayName, () => onChoose?.Invoke(relic));
        spec.subtitle = $"{relic.rarity} • {relic.classId}";
        spec.backgroundColor = RelicUIColor.ColorFor(relic.rarity);
        spec.icon = iconOverride ?? relic.icon;
        spec.tooltipRichText = RelicUI.BuildDescription(relic, debug: false);
        spec.debugId = relic.seedHash;
        return spec;
    }

    public static List<UIButtonSpec> ToSpecs(IEnumerable<RelicInstance> relics, System.Action<RelicInstance> onChoose)
    {
        var list = new List<UIButtonSpec>();
        foreach (var r in relics)
            list.Add(ToSpec(r, onChoose));
        return list;
    }
}
