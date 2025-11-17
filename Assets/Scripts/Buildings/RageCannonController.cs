using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RageCannonController : MonoBehaviour
{
    PlanetaryHealth associatedPlanetaryHealth;
    DamageController damageController;

    public float baseDamage = 5f;
    public float damageIncreasePerPercentHealthLoss = 0.1f;

    void Awake()
    {
        GetAssociatedOrbitalData();
        damageController = GetComponent<DamageController>();
    }

    private void FixedUpdate()
    {
        float maxHealth = associatedPlanetaryHealth.GetMaxHealth();
        float currentHealth = associatedPlanetaryHealth.GetHealth();
        damageController.damage = baseDamage + damageIncreasePerPercentHealthLoss * ((maxHealth - currentHealth) / maxHealth);
    }

    void GetAssociatedOrbitalData()
    {
        Collider[] hits = Physics.OverlapBox(transform.position, Vector3.one * 0.5f, Quaternion.identity, LayerMask.GetMask("BuildableArea"));
        //Debug.Log($"Name of hit collider: {hits[0].transform.name}");
        GameObject hitObject = hits[0].transform.parent.gameObject;
        associatedPlanetaryHealth = hitObject.GetComponent<PlanetaryHealth>();
    }
}
