using System.Collections.Generic;
using UnityEngine;

public class AdjacentBuildingScanner : MonoBehaviour
{
    private float scanDistance = 4f; 
    public LayerMask buildingLayer;  // Only scan for objects on this layer

    // Returns a list of adjacent buildings (Building components)
    public List<GameObject> GetAdjacentBuildings()
    {
        List<GameObject> adjacentBuildings = new List<GameObject>();

        // Directions: Up, Down, Left, Right
        Vector3[] directions = new Vector3[]
        {
            Vector3.forward,
            Vector3.back,
            Vector3.left,
            Vector3.right
        };

        foreach (Vector3 dir in directions)
        {
            Vector3 scanPos = transform.position + dir * scanDistance;

            Collider[] hits = Physics.OverlapSphere(scanPos, 0.3f, buildingLayer);
            foreach (Collider hit in hits)
            {
                GameObject building = hit.gameObject;
                if (building != null && !adjacentBuildings.Contains(building))
                {
                    adjacentBuildings.Add(building);
                }
            }
        }

        return adjacentBuildings;
    }
}
