using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Beacon.ProcGen;

public class RelicSmokeTest : MonoBehaviour
{
    [Header("Test params")]
    public int rolls = 10;
    public bool simulateCannonBuild = true;
    public bool simulateBoosterBuild = true;
    public bool simulateHangarBuild = true;

    RelicManager mgr;

    void Start()
    {
        mgr = FindObjectOfType<RelicManager>();
        if (mgr == null)
        {
            var go = new GameObject("RelicManager (Auto)");
            mgr = go.AddComponent<RelicManager>();
        }

        mgr.runId = 1;
        mgr.stageIndex = 0;
        mgr.itemLevel = 1;
        mgr.autoGatherBuildingTags = false; // we'll feed tags manually below
        mgr.extraRunTags = RelicTag.None;
        if (simulateCannonBuild) mgr.extraRunTags |= RelicTag.Projectile | RelicTag.Crit | RelicTag.FireRate;
        if (simulateBoosterBuild) mgr.extraRunTags |= RelicTag.Aura;
        if (simulateHangarBuild) mgr.extraRunTags |= RelicTag.Summon;

        // Build minimal content in memory
        BuildTestContent(mgr);

        // Roll a bunch and print
        var rarityCounts = new Dictionary<RelicRarity, int>();
        var classCounts = new Dictionary<RelicClass, int>();

        for (int i = 0; i < rolls; i++)
        {
            var offer = mgr.RollChoices();
            Debug.Log($"--- Offer #{i + 1} ---");
            for (int j = 0; j < offer.Count; j++)
            {
                var r = offer[j];
                rarityCounts[r.rarity] = rarityCounts.GetValueOrDefault(r.rarity) + 1;
                classCounts[r.classId] = classCounts.GetValueOrDefault(r.classId) + 1;

                Debug.Log(Pretty(r, prefix: $"[{j + 1}] "));
            }

            // Pretend we take the first relic each time and apply it (tests totals integration)
            if (offer.Count > 0)
            {
                mgr.Take(offer[0]);
            }
        }

        // Show aggregate stats
        Debug.Log("=== Aggregate ===");
        Debug.Log("Rarities: " + string.Join(", ", rarityCounts.Select(kv => $"{kv.Key}:{kv.Value}")));
        Debug.Log("Classes: " + string.Join(", ", classCounts.Select(kv => $"{kv.Key}:{kv.Value}")));

        // Show current totals applied
        Debug.Log("=== Totals (post-takes) ===");
        foreach (StatType st in Enum.GetValues(typeof(StatType)))
        {
            var add = mgr.GetAdd(st);
            var mul = mgr.GetMul(st);
            if (Mathf.Abs(add) > 0.0001f || Mathf.Abs(mul) > 0.0001f)
            {
                Debug.Log($"{st}: add={add:F3}, mul={mul:P1}");
            }
        }
    }

    static string Pretty(RelicInstance r, string prefix = "")
    {
        var lines = new List<string>();
        foreach (var m in r.resolvedModifiers)
        {
            var p = m.mul != 0 ? $"{m.mul * 100f:+0.#;-0.#}%" : $"{m.add:+0.##;-0.##}";
            lines.Add($"• {m.stat}: {p}");
        }
        var legend = string.IsNullOrEmpty(r.legendaryId) ? "" : $" | Legendary: {r.legendaryId}";
        return $"{prefix}{r.displayName} ({r.classId}) [{r.seedHash}]{legend}\n  " + string.Join("\n  ", lines);
    }

    // ---------- Minimal in-memory content ----------

