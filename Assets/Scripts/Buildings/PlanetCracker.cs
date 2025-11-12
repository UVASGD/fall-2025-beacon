using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetCracker : MonoBehaviour
{
    public static List<PlanetCracker> crackerControllers = new List<PlanetCracker>();

    OrbitalData associatedOrbitalData;

    void Awake()
    {
        crackerControllers.Add(this);
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

    public int GetCrackingMoney()
    {
        associatedOrbitalData.associatedPlanet.GetComponent<PlanetaryHealth>().ChangeHealth(-20f);
        return Mathf.RoundToInt(associatedOrbitalData.BaseOreContent * 0.40f);
    }

    private void OnDestroy()
    {
        crackerControllers.Remove(this);
    }
}
