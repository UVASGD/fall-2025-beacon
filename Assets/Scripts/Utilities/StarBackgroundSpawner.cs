using UnityEngine;

public class StarBackgroundSpawner : MonoBehaviour
{
    public GameObject[] starPrefabs; // Prefabs of star clusters
    public Vector2 areaSize = new Vector2(100, 100); // Size of spawn area (X = width, Y = height)
    public float spacing = 10f; // Distance between clusters
    public Transform parent; // Optional parent for organization
    public float noSpawnChance = 0.5f; //Chance to not spawn a star;
    
    void Start()
    {
        SpawnStarfield();
    }

    void SpawnStarfield()
    {
        for (float x = -areaSize.x / 2; x < areaSize.x / 2; x += spacing)
        {
            for (float z = -areaSize.y / 2; z < areaSize.y / 2; z += spacing)
            {
                if (Random.Range(0f, 1f) < noSpawnChance)
                    continue;
                Vector3 position = new Vector3(x, -6f, z);
                GameObject prefab = starPrefabs[Random.Range(0, starPrefabs.Length)];

                GameObject star = Instantiate(prefab, position, Quaternion.identity, parent);

                star.transform.position += new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)) * spacing / 2f; 
            }
        }
    }
}
