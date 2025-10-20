using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineController : MonoBehaviour
{
    public static List<MineController> mineControllers = new List<MineController>();

    OrbitalData associatedOrbitalData;

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
        OrbitalData orbitalData = OrbitHandler.Instance.GetOrbitalDataOfPlanet(hitObject);
        if (orbitalData != null)
            associatedOrbitalData = orbitalData;
    }

    public int GetMiningMoney()
    {
        return Mathf.RoundToInt(associatedOrbitalData.BaseOreContent * 0.2f);
    }

    private void OnDestroy()
    {
        mineControllers.Remove(this);
    }
}
