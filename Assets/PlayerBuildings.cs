using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuildings : MonoBehaviour
{
    public List<Building> initialBuildings;
    public BuildingButtonGenerator buttonGenerator;

    [SerializeField]
    private List<Building> buildings = new List<Building>();

    private void Awake()
    {
        foreach(var building in initialBuildings)
        {
            AddBuilding(building);
        }
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
}
