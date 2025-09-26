using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuildings : MonoBehaviour
{
    private List<Building> initialBuildings; //set to private for better encapsulation. Please change value through SetInitialBuidlings
    public BuildingButtonGenerator buttonGenerator;

    [SerializeField]
    private List<Building> buildings = new List<Building>();

    void Start()
    {
        if (initialBuildings == null)
        {
            SetInitialBuildings(new List<Building>()); //ensuring initialBuildings is never null to avoid errors
            Debug.LogWarning("Initial buildings were not set. Defaulting to empty list.");
        }
        foreach (var building in initialBuildings) //moved this foreach from Awake to Start
        {
            if (building != null) AddBuilding(building);
        }
        if (buttonGenerator != null) buttonGenerator.GenerateBuildingButtons(buildings);
        else Debug.LogWarning($"{nameof(PlayerBuildings)}: buttonGenerator not assigned.");
    }

    public void AddBuilding(Building toAdd)
    {
        buildings.Add(toAdd);
        buttonGenerator.GenerateBuildingButtons(buildings);
    }

    public void RemoveAtIndex(int index)
    {
        buildings.RemoveAt(index);
        buttonGenerator.GenerateBuildingButtons(buildings);
    }

    public Building GetBuilding(int index)
    {
        return buildings[index];
    }

    public void SetInitialBuildings(List<Building> initial)
    {
        initialBuildings = initial;
    }
}
