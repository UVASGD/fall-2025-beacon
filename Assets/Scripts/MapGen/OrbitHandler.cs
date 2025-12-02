using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitHandler : MonoBehaviour
{
    public static OrbitHandler Instance;

    [SerializeField] List<OrbitalData> orbits;
    private List<GameObject> instantiatedOrbits;
    [SerializeField] CaptureChance bodyCaptureChance;
    void Awake()
    {
        Instance = this;

        Vector3 center = this.transform.position;

        instantiatedOrbits = new List<GameObject>(); //references to the instantiated orbits

        if (FactionManager.i.GetPlayerFaction().FactionBase.HasMoon) //Spawn moon if faction has moon, otherwise clear orbit list
        {
            foreach (var orbit in orbits)
            {
                var orbitingBody = Instantiate(orbit.associatedPlanet);
                orbit.RandomizePhasePercent();
                instantiatedOrbits.Add(orbitingBody);

                placePlanetaryOrbit(orbitingBody, orbit);
            }
        }
        else
        {
            orbits = new List<OrbitalData>();   
        }
    }
    void Start()
    {
        WaveManager.Singleton.onWaveFinished += OnTurnEnd;
    }
    public void Update()
    {
        //only if the wavestate is active
        if (WaveManager.Singleton.waveState == WaveState.Inactive)
            return;
        //orbit the planet relative to its phase length
        for (int i = 0; i < instantiatedOrbits.Count; i++)
        {
            //Developer note: Disabling orbits for gameplay testing
            //orbits[i].IncrementOrbitalProgress(Time.deltaTime);

            //placePlanetaryOrbit(instantiatedOrbits[i], orbits[i]);
        }
    }

    //Randomly places target away from others
    public static void PlaceOnGrid(Transform target, List<Transform> others, int gridSize, float cellSize, float minDistance, float maxDistance, bool gridSizeIsEven)
    {
        if (target == null) return;
        if (gridSize <= 0) return;

        // Centered grid � calculate extents
        int half = gridSize / 2;

        const int maxAttempts = 1000;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            attempts++;

            // Pick a random grid coordinate (aligned to grid)
            int xCell = Random.Range(-half, half + 1);
            int zCell = Random.Range(-half, half + 1);

            Vector3 newPos = new Vector3(xCell * cellSize, 0f, zCell * cellSize);
            if(gridSizeIsEven)
                newPos -= (Vector3.right * cellSize / 2f + Vector3.forward * cellSize / 2f);

            // Check distance from all others
            bool tooClose = false;
            bool tooFar = false;
            foreach (Transform t in others)
            {
                if (t == null || t == target) continue;
                if (Vector3.Distance(newPos, t.position) < minDistance)
                {
                    tooClose = true;
                    break;
                }
                else if (Vector3.Distance(newPos, t.position) > maxDistance)
                {
                    tooFar = true;
                    break;
                }
            }

            if (!tooClose && !tooFar)
            {
                target.position = newPos;
                return;
            }
        }

        Debug.LogWarning($"[GridPlacer] Could not find a valid position for {target.name} after {maxAttempts} attempts.");
    }

    public void OnTurnEnd()
    {
        AsteroidChance();
    }

    private void placePlanetaryOrbit(GameObject orbitingBody, OrbitalData orbit)
    {
        List<Transform> orbitTransforms = new List<Transform>();
        foreach(var _orbit in orbits)
        {
            orbitTransforms.Add(_orbit.associatedPlanet.transform);
        }
        orbitTransforms.Add(HomePlanetController.i.transform);
        var planetHealth = orbitingBody.AddComponent<PlanetaryHealth>(); //storing planetHealth to remove additional calls to getComponent (as this is slow in realtime)
        planetHealth.SetMaxHealth(orbit.PlanetMaxHealth);
        planetHealth.TopOffHealth();
        planetHealth.SetRenderer(orbitingBody.GetComponentInChildren<SpriteRenderer>()); //configuring the hitflash (make sure orbiting bodies have their mesh as the earliest child possible)
        orbit.orbitingPlanet = orbitingBody;    
        PlaceOnGrid(orbitingBody.transform, orbitTransforms, 25, 4, 15f, 30f, orbit.BuildingSize % 2 == 0);

        return;
        /* commenting out this unreachable code
        Vector3 center = this.transform.position;
        float angle = orbit.phasePercent * 2 * Mathf.PI; //calculating the relative angle
        Vector3 offset = new Vector3(
                orbit.OrbitalDistance * Mathf.Cos(angle),
                0f,
                orbit.OrbitalDistance * Mathf.Sin(angle)
        );
        orbitingBody.AddComponent<PlanetaryHealth>().SetMaxHealth(orbit.PlanetMaxHealth);
        orbitingBody.GetComponent<PlanetaryHealth>().TopOffHealth();
        orbitingBody.transform.position = center + offset;
        */
    }
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
            Debug.Log($"Asteroid generated this turn of size {size}"); //size should be small, as 1 is the default size
            AddSingleOrbit(Instantiate(AsteroidVarieties.i.AsteroidPrefab), size);
        }
        else
            Debug.Log("Asteroid Not Generated");
    }

    private void AddSingleOrbit(GameObject orbitingObject, float scale)
    {
        instantiatedOrbits.Add(orbitingObject);

        SpawnedAsteroid spawnedAsteroid = orbitingObject.GetComponent<SpawnedAsteroid>();
        spawnedAsteroid.InitializeAsteroid(scale);

        OrbitalData orbitalData = spawnedAsteroid.OrbitalData;
        orbits.Add(orbitalData);
        placePlanetaryOrbit(orbitingObject, orbitalData);
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

    //Returns an orbital data associated with supplied gameObject. Returns null if none are associated.
    public OrbitalData GetOrbitalDataOfPlanet(GameObject associatedPlanet)
    {
        foreach (var orbData in orbits)
        {
            if (orbData.orbitingPlanet == associatedPlanet) //This is CORRECT, even though variable names don't match
                return orbData;
        }
        return null;
    }

    public Transform GetClosestPlanetToPosition(Vector3 position)
    {
        float closestDistance = Mathf.Infinity;
        Transform closestTransform = null;
        foreach(var orbData in orbits)
        {
            float distance = Vector3.Distance(position, orbData.associatedPlanet.transform.position);
            if(distance < closestDistance)
            {
                closestDistance = distance;
                closestTransform = orbData.associatedPlanet.transform;  
            }
        }

        return closestTransform;
    }

    public void RemoveOrbitalData(GameObject orbitingBody)
    {
        OrbitalData orbitalDataToRemove = null;
        foreach(var orbData in orbits)
        {
            if(orbData.orbitingPlanet == orbitingBody)
            {
                orbitalDataToRemove = orbData;
            }
        }

        orbits.Remove(orbitalDataToRemove);
    }
}