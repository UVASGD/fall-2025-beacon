using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetCracker : MonoBehaviour
{
    public static List<PlanetCracker> crackerControllers = new List<PlanetCracker>();

    GameObject associatedPlanet;
    bool onMainPlanet;

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
        associatedPlanet = hitObject;
        onMainPlanet = hitObject.GetComponent<HomePlanetController>() != null;
    }

    public int GetCrackingMoney()
    {
        int crackingDamage = 20;

        var crackMod = .2f;
        var associatedPlanetaryHealth = GetComponent<PlanetaryHealth>();
        if(associatedPlanetaryHealth.GetHealth() > crackingDamage * 2f)
        {
            associatedPlanetaryHealth.ChangeHealth(-crackingDamage);
            crackMod *= 2f;
        }

        var mainPlanetMod = onMainPlanet ? 0.25f : 1f;

        return Mathf.RoundToInt(SpawnedAsteroid.DEFAULTORECONTENT * crackMod * mainPlanetMod);
    }

    private void OnDestroy()
    {
        crackerControllers.Remove(this);
    }
}
