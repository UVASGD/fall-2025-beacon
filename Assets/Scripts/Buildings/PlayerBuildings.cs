using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuildings : MonoBehaviour
{
    public static PlayerBuildings Instance;

    private List<Building> initialBuildings; //set to private for better encapsulation. Please change value through SetInitialBuidlings
    public BuildingButtonGenerator buttonGenerator;

    [SerializeField]
    private List<Building> buildings = new List<Building>();

    private void Awake()
    {
        Instance = this;    
    }

    void Start()
    {
        foreach (var building in initialBuildings) //moved this foreach from Awake to Start
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

    public Building[] GetBuildings()
    {
        return buildings.ToArray();
    }

    public void SetInitialBuildings(List<Building> initial)
    {
        initialBuildings = initial;
    }
}