    void BuildTestContent(RelicManager m)
    {
        // Clear any existing content
        m.classDefs.Clear();
        m.genericAffixes.Clear();
        m.legendaryDefs.Clear();

        // Generic affixes
        m.genericAffixes.Add(MakeAffix(
            id: "gen_dmg",
            name: "Global Damage %",
            tags: RelicTag.None,
            PercentLine(StatType.Damage, 0.04f, 0.10f)
        ));
        m.genericAffixes.Add(MakeAffix(
            id: "gen_aspd",
            name: "Global FireRate %",
            tags: RelicTag.None,
            PercentLine(StatType.FireRate, 0.05f, 0.12f)
        ));
        m.genericAffixes.Add(MakeAffix(
            id: "gen_proj",
            name: "Global ProjectileSpeed %",
            tags: RelicTag.None,
            PercentLine(StatType.ProjectileSpeed, 0.06f, 0.20f)
        ));

        // Cannon class
        var cannon = ScriptableObject.CreateInstance<RelicClassDef>();
        cannon.name = "Class_Cannon";
        cannon.classId = RelicClass.Cannon;
        cannon.providedTags = RelicTag.Projectile | RelicTag.Crit | RelicTag.FireRate;
        cannon.implicitAffixes = new List<RelicAffixDef>
        {
            MakeAffix("imp_can_dmg","Cannon Damage %", RelicTag.Projectile, PercentLine(StatType.Damage, 0.04f, 0.10f)),
            MakeAffix("imp_can_ps","Cannon ProjectileSpeed %", RelicTag.Projectile, PercentLine(StatType.ProjectileSpeed, 0.06f, 0.15f)),
        };
        cannon.affixPool = new List<RelicAffixDef>
        {
            MakeAffix("can_aspd","Cannon FireRate %", RelicTag.FireRate, PercentLine(StatType.FireRate, 0.06f, 0.15f)),
            MakeAffix("can_elite","Damage vs Elites %", RelicTag.Boss, PercentLine(StatType.Damage, 0.10f, 0.25f)),
        };
        m.classDefs.Add(cannon);

        // Booster class
        var booster = ScriptableObject.CreateInstance<RelicClassDef>();
        booster.name = "Class_Booster";
        booster.classId = RelicClass.Booster;
        booster.providedTags = RelicTag.Aura;
        booster.implicitAffixes = new List<RelicAffixDef>
        {
            MakeAffix("imp_boo_rng","Booster Aura Range %", RelicTag.Range | RelicTag.Aura, PercentLine(StatType.Range, 0.10f, 0.20f)),
            MakeAffix("imp_boo_dmg","Aura Grants Damage %", RelicTag.Aura, PercentLine(StatType.Damage, 0.04f, 0.08f))
        };
        booster.affixPool = new List<RelicAffixDef>
        {
            MakeAffix("boo_grant_aspd","Aura Grants FireRate %", RelicTag.Aura | RelicTag.FireRate, PercentLine(StatType.FireRate, 0.06f, 0.15f)),
            MakeAffix("boo_grant_proj","Aura Grants ProjectileSpeed %", RelicTag.Aura | RelicTag.Projectile, PercentLine(StatType.ProjectileSpeed, 0.10f, 0.22f))
        };
        m.classDefs.Add(booster);

        // Hangar class
        var hangar = ScriptableObject.CreateInstance<RelicClassDef>();
        hangar.name = "Class_Hangar";
        hangar.classId = RelicClass.Hangar;
        hangar.providedTags = RelicTag.Summon;
        hangar.implicitAffixes = new List<RelicAffixDef>
        {
            MakeAffix("imp_han_dmg","Fighter Damage %", RelicTag.Summon, PercentLine(StatType.Damage, 0.10f, 0.20f)),
        };
        hangar.affixPool = new List<RelicAffixDef>
        {
            MakeAffix("han_hp","Fighter HP %", RelicTag.Health | RelicTag.Summon, PercentLine(StatType.MaxHealth, 0.15f, 0.35f)),
            MakeAffix("han_aspd","Fighter Attack Speed %", RelicTag.Summon | RelicTag.FireRate, PercentLine(StatType.FireRate, 0.10f, 0.25f))
        };
        m.classDefs.Add(hangar);

        // Simple legendary examples (numbers only for v1)
        m.legendaryDefs.Add(MakeLegendary("leg_ricochet", "Ricochet Engine", PercentMod(StatType.ProjectileSpeed, 0.30f)));
        m.legendaryDefs.Add(MakeLegendary("leg_overtone", "Overtone Field", PercentMod(StatType.Damage, 0.25f)));
        m.legendaryDefs.Add(MakeLegendary("leg_wingcmd", "Wing Commander", PercentMod(StatType.FireRate, 0.20f)));
    }

    // --- tiny SO builders + helpers (these solve the 'Percent' error) ---

    static RelicAffixDef MakeAffix(string id, string name, RelicTag tags, RelicAffixDef.Line line)
    {
        var a = ScriptableObject.CreateInstance<RelicAffixDef>();
        a.id = id;
        a.displayName = name;
        a.tags = tags;
        a.lines = new List<RelicAffixDef.Line> { line };
        a.allowedClasses = new List<RelicClass> {
            RelicClass.Generic, RelicClass.Cannon, RelicClass.Booster, RelicClass.Hangar
        };

        // reasonable defaults for rarity scalars
        for (int i = 0; i < a.lines.Count; i++)
        {
            var l = a.lines[i];
            l.rarityScalarCommon = 1.00f;
            l.rarityScalarUncommon = 1.05f;
            l.rarityScalarRare = 1.12f;
            l.rarityScalarEpic = 1.20f;
            l.rarityScalarLegendary = 1.30f;
            a.lines[i] = l;
        }
        return a;
    }

    static RelicLegendaryDef MakeLegendary(string id, string name, RelicDefinition.Modifier flatMod)
    {
        var l = ScriptableObject.CreateInstance<RelicLegendaryDef>();
        l.id = id;
        l.displayName = name;
        l.flatModifiers = new List<RelicDefinition.Modifier> { flatMod };
        return l;
    }

    // Create a percent line (goes into Modifier.mul)
    static RelicAffixDef.Line PercentLine(StatType st, float min, float max)
        => new RelicAffixDef.Line
        {
            stat = st,
            min = min,
            max = max,
            isPercent = true
        };

    // Create a percent modifier (for the simple legendaries)
    static RelicDefinition.Modifier PercentMod(StatType st, float value)
        => new RelicDefinition.Modifier
        {
            stat = st,
            add = 0f,
            mul = value
        };
}

// Simple dictionary helper
static class DictExt
{
    public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> d, TKey k, TValue def = default)
        => d.TryGetValue(k, out var v) ? v : def;
}
