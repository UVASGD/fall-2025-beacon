using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Beacon.ProcGen; // << use your procgen RNG + seeds

public static class RelicRoller
{
    [Serializable]
    public struct RollContext
    {
        public ulong runSeed;          // from SeedRegistry.RunSeed
        public int runId;              // optional
        public int stageIndex;         // optional
        public int itemLevel;          // optional
        public RelicTag runContextTags; // tags from the current build/run
    }

    [Serializable]
    public struct ContentRefs
    {
        public List<RelicClassDef> classes;
        public List<RelicAffixDef> generics;
        public List<RelicLegendaryDef> legendaries;
    }

    public static RelicInstance RollOne(RollContext ctx, ContentRefs content, int offerSetSerial, int offerIndex)
    {
        ulong sub = SeedRegistry.SubSeed($"Relics/OfferSet/{offerSetSerial}/Choice/{offerIndex}");

        var rngRarity = DetRng.FromContext(sub, "rarity");
        var rngClass = DetRng.FromContext(sub, "class");
        var rngAffix = DetRng.FromContext(sub, "affix");
        var rngValue = DetRng.FromContext(sub, "value");
        var rngLeg = DetRng.FromContext(sub, "legend");

        var rarity = PickRarity(ref rngRarity);
        int slots = rarity switch
        {
            RelicRarity.Common => 1,
            RelicRarity.Uncommon => 2,
            RelicRarity.Rare => 3,
            RelicRarity.Epic => 4,
            RelicRarity.Legendary => (rngRarity.Chance(0.30) ? 5 : 4),
            _ => 2
        };

        // --- class (bias using runContextTags)
        var cls = PickClass(ref rngClass, content.classes, ctx.runContextTags, out var classDef);

        // --- instance shell
        var inst = new RelicInstance
        {
            classId = cls,
            rarity = rarity,
            seedHash = MakeShortHash(ctx.runSeed, offerIndex),
            displayName = $"{rarity} {classDef.classId}",
            icon = null
        };

        // --- implicits
        if (classDef.implicitAffixes != null)
        {
            foreach (var imp in classDef.implicitAffixes)
            {
                if (imp == null) continue;
                inst.implicitIds.Add(imp.id);
                imp.RollLines(ref rngValue, rarity, inst.resolvedModifiers); // << procgen path
            }
        }

        // --- candidate pool (class + generic), filtered by tags
        RelicTag itemTags = classDef.providedTags | ctx.runContextTags;
        var candidates = new List<RelicAffixDef>();
        if (classDef.affixPool != null) candidates.AddRange(classDef.affixPool);
        if (content.generics != null) candidates.AddRange(content.generics);
        candidates = candidates.Where(a => a != null && a.IsAllowed(cls, itemTags)).ToList();

        // --- pick affixes (no dup unless stackable)
        for (int i = 0; i < slots && candidates.Count > 0; i++)
        {
            int idx = (int)(rngAffix.NextUInt() % (uint)candidates.Count);
            var a = candidates[idx];
            inst.affixIds.Add(a.id);
            a.RollLines(ref rngValue, rarity, inst.resolvedModifiers);

            if (!a.stackable) candidates.RemoveAll(x => x == a);
        }

        // --- legendary (epic/legendary only)
        if (rarity >= RelicRarity.Epic && content.legendaries != null && content.legendaries.Count > 0)
        {
            double p = rarity == RelicRarity.Epic ? 0.10 : 0.35;
            if (rngLeg.Chance(p))
            {
                var legPool = content.legendaries.Where(l => l != null && l.IsAllowed(cls)).ToList();
                if (legPool.Count > 0)
                {
                    int li = (int)(rngLeg.NextUInt() % (uint)legPool.Count);
                    var leg = legPool[li];
                    inst.legendaryId = leg.id;
                    inst.resolvedModifiers.AddRange(leg.flatModifiers);
                }
            }
        }

        return inst;
    }

    // -------- helpers --------

    static string MakeShortHash(ulong runSeed, int offerIndex)
    {
        ulong h = Hash.Hash64(runSeed, "RelicSeed", offerIndex);
        return ((uint)(h & 0xFFFFFFFF)).ToString("X8");
    }

    static RelicRarity PickRarity(ref DetRng rng)
    {
        // tweak: Common 45%, Uncommon 30%, Rare 17%, Epic 6.5%, Legendary 1.5%
        double r = rng.NextDouble01();
        if (r < 0.45) return RelicRarity.Common;
        if (r < 0.75) return RelicRarity.Uncommon;
        if (r < 0.92) return RelicRarity.Rare;
        if (r < 0.985) return RelicRarity.Epic;
        return RelicRarity.Legendary;
    }

    static RelicClass PickClass(ref DetRng rng, List<RelicClassDef> classes, RelicTag runTags, out RelicClassDef def)
    {
        float wCannon = 1f + (((runTags & RelicTag.Projectile) != 0) ? 0.5f : 0f);
        float wBooster = 1f + (((runTags & RelicTag.Aura) != 0) ? 0.5f : 0f);
        float wHangar = 1f + (((runTags & RelicTag.Summon) != 0) ? 0.5f : 0f);

        var pool = new List<(RelicClassDef d, float w)>();
        foreach (var d in classes)
        {
            if (d == null) continue;
            float w = d.classId switch
            {
                RelicClass.Cannon => wCannon,
                RelicClass.Booster => wBooster,
                RelicClass.Hangar => wHangar,
                _ => 0.8f
            };
            pool.Add((d, w));
        }
        if (pool.Count == 0) { def = null; return RelicClass.Generic; }

        var weights = pool.Select(t => t.w).ToList();
        int idx = WeightedChoice.PickIndex(rng.NextDouble01(), weights);
        def = pool[Mathf.Clamp(idx, 0, pool.Count - 1)].d;
        return def.classId;
    }
}
