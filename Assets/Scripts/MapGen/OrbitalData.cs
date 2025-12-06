using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OrbitalData //contains data for child orbits that are contained in the planet class
{
    [SerializeField] int phaseLength; //in seconds
    [SerializeField] public float phaseProgress; //elapsed time in the realtime phase
    [SerializeField] int orbitalDistance;
    public float phasePercent => phaseProgress / phaseLength;
    [SerializeField] public GameObject associatedPlanet;
    [SerializeField] public GameObject orbitingPlanet; //GameObject of orbital in game, NOT a prefab

    [SerializeField] bool counterClockwise = true; //if false, this body orbits in retrograde
    [SerializeField] float baseOreContent;
    [SerializeField] int planetMaxHealth;
    [SerializeField] int buildingSize;

    [SerializeField] WorldSpaceHealthbar healthBar;

    public int PhaseLength => phaseLength;
    public int OrbitalDistance => orbitalDistance;
    public float BaseOreContent => baseOreContent;
    public int PlanetMaxHealth => planetMaxHealth;
    public int BuildingSize => buildingSize;    
    public void IncrementOrbitalProgress(float timeDelta)
    {
        if (counterClockwise)
            phaseProgress += timeDelta;
        else
            phaseProgress -= timeDelta;

        if (Math.Abs(phaseProgress) > phaseLength)
        {
            phaseProgress -= phaseLength; //to reduce progress back to below its phase length
        }
    }

    public void SetBuildingSize(int setTo)
    {
        buildingSize = setTo;
    }

    public void SetPlanetMaxHealth(int setTo)
    {
        planetMaxHealth = setTo;
    }

    public void SetBaseOreContent(int setTo)
    {
        baseOreContent = setTo;
    }

    public void RandomizePhasePercent()
    {
        phaseProgress = Mathf.RoundToInt(phaseLength * UnityEngine.Random.Range(-1f, 1f));
    }
}