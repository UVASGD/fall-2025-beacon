using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineController : MonoBehaviour
{
    public static List<MineController> mineControllers = new List<MineController>();

    bool onMainPlanet = false;

    void Awake()
    {
        mineControllers.Add(this);
        GetAssociatedOrbitalData();        
    }

    void GetAssociatedOrbitalData()
    {
        Collider[] hits = Physics.OverlapBox(transform.position, Vector3.one * 0.5f, Quaternion.identity, LayerMask.GetMask("BuildableArea"));
        //Debug.Log($"Name of hit collider: {hits[0].transform.name}");
        GameObject hitObject = hits[0].transform.parent.gameObject;
        onMainPlanet = hitObject.GetComponent<HomePlanetController>() != null;
    }

    public int GetMiningMoney()
    {
        if(!onMainPlanet)
            return Mathf.RoundToInt(SpawnedAsteroid.DEFAULTORECONTENT * 0.2f);
        else
            return Mathf.RoundToInt(SpawnedAsteroid.DEFAULTORECONTENT * 0.2f * 0.25f);
    }

    private void OnDestroy()
    {
        mineControllers.Remove(this);
    }
}
