using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[Serializable]
public struct UIButtonSpec
{
    // Visuals
    public string title;           // main label
    public string subtitle;        // optional small text (cost, rarity, etc.)
    public Sprite icon;            // optional
    public Color backgroundColor;  // background tint (rarity, category color)
    public bool interactable;      // disabled style + blocks click

    // Optional “badge” (e.g., cost, stock)
    public string badgeText;       // e.g., "$100", "x3"
    public Color badgeColor;

    // Tooltip (rich text ok)
    public string tooltipRichText; // if null/empty, no tooltip

    // Click behavior
    public Action onClick;         // assign per-item callback

    // (Optional) for debugging / analytics
    public string debugId;         // e.g., relic seed hash, building id
}

// Simple helpers for defaults
public static class UIButtonSpecDefaults
{
    public static UIButtonSpec Make(string title, Action onClick)
    {
        return new UIButtonSpec
        {
            title = title,
            interactable = true,
            backgroundColor = Color.white,
            badgeColor = new Color32(0, 0, 0, 100),
            onClick = onClick
        };
    }
}
