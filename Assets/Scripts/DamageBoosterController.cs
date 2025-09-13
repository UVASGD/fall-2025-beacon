using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AdjacentBuildingScanner))]
public class DamageBoosterController : MonoBehaviour
{
    public float damageBoostPercent = 0.1f;

    private void Start()
    {
        WaveManager.Singleton.onWaveStart += OnWaveStart;
    }

    public void OnWaveStart()
    {
        List<GameObject> adjacentBuildings = GetComponent<AdjacentBuildingScanner>().GetAdjacentBuildings();
        foreach(var b in adjacentBuildings)
        {
            if(b.GetComponent<DamageController>()  != null)
            {
                for(int i = 0; i < GetComponent<LevelController>().GetLevel(); i++)
                    b.GetComponent<DamageController>().damage *= (1 + damageBoostPercent);
            }
        }
    }
}
