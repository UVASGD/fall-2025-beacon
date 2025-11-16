using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Singleton;
    public GameObject frigateSwarmPrefab;
    public GameObject carrierSpawnPrefab;
    public List<GameObject> portals = new List<GameObject>();
    public float spawnInBetweenTime = 1f;
    public GameObject startNextWaveButton;
    public GameObject portalPrefab;

    public delegate void Simple();
    public event Simple onWaveFinished;
    public event Simple onWaveStart;

    [SerializeField]
    private int waveCount = 1;

    [SerializeField]
    private int remainingEnemies = 0;
    private bool waveInProgress;


    public WaveState waveState { get; private set; }
    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        waveState = WaveState.Inactive;
    }

    private void Update()
    {
        if(remainingEnemies == 0 && waveInProgress) {
            Debug.Log("Wave is finished");
            waveInProgress = false;
            if(onWaveFinished != null)
            {
                waveState = WaveState.Inactive;
                onWaveFinished();
            }
        }
    }

    public void ShowStartNextWaveButton()
    {
        startNextWaveButton.SetActive(true);
    }

    public void StartNewWave()
    {
        if(PlayerBuildings.Instance.GetBuildings().Length != 0)
        {
            return;
        }

        SpawnNewPortals();
        startNextWaveButton.SetActive(false);
        if (onWaveStart != null)
        {
            waveState = WaveState.Active;
            onWaveStart();
        }
        int enemiesToAssign = waveCount * portals.Count;
        int[] portalsToSpawnCarrier = ReturnRandomIntegerArrayMinMax(waveCount / 3, 0, portals.Count);
        int[] toSpawnCounts = ReturnRandomIntegerArray(portals.Count, 1, enemiesToAssign);
        for(int i = 0; i < toSpawnCounts.Length; i++)
        {
            List<GameObject> toSpawn = new List<GameObject>();
            int carriersToAdd = CountIntegerInArray(i, portalsToSpawnCarrier);
            for(int j = 0; j < toSpawnCounts[i]; j++)
            {
                toSpawn.Add(frigateSwarmPrefab);
            }
            for(int k = 0; k < carriersToAdd; k++)
            {
                toSpawn.Add(carrierSpawnPrefab);
            }
            StartCoroutine(SpawnFromPortal(portals[i], toSpawn));
        }
        waveCount += 1;
        waveInProgress = true;
    }

    private void SpawnNewPortals()
    {
        foreach(var portal in portals)
        {
            Destroy(portal);
        }

        int portalCount = 4;
        List<GameObject> allOrbitingBodies = new List<GameObject>();
        foreach(var health in PlanetaryHealth.planetaryHealths)
        {
            allOrbitingBodies.Add(health.gameObject);
        }
        List<GameObject> newPortals = PrefabPlacementHelper.PlacePrefabs(portalPrefab, portalCount, 20f, 80f, allOrbitingBodies);
        portals = newPortals;   
    }

    public void OnEnemySpawn()
    {
        remainingEnemies++;
    }

    public void OnEnemyDie()
    {
        remainingEnemies--; 
    }

    IEnumerator SpawnFromPortal(GameObject portal, List<GameObject> toSpawn)
    {
        int currentIndex = 0;
        while(currentIndex < toSpawn.Count)
        {
            Instantiate(toSpawn[currentIndex], portal.transform.position, portal.transform.rotation);
            yield return new WaitForSeconds(spawnInBetweenTime);
            currentIndex++;
        }
        yield return new WaitForSeconds(1f);
        portal.GetComponent<SmoothRandomRotateAndScale>().ShrinkAndDestroy();
    }

    public int CountIntegerInArray(int targetInteger, int[] integerArray)
    {
        int count = 0;
        for (int i = 0; i < integerArray.Length; i++)
        {
            if(integerArray[i] == targetInteger)
                count++;
        }

        return count;
    }

    public int[] ReturnRandomIntegerArrayMinMax(int length, int min, int max)
    {
        int[] array = new int[length];  
        for(int i = 0; i < length; i++)
        {
            array[i] = Random.Range(min, max);
        }
        return array;   
    }

    public int[] ReturnRandomIntegerArray(int length, int min, int sum)
    {
        int[] array = new int[length];
        for(int i = 0; i < length; i++)
        {
            array[i] = min;
        }
        while(SumArray(array) < sum)
        {
            array[Random.Range(0, length)]++;
        }
        return array;
    }

    public int SumArray(int[] toSum)
    {
        int returnSum = 0;
        foreach(int i in toSum)
        {
            returnSum += i;
        }
        return returnSum;
    }

    public int GetFinishedWaveCount()
    {
        return waveCount - 2;
    }

    private void OnDestroy()
    {
        onWaveFinished = null;
        onWaveStart = null;
    }
}

public enum WaveState
{
    Active,
    Inactive //used to pause orbits if the wavestate is inactive (during the build/place phase)
}