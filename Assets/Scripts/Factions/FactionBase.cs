using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "newFactionBase", menuName = "ScriptableObjects/FactionBase")]
public class FactionBase : ScriptableObject
{
    [Header("General")]
    [SerializeField] string factionName; //simple name of the faction
    [SerializeField] string factionAdjective; //for use in dialogue popups
    [SerializeField] int factionID; //integer index of the faction, determining the ordering of the faction on the select screen
    [SerializeField] Sprite factionIcon;
    [SerializeField] List<Building> factionBuildings; //list of relevant faction buildings
    [SerializeField] List<Building> startingBuildings; //buildings that are available for free round 1 placement
    [SerializeField] Planet homePlanet; //starting planet of the faction

    [Header("Selection Screen Stuff")]
    [TextArea(5, 10)]
    [SerializeField] string factionOverview; //a short paragraph describing the faction's backstory and scenario
    [SerializeField] Sprite leaderPortrait; //if we have character leaders for each faction
    [SerializeField] string hoverText; //when hovering over the faction icon on the selection menu, text box with this data pops up about their gameplay style

    // Public getters
    public string FactionName => factionName;
    public string FactionAdjective => factionAdjective;
    public int FactionID => factionID;
    public Sprite FactionIcon => factionIcon;
    public List<Building> FactionBuildings => factionBuildings;
    public List<Building> StartingBuildings => startingBuildings;
    public Planet HomePlanet => homePlanet;
    public string FactionOverview => factionOverview;
    public Sprite LeaderPortrait => leaderPortrait;
    public string HoverText => hoverText;
}