using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetIncome : MonoBehaviour
{
    [SerializeField] private float basePlanetIncome;
    [SerializeField] private float taxRate;
    public float BasePlanetIncome => basePlanetIncome;
    public float TaxRate => taxRate;
}