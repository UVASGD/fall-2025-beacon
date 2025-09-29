using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OrbitHandler : MonoBehaviour
{
    [SerializeField] List<OrbitalData> orbits;
    private List<GameObject> instantiatedOrbits;
    [SerializeField] CaptureChance bodyCaptureChance;
    void Awake()
    {
        Vector3 center = this.transform.position;

        instantiatedOrbits = new List<GameObject>(); //references to the instantiated orbits

        foreach (var orbit in orbits)
        {
            var orbitingBody = Instantiate(orbit.associatedPlanet);

            instantiatedOrbits.Add(orbitingBody);

            placePlanetaryOrbit(orbitingBody, orbit);
        }
    }
    void Start()
    {
        WaveManager.Singleton.onWaveFinished += OnTurnEnd;
    }
    public void Update()
    {
        //orbit the planet relative to its phase length
        for (int i = 0; i < instantiatedOrbits.Count; i++)
        {
            orbits[i].IncrementOrbitalProgress(Time.deltaTime);

            placePlanetaryOrbit(instantiatedOrbits[i], orbits[i]);
        }
    }

    public void OnTurnEnd()
    {
        AsteroidChance();
    }

    private void placePlanetaryOrbit(GameObject orbitingBody, OrbitalData orbit)
    {
        Vector3 center = this.transform.position;
        float angle = orbit.phasePercent * 2 * Mathf.PI; //calculating the relative angle
        Vector3 offset = new Vector3(
                orbit.OrbitalDistance * Mathf.Cos(angle),
                0f,
                orbit.OrbitalDistance * Mathf.Sin(angle)
        );

        orbitingBody.transform.position = center + offset;
    }

    //Rant time: WHY are the axes in this project incorrect? Why is Z up/down instead of the Y axis??? Who made this decision? Rant over.

    private float previousValue;
    private bool usePrevious = false;

    public void AsteroidChance()
    {
        //for the asteroid capture chance
        float asteroidChance = GetNormalRandom(bodyCaptureChance.MeanCaptureChance, bodyCaptureChance.CaptureDeviation);
        if (asteroidChance > Random.value) //if our asteroidChance is greater than a random value from 0 to 1
        {
            //generate an asteroid based on its size average and deviation
            float size = GetNormalRandom(bodyCaptureChance.MeanBodySize, bodyCaptureChance.SizeDeviation);
            Debug.Log($"Asteroid generated this turn of size {size}");
        }
        else
            Debug.Log("Asteroid Not Generated");
    }


    //this function selects a random point on a bell curve using some algorithm Gauss (my goat) created. Important for asteroid generation.
    //Can be implemented into proc gen seeds if we replace the Random.value with a point on the perlin noise file.
    public float GetNormalRandom(float mean, float stdDev)
    {
        if (usePrevious)
        {
            usePrevious = false;
            return mean + previousValue * stdDev;
        }
        //generate two random numbers (0,1]
        float u1 = 1f - Random.value;
        float u2 = 1f - Random.value;

        //then do a Box-Muller transformation or something idk this was inspired by StackOverflow
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);

        //store z1 for next call (optimization)
        float randStdNormal2 = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Cos(2.0f * Mathf.PI * u2);
        previousValue = randStdNormal2;

        usePrevious = true;

        //return mean + the appropriate number of deviations
        return mean + randStdNormal * stdDev;
    }
}