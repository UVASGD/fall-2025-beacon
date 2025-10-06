using System;
using System.Collections.Generic;
using System.Linq;
using Beacon.ProcGen;
using UnityEngine;

// ================================================
//  Relic System v1.2 — Expanded to match legacy UI/roller expectations
//  - Adds: RelicInstance.icon, implicitIds, affixIds
//  - RelicAffixDef: lines is List<Line>, allowedClasses, stackable, IsAllowed(), RollLines()
//  - RelicLegendaryDef: allowedClasses, IsAllowed()
//  - Run hooks: NotifyWaveFinished(), ShouldOfferRelicNow()
//  - Removed extension GetValueOrDefault to avoid ambiguity with project DictExt
// ================================================

#region Enums & Flags

[System.Flags]
public enum RelicTag
{
    None = 0,
    Projectile = 1 << 0,
    Crit = 1 << 1,
    FireRate = 1 << 2,
    Aura = 1 << 3,
    Summon = 1 << 4,
    Range = 1 << 5,
    Health = 1 << 6,
    Boss = 1 << 7,
}

public enum RelicClass
{
    Generic = 0,
    Cannon = 1,
    Booster = 2,
    Hangar = 3,
}

public enum RelicRarity
{
    Common = 0,
    Uncommon = 1,
    Rare = 2,
    Epic = 3,
    Legendary = 4,
}

#endregion

#region Scriptable Definitions

// Affix definition with one or more "lines" that can roll within [min,max]
public class RelicAffixDef : ScriptableObject
{
    public string id;
    public string displayName;
    public RelicTag tags = RelicTag.None;
    public bool stackable = true;

    // Which classes can use this affix (empty/null = allowed for all)
    public List<RelicClass> allowedClasses = new();

    [System.Serializable]
    public struct Line
    {
        public StatType stat;
        public float min;
        public float max;
        public bool isPercent;

        // rarity scalars (1.0 = no change)
        public float rarityScalarCommon;
        public float rarityScalarUncommon;
        public float rarityScalarRare;
        public float rarityScalarEpic;
        public float rarityScalarLegendary;
    }

    // NOTE: List<T> to support ".Count" property used by legacy code
    public List<Line> lines = new();

    public bool IsAllowed(RelicClass c)
    {
        if (allowedClasses == null || allowedClasses.Count == 0) return true;
        return allowedClasses.Contains(c);
    }
    public bool IsAllowed(RelicClass c, RelicTag itemTags)
    {
        // Class gate: empty allowedClasses => allowed for all
        if (allowedClasses != null && allowedClasses.Count > 0 && !allowedClasses.Contains(c))
            return false;

        // Tag gate: if this affix has no tags, allow it; otherwise require an overlap
        if (tags == RelicTag.None) return true;
        return (tags & itemTags) != 0;
    }


    // Helper to pick the scalar for the rarity; 1.0f when unset/zero
    private static float RarityScalar(Line line, RelicRarity rarity)
    {
        switch (rarity)
        {
            case RelicRarity.Common: return (line.rarityScalarCommon == 0f) ? 1f : line.rarityScalarCommon;
            case RelicRarity.Uncommon: return (line.rarityScalarUncommon == 0f) ? 1f : line.rarityScalarUncommon;
            case RelicRarity.Rare: return (line.rarityScalarRare == 0f) ? 1f : line.rarityScalarRare;
            case RelicRarity.Epic: return (line.rarityScalarEpic == 0f) ? 1f : line.rarityScalarEpic;
            case RelicRarity.Legendary: return (line.rarityScalarLegendary == 0f) ? 1f : line.rarityScalarLegendary;
            default: return 1f;
        }
    }

    // Convenience: roll into a list of modifiers for the given rarity
    public List<RelicDefinition.Modifier> RollLines(RelicRarity rarity)
    {
        var mods = new List<RelicDefinition.Modifier>();
        if (lines == null) return mods;
        foreach (var line in lines)
        {
            float baseRoll = UnityEngine.Random.Range(line.min, line.max);
            float scalar = 1f;
            switch (rarity)
            {
                case RelicRarity.Common: scalar = line.rarityScalarCommon == 0 ? 1f : line.rarityScalarCommon; break;
                case RelicRarity.Uncommon: scalar = line.rarityScalarUncommon == 0 ? 1f : line.rarityScalarUncommon; break;
                case RelicRarity.Rare: scalar = line.rarityScalarRare == 0 ? 1f : line.rarityScalarRare; break;
                case RelicRarity.Epic: scalar = line.rarityScalarEpic == 0 ? 1f : line.rarityScalarEpic; break;
                case RelicRarity.Legendary: scalar = line.rarityScalarLegendary == 0 ? 1f : line.rarityScalarLegendary; break;
            }
            float value = baseRoll * scalar;
            mods.Add(new RelicDefinition.Modifier
            {
                stat = line.stat,
                add = line.isPercent ? 0f : value,
                mul = line.isPercent ? value : 0f
            });
        }
        return mods;
    }

