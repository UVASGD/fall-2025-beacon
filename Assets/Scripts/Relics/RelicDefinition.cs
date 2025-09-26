using System.Collections.Generic;
using UnityEngine;

public enum StatType { Damage, FireRate, Range, ProjectileSpeed, MaxHealth }

[CreateAssetMenu(menuName = "Relics/Relic", fileName = "NewRelic")]
public class RelicDefinition : ScriptableObject
{
    [Header("Identity")]
    public string id; // Optional unique key. Leave blank to allow duplicates.
    public string displayName;
    [TextArea] public string description;
    public Sprite icon;

    [System.Serializable]
    public struct Modifier
    {
        public StatType stat;
        public float add; // flat bonus, e.g., +5 damage
        public float mul; // % bonus, 0.20 = +20%
    }

    [Header("Stat modifiers")]
    public List<Modifier> modifiers = new List<Modifier>();
}
