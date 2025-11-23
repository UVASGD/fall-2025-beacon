using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionSelectionMenu : MonoBehaviour
{
    [SerializeField] GameObject iconPrefab;

    public void SpawnFactionIcons(List<Faction> factions)
    {
        foreach (var faction in factions)
        {
            FactionSelectionIcon icon = Instantiate(iconPrefab, this.transform).GetComponent<FactionSelectionIcon>();

            icon.factionTitle.text = faction.FactionBase.FactionName;
            icon.factionImage.sprite = faction.FactionBase.FactionIcon;
            icon.factionImage.GetComponent<RectTransform>().sizeDelta = faction.FactionBase.FactionIconSize;
            icon.factionId = faction.FactionBase.FactionID;
            icon.overviewText.text = faction.FactionBase.FactionOverview;
        }
    }
}