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

    private bool _initialized;

    private void Awake()
    {
        i = this;
        if (planetaryGraphics == null) planetaryGraphics = GetComponentInChildren<SpriteRenderer>(true);
        if (planetaryHealth == null) planetaryHealth = GetComponentInChildren<PlanetaryHealth>(true);
        if (planetIncome == null) planetIncome = GetComponentInChildren<PlanetIncome>(true);

    }

    private void Start()
    {
        TryInitializeModularPlanet();
    }
    public void SetHomePlanet(Planet planet)
    {
        planetType = planet;
        TryInitializeModularPlanet();
    }

    private void TryInitializeModularPlanet(){
        if (_initialized) return;
        if (planetType == null) {
            Debug.LogWarning($"{nameof(HomePlanetController)}: Planet type not set. Cannot initialize modular planet.");
            return;
        }
        if (planetType.BasePlanetSprites == null ||
            planetType.BasePlanetSprites.Count == 0 ||
            planetType.BasePlanetSprites[0] == null)
        { Debug.LogWarning($"{nameof(HomePlanetController)}: Planet type has no valid base sprites. Cannot initialize modular planet."); return; 
        }
        planetaryGraphics.sprite = planetType.BasePlanetSprites[0];
        planetaryHealth.SetMaxHealth(planetType.MaxPlanetHealth);
        planetaryHealth.TopOffHealth();
        _initialized = true;
        }

    //private void InitializeModularPlanet()
    //{
    //    planetaryGraphics.sprite = planetType.BasePlanetSprites[0]; //defaulting to zero as the undamaged state.
    //    planetaryHealth.SetMaxHealth(planetType.MaxPlanetHealth);
    //    planetaryHealth.TopOffHealth();
    //}

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Help catch setup issues immediately in the Editor
        if (planetaryGraphics == null) planetaryGraphics = GetComponentInChildren<SpriteRenderer>(true);
        if (planetaryHealth == null) planetaryHealth = GetComponentInChildren<PlanetaryHealth>(true);
        if (planetIncome == null) planetIncome = GetComponentInChildren<PlanetIncome>(true);
    }
#endif
}