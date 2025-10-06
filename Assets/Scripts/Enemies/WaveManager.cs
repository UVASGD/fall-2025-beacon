using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Singleton;

    public GameObject frigateSwarmPrefab;
    public GameObject carrierSpawnPrefab;
    public float spawnInBetweenTime = 1f;
    public GameObject startNextWaveButton;

    [Header("Portal Spawn Settings")]
    public Sprite[] portalSprites;
    public int portalsPerWave = 3;
    public float innerRadius = 5f;
    public float outerRadius = 15f;
    public Vector3 centerPoint = Vector3.zero;

    private List<GameObject> activePortals = new List<GameObject>();

    public delegate void Simple();
    public event Simple onWaveFinished;
    public event Simple onWaveStart;

    [SerializeField]
    private int waveCount = 1;

    [SerializeField]
    private int remainingEnemies = 0;
    private bool waveInProgress;

    private void Awake()
    {
        Singleton = this;   
    }

    private void Start()
    {

    }

    private void Update()
    {
        if(remainingEnemies == 0 && waveInProgress) {
            Debug.Log("Wave is finished");
            waveInProgress = false;
            DestroyPortals();
            if(onWaveFinished != null)
            {
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
        startNextWaveButton.SetActive(false);
        if (onWaveStart != null)
        {
            onWaveStart();
        }

        // Spawn portals for this wave
        SpawnPortalsForWave();

        int enemiesToAssign = waveCount * activePortals.Count;
        int[] portalsToSpawnCarrier = ReturnRandomIntegerArrayMinMax(waveCount / 3, 0, activePortals.Count);
        int[] toSpawnCounts = ReturnRandomIntegerArray(activePortals.Count, 1, enemiesToAssign);
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
            StartCoroutine(SpawnFromPortal(activePortals[i], toSpawn));
        }
        waveCount += 1;
        waveInProgress = true;
    }

    public void OnEnemySpawn()
    {
        remainingEnemies++;
    }

    public void OnEnemyDie()
    {
        remainingEnemies--;
    }

    void SpawnPortalsForWave()
    {
        if (portalSprites == null || portalSprites.Length == 0)
        {
            Debug.LogError("No portal sprites assigned to WaveManager!");
            return;
        }

        for (int i = 0; i < portalsPerWave; i++)
        {
            // Generate random position in ring/annulus
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float distance = Random.Range(innerRadius, outerRadius);
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
            Vector3 spawnPos = centerPoint + new Vector3(offset.x, 0, offset.y);

            // Create portal GameObject
            GameObject portal = new GameObject("Portal_" + i);
            portal.transform.position = spawnPos;

            // Add SpriteRenderer and assign random sprite
            SpriteRenderer spriteRenderer = portal.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = portalSprites[Random.Range(0, portalSprites.Length)];

            activePortals.Add(portal);
        }
    }

    void DestroyPortals()
    {
        foreach (GameObject portal in activePortals)
        {
            if (portal != null)
            {
                Destroy(portal);
            }
        }
        activePortals.Clear();
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

    private void OnDrawGizmosSelected()
    {
        // Draw inner radius circle
        Gizmos.color = Color.yellow;
        DrawCircle(centerPoint, innerRadius);

        // Draw outer radius circle
        Gizmos.color = Color.red;
        DrawCircle(centerPoint, outerRadius);

        // Draw center point
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(centerPoint, 0.3f);
    }

    private void DrawCircle(Vector3 center, float radius)
    {
        int segments = 64;
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(radius, 0, 0);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
}
