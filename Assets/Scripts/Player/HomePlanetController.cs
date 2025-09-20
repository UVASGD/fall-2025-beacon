using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomePlanetController : MonoBehaviour
{
    [SerializeField] Planet planetType;
    public static HomePlanetController i;
    [SerializeField] SpriteRenderer planetaryGraphics;
    [SerializeField] PlanetaryHealth planetaryHealth;
    [SerializeField] PlanetIncome planetIncome;

    private void Awake()
    {
        i = this;
    }

    private void Start()
    {
        InitializeModularPlanet();
    }
    public void SetHomePlanet(Planet planet)
    {
        planetType = planet;
    }
    private void InitializeModularPlanet()
    {
        planetaryGraphics.sprite = planetType.BasePlanetSprites[0]; //defaulting to zero as the undamaged state.
        planetaryHealth.SetMaxHealth(planetType.MaxPlanetHealth);
        planetaryHealth.TopOffHealth();
    }
}