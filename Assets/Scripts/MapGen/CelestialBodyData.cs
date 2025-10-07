using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CelestialBodyData
{
    [SerializeField] string bodyName;
    [SerializeField] string population; //for now, purely a flavor stat for the body menu with no gameplay use
    [SerializeField] int baseOreContent;
    [SerializeField] Sprite planetIcon;

    public string BodyName => bodyName;
    public string Population => population;
    public int BaseOreContent => baseOreContent;
    public Sprite PlanetIcon => planetIcon;
}