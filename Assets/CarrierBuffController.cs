using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script for player ships that are buffed by nearby carriers
public class CarrierBuffController : MonoBehaviour
{
    const float baseBuffRange = 5f;

    public float damageReduction = 0.1f;
    public float damageBuff = 0.1f;
    bool carrierInRange;

    private void FixedUpdate()
    {
        UpdateCarrierInRange();
    }

    void UpdateCarrierInRange()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, baseBuffRange, LayerMask.GetMask("PlayerSwarm"));
        bool carrierHit = false;
        foreach (var hit in hits)
        {
            if (hit.transform.name.Contains("Carrier"))
            {
                carrierHit = true;
            }
        }

        carrierInRange = carrierHit;
    }

    public bool CarrierInRange()
    {
        return carrierInRange;
    }
}
