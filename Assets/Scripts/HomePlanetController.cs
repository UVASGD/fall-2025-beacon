using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomePlanetController : MonoBehaviour
{
    [SerializeField] Planet planetType;
    public static HomePlanetController Singleton;
    [SerializeField] SpriteRenderer planetaryGraphics;
    [SerializeField] PlanetaryHealth planetaryHealth;

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        InitializeModularPlanet();
    }

    private void InitializeModularPlanet()
    {
        planetaryGraphics.sprite = planetType.BasePlanetSprites[0]; //defaulting to zero as the undamaged state.
        planetaryHealth.SetMaxHealth(planetType.MaxPlanetHealth);
        planetaryHealth.TopOffHealth();
    }
}