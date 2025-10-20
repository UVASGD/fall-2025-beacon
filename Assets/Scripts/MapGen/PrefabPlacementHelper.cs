using System.Collections.Generic;
using UnityEngine;

public static class PrefabPlacementHelper
{
    /// <summary>
    /// Places X instances of a prefab randomly within range of given reference objects.
    /// Each placed prefab will be at least 'minDistance' away from all references,
    /// and within 'maxDistance' of at least one reference.
    /// </summary>
    public static List<GameObject> PlacePrefabs(GameObject prefab, int count, float minDistance, float maxDistance, List<GameObject> referenceObjects)
    {
        List<GameObject> spawned = new List<GameObject>();

        if (prefab == null || referenceObjects == null || referenceObjects.Count == 0)
        {
            Debug.LogWarning("PrefabPlacementHelper: Missing prefab or reference objects.");
            return spawned;
        }

        int placed = 0;
        int attempts = 0;
        const int maxAttempts = 5000;

        while (placed < count && attempts < maxAttempts)
        {
            attempts++;

            // Pick a random reference point
            GameObject refObj = referenceObjects[Random.Range(0, referenceObjects.Count)];

            // Random direction and distance
            Vector2 dir = Random.insideUnitCircle.normalized;
            float distance = Random.Range(minDistance, maxDistance);

            Vector3 candidate = refObj.transform.position + new Vector3(dir.x, 0f, dir.y) * distance;

            // Check distance constraints
            bool tooClose = false;
            bool validDistance = false;

            foreach (GameObject obj in referenceObjects)
            {
                if (obj == null) continue;
                float d = Vector3.Distance(candidate, obj.transform.position);

                if (d < minDistance)
                {
                    tooClose = true;
                    break;
                }

                if (d <= maxDistance)
                    validDistance = true;
            }

            if (tooClose || !validDistance)
                continue;

            GameObject newObj = Object.Instantiate(prefab, candidate, Quaternion.identity);
            spawned.Add(newObj);
            placed++;
        }

        if (placed < count)
            Debug.LogWarning($"PrefabPlacementHelper: Placed only {placed}/{count} prefabs after {attempts} attempts.");

        return spawned;
    }
}