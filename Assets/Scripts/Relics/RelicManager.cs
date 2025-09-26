using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RelicManager : MonoBehaviour
{
    public static RelicManager Singleton;

    [Header("Offer cadence")]
    [Tooltip("Offer relics every N finished waves. Set 0 to disable.")]
    public int offerEveryNWaves = 2;
    public int choicesPerOffer = 3;

    [Header("Relic pool")]
    public List<RelicDefinition> allRelics = new();

    // Totals
    private readonly Dictionary<StatType, float> addTotals = new();
    private readonly Dictionary<StatType, float> mulTotals = new();
    private readonly HashSet<string> takenIds = new();

    private int finishedWaves = 0;

    public System.Action OnRelicsChanged;

    void Awake()
    {
        if (Singleton != null && Singleton != this) { Destroy(gameObject); return; }
        Singleton = this;
        DontDestroyOnLoad(gameObject);

        foreach (StatType st in System.Enum.GetValues(typeof(StatType)))
        {
            addTotals[st] = 0f;
            mulTotals[st] = 0f;
        }
    }

    // Call this when a wave ends (we'll do it from the Shop flow)
    public void NotifyWaveFinished() => finishedWaves++;

    public bool ShouldOfferRelicNow()
        => offerEveryNWaves > 0 && finishedWaves % offerEveryNWaves == 0;

    public List<RelicDefinition> RollChoices()
    {
        var pool = allRelics.Where(r => string.IsNullOrEmpty(r.id) || !takenIds.Contains(r.id)).ToList();
        if (pool.Count == 0) pool = new List<RelicDefinition>(allRelics); // allow repeats if exhausted

        var result = new List<RelicDefinition>();
        for (int i = 0; i < choicesPerOffer && pool.Count > 0; i++)
        {
            int idx = Random.Range(0, pool.Count);
            result.Add(pool[idx]);
            pool.RemoveAt(idx);
        }
        return result;
    }

    public void Take(RelicDefinition relic)
    {
        if (relic == null) return;
        if (!string.IsNullOrEmpty(relic.id)) takenIds.Add(relic.id);

        foreach (var m in relic.modifiers)
        {
            addTotals[m.stat] += m.add;
            mulTotals[m.stat] += m.mul; // Linear stacking of percentages: 10% + 20% = 30%
        }

        OnRelicsChanged?.Invoke();
    }

    public float GetAdd(StatType st) => addTotals[st];
    public float GetMul(StatType st) => mulTotals[st];

    // (base + sumAdd) * (1 + sumMul)
    public float Apply(float baseValue, StatType st)
        => (baseValue + addTotals[st]) * (1f + mulTotals[st]);
}
