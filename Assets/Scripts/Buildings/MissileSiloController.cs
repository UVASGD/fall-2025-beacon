using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSiloController : MonoBehaviour, ITurret
{
    public float fireRate = 0.1f;



    public float GetFireRate()
    {
        return fireRate;
    }
}
