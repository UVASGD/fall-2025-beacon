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

    public int PhaseLength => phaseLength;
    public int OrbitalDistance => orbitalDistance;

    public void IncrementOrbitalProgress(float timeDelta)
    {
        phaseProgress += timeDelta;
        if (phaseProgress > phaseLength)
        {
            phaseProgress -= phaseLength; //to reduce progress back to below its phase length
        }
    }
}