    // Overload used by RelicRoller: deterministic rolls with your DetRng, append into 'into'
    public void RollLines(ref DetRng rng, RelicRarity rarity, List<RelicDefinition.Modifier> into)
    {
        if (into == null || lines == null) return;

        for (int i = 0; i < lines.Count; i++)
        {
            var line = lines[i];

            // Deterministic value in [min, max] using your RNG
            float t = (float)rng.NextDouble01();
            float baseRoll = Mathf.Lerp(line.min, line.max, t);

            float scalar = RarityScalar(line, rarity);
            float value = baseRoll * scalar;

            into.Add(new RelicDefinition.Modifier
            {
                stat = line.stat,
                add = line.isPercent ? 0f : value,
                mul = line.isPercent ? value : 0f
            });
        }
    }
}

// Class definition: implicit affixes and class-specific pool
public class RelicClassDef : ScriptableObject
{
    public string displayName; // avoid colliding with UnityEngine.Object.name
    public RelicClass classId = RelicClass.Generic;
    public RelicTag providedTags = RelicTag.None;
    public List<RelicAffixDef> implicitAffixes = new();
    public List<RelicAffixDef> affixPool = new();
}

// Legendary definition: flat bundle of modifiers + a display name
public class RelicLegendaryDef : ScriptableObject
{
    public string id;
    public string displayName;
    public List<RelicDefinition.Modifier> flatModifiers = new();

    public List<RelicClass> allowedClasses = new();

    public bool IsAllowed(RelicClass c)
    {
        if (allowedClasses == null || allowedClasses.Count == 0) return true;
        return allowedClasses.Contains(c);
    }
}

#endregion

#region Runtime Instance

public class RelicInstance
{
    public string id;                 // stable-ish id for this roll (hash-based)
    public string displayName;        // for UI
    public RelicClass classId;
    public RelicRarity rarity;
    public RelicTag tags;
    public string legendaryId;        // optional
    public string seedHash;           // string version of the internal hash

    public Sprite icon;               // for UI buttons/cards

    // For UI: which affixes contributed (implicit and rolled)
    public List<string> implicitIds = new();
    public List<string> affixIds = new();

    public List<RelicDefinition.Modifier> resolvedModifiers = new();
}

#endregion

public class RelicManager : MonoBehaviour
{
    public static RelicManager Singleton;

    [Header("Run context")]
    public int runId = 1;
    public int stageIndex = 0;
    public int itemLevel = 1;
    public bool autoGatherBuildingTags = false; // if true, merge tags from current build (stubbed)
    public RelicTag extraRunTags = RelicTag.None; // user-supplied snapshot of the build

    [Header("Wave offer logic")]
    [SerializeField] private bool incrementStageOnWaveFinish = true; // if ShopManager doesn't pass a wave index
    [SerializeField] private int offerEveryNWaves = 1;               // offer each N waves (1 = every wave)
    private int _wavesSinceLastOffer = 0;
    private bool pendingOffer;

    [Header("Offer settings")]
    public int choicesPerOffer = 3;
    public Vector2Int affixCountRange = new(1, 2); // (min,max) rolled affixes from pools (in addition to implicit)

    [Header("Content")]
    public List<RelicClassDef> classDefs = new();
    public List<RelicAffixDef> genericAffixes = new();
    public List<RelicLegendaryDef> legendaryDefs = new();

    [Header("Rarity Weights (sum need not equal 1)")]
    public float wCommon = 60f;
    public float wUncommon = 25f;
    public float wRare = 12f;
    public float wEpic = 3f;
    public float wLegendary = 1f;

    // Totals
    private readonly Dictionary<StatType, float> addTotals = new();
    private readonly Dictionary<StatType, float> mulTotals = new();

    public System.Action OnRelicsChanged;

    void Awake()
    {
        if (Singleton == null) Singleton = this;
        InitTotals();
    }

    void InitTotals()
    {
        // Ensure all StatType keys exist
        foreach (StatType st in System.Enum.GetValues(typeof(StatType)))
        {
            if (!addTotals.ContainsKey(st)) addTotals[st] = 0f;
            if (!mulTotals.ContainsKey(st)) mulTotals[st] = 0f;
        }
    }

    // --------------------------- Public API ---------------------------

    // Legacy hook used by ShopManager
    public void NotifyWaveFinished()
    {
        if (incrementStageOnWaveFinish) stageIndex++;
        HandlePostWaveForRelics();
    }

    private void HandlePostWaveForRelics()
    {
        _wavesSinceLastOffer++;
        if (_wavesSinceLastOffer >= Mathf.Max(1, offerEveryNWaves))
        {
            pendingOffer = true;
            _wavesSinceLastOffer = 0;
        }
    }

    // Legacy hook used by ShopManager
    public bool ShouldOfferRelicNow()
    {
        if (pendingOffer)
        {
            pendingOffer = false;
            return true;
        }
        return false;
    }

    // Primary: returns instances (preferred newer API)
    public List<RelicInstance> RollChoices()
    {
        var result = new List<RelicInstance>(choicesPerOffer);
        var effectiveTags = GetEffectiveTags();
        var viableClasses = GetViableClasses(effectiveTags);
        if (viableClasses.Count == 0) viableClasses.Add(null); // allow "generic-only" if no classes

        for (int i = 0; i < choicesPerOffer; i++)
        {
            var rarity = RollRarity();
            var cls = WeightedPick(viableClasses, c => ScoreClass(c, effectiveTags));
            var inst = BuildInstance(rarity, cls, effectiveTags, offerIndex: i);
            result.Add(inst);
        }
        return result;
    }

    // Legacy adapter for older UI expecting RelicDefinition list
    public List<RelicDefinition> RollChoicesLegacy()
    {
        var inst = RollChoices();
        var defs = new List<RelicDefinition>(inst.Count);
        foreach (var r in inst)
        {
            var def = ScriptableObject.CreateInstance<RelicDefinition>();
            def.id = r.id;
            def.name = r.displayName;
            def.modifiers = r.resolvedModifiers.ToList();
            defs.Add(def);
        }
        return defs;
    }

    // Apply a rolled relic
    public void Take(RelicInstance relic)
    {
        if (relic == null) return;
        foreach (var m in relic.resolvedModifiers)
        {
            float add = 0f, mul = 0f;
            addTotals.TryGetValue(m.stat, out add);
            mulTotals.TryGetValue(m.stat, out mul);
            addTotals[m.stat] = add + m.add;
            mulTotals[m.stat] = mul + m.mul;
        }
        OnRelicsChanged?.Invoke();
    }

    // Legacy compatibility: allow taking a simple RelicDefinition by treating its modifiers as-is.
    public void Take(RelicDefinition def)
    {
        if (def == null) return;
        foreach (var m in def.modifiers)
        {
            float add = 0f, mul = 0f;
            addTotals.TryGetValue(m.stat, out add);
            mulTotals.TryGetValue(m.stat, out mul);
            addTotals[m.stat] = add + m.add;
            mulTotals[m.stat] = mul + m.mul;
        }
        OnRelicsChanged?.Invoke();
    }

    public float GetAdd(StatType st)
    {
        float v; return addTotals.TryGetValue(st, out v) ? v : 0f;
    }
    public float GetMul(StatType st)
    {
        float v; return mulTotals.TryGetValue(st, out v) ? v : 0f;
    }
    public float Apply(float baseValue, StatType st) => (baseValue + GetAdd(st)) * (1f + GetMul(st));

    // --------------------------- Build helpers ---------------------------

    RelicInstance BuildInstance(RelicRarity rarity, RelicClassDef cls, RelicTag effectiveTags, int offerIndex)
    {
        var inst = new RelicInstance
        {
            classId = cls ? cls.classId : RelicClass.Generic,
            rarity = rarity,
            tags = effectiveTags,
            icon = null, // hook up per-class icons if you have them
        };

        var mods = new List<RelicDefinition.Modifier>();

        // Implicit affixes from class (if any)
        if (cls && cls.implicitAffixes != null)
        {
            foreach (var a in cls.implicitAffixes)
            {
                if (a == null) continue;
                inst.implicitIds.Add(a.id);
                mods.AddRange(a.RollLines(rarity));
            }
        }

        // Pooled affixes: mix class pool + generic pool
        var pool = new List<RelicAffixDef>();
        if (cls && cls.affixPool != null) pool.AddRange(cls.affixPool);
        if (genericAffixes != null) pool.AddRange(genericAffixes);

        int affixCount = Mathf.Clamp(UnityEngine.Random.Range(affixCountRange.x, affixCountRange.y + 1), 0, 16);
        for (int k = 0; k < affixCount && pool.Count > 0; k++)
        {
            var candidate = WeightedPick(pool, a => ScoreAffix(a, effectiveTags));
            if (candidate == null) break;
            if (!candidate.IsAllowed(inst.classId)) continue;

            inst.affixIds.Add(candidate.id);
            mods.AddRange(candidate.RollLines(rarity));

            if (!candidate.stackable)
                pool.Remove(candidate);
        }

        // Legendary: add a flat bundle and mark
        if (rarity == RelicRarity.Legendary && legendaryDefs != null && legendaryDefs.Count > 0)
        {
            // Prefer legendaries allowed for this class
            var allowed = legendaryDefs.Where(l => l != null && l.IsAllowed(inst.classId)).ToList();
            var legPool = allowed.Count > 0 ? allowed : legendaryDefs;
            var leg = legPool[UnityEngine.Random.Range(0, legPool.Count)];
            inst.legendaryId = leg?.id ?? string.Empty;
            if (leg != null && leg.flatModifiers != null) mods.AddRange(leg.flatModifiers);
        }

        // Name & id
        var className = cls ? cls.classId.ToString() : "Relic";
        inst.displayName = $"{rarity} {className}";
        inst.id = ComputeInstanceId(rarity, cls, mods, offerIndex);
        inst.seedHash = inst.id;

        inst.resolvedModifiers = mods;
        return inst;
    }

    RelicRarity RollRarity()
    {
        var weights = new (RelicRarity rr, float w)[]
        {
            (RelicRarity.Common,    Mathf.Max(0.0001f, wCommon)),
            (RelicRarity.Uncommon,  Mathf.Max(0.0001f, wUncommon)),
            (RelicRarity.Rare,      Mathf.Max(0.0001f, wRare)),
            (RelicRarity.Epic,      Mathf.Max(0.0001f, wEpic)),
            (RelicRarity.Legendary, Mathf.Max(0.0001f, wLegendary)),
        };
        return WeightedPick(weights, x => x.w).rr;
    }

    RelicTag GetEffectiveTags()
    {
        var tags = extraRunTags;
        if (autoGatherBuildingTags)
        {
            // TODO: scan placed buildings to add tags dynamically
        }
        return tags;
    }

    List<RelicClassDef> GetViableClasses(RelicTag tags)
    {
        var result = new List<RelicClassDef>();
        foreach (var c in classDefs)
        {
            if (!c) continue;
            if (c.providedTags == RelicTag.None) { result.Add(c); continue; }
            if ((c.providedTags & tags) != 0) result.Add(c);
        }
        return result;
    }

    float ScoreClass(RelicClassDef c, RelicTag tags)
    {
        if (!c) return 1f;
        int overlap = CountBits((int)(c.providedTags & tags));
        return 1f + overlap;
    }

    float ScoreAffix(RelicAffixDef a, RelicTag tags)
    {
        if (!a) return 1f;
        int overlap = CountBits((int)(a.tags & tags));
        return 1f + overlap;
    }

    static int CountBits(int v)
    {
        // builtin-ish popcount
        uint x = (uint)v;
        x = x - ((x >> 1) & 0x55555555u);
        x = (x & 0x33333333u) + ((x >> 2) & 0x33333333u);
        return (int)((((x + (x >> 4)) & 0x0F0F0F0Fu) * 0x01010101u) >> 24);
    }

    // Simple weighted pick
    static T WeightedPick<T>(IList<T> items, System.Func<T, float> weight)
    {
        if (items == null || items.Count == 0) return default;
        float sum = 0f;
        for (int i = 0; i < items.Count; i++) sum += Mathf.Max(0, weight(items[i]));
        float r = UnityEngine.Random.value * sum;
        for (int i = 0; i < items.Count; i++)
        {
            r -= Mathf.Max(0, weight(items[i]));
            if (r <= 0) return items[i];
        }
        return items[items.Count - 1];
    }

    // Build a deterministic-ish hash string from run context + composition
    string ComputeInstanceId(RelicRarity rarity, RelicClassDef cls, List<RelicDefinition.Modifier> mods, int offerIndex)
    {
        unchecked
        {
            uint h = 2166136261u; // FNV-1a 32-bit
            void Mix(int v)
            {
                h ^= (uint)v;
                h *= 16777619u;
            }

            Mix(runId);
            Mix(stageIndex);
            Mix(itemLevel);
            Mix(offerIndex);
            Mix((int)rarity);
            Mix(cls ? (int)cls.classId : -1);
            Mix((int)extraRunTags);

            foreach (var m in mods)
            {
                Mix((int)m.stat);
                // quantize to 1e-4 to avoid floating chaos
                Mix((int)(m.add * 10000f));
                Mix((int)(m.mul * 10000f));
            }

            return h.ToString("X8");
        }
    }
}
