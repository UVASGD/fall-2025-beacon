using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetaryHealth : MonoBehaviour, IHealth
{
    public static List<PlanetaryHealth> planetaryHealths = new List<PlanetaryHealth>();

    [SerializeField] private float health = 100;
    [SerializeField] private float maxHealth = 100;
    private float shield = 0;
    public Bar bar;

    [Header("Planetary Hit Flash")]
    [SerializeField] private SpriteRenderer planetRenderer; //so the planet can hitFlash red on impact
    private bool useWorldspaceBar = false;
    private WorldSpaceHealthbar worldBar;
    
    private int healingBuildings = 0;
    private int shieldBuildings = 0;
    private int mineLayingBuidlings = 0;

    private int mines = 0;

    private InfoToDisplayController mInfoToDisplayController;

    void Awake()
    {
        planetaryHealths.Add(this);
    }

    void Start()
    {
        if (bar != null)
        {
            bar.SetMaxValue(maxHealth);
            bar.SetValue(health);
        }
        mInfoToDisplayController = GetComponentInChildren<InfoToDisplayController>();

        WaveManager.Singleton.onWaveFinished += OnWaveFinished;
        WaveManager.Singleton.onWaveStart += OnWaveStart;   
    }

    void FixedUpdate()
    {
        string toDisplay = $"Health: {Mathf.RoundToInt(health)}/{maxHealth}";
        if (MaxShields() > 0)
            toDisplay += $"\nShield: {Mathf.RoundToInt(shield)}/{MaxShields()}";
        if (MaxMines() > 0)
            toDisplay += $"\nMines: {mines}/{MaxMines()}";
        mInfoToDisplayController.infoText = toDisplay;
    }

    public static int GetAndResetKilledPlanets()
    {
        int toReturn = killedPlanets;
        killedPlanets = 0;
        return toReturn;
    }

    public void OnWaveFinished()
    {

    }

    public void OnWaveStart()
    {
        ChangeHealth(15f * healingBuildings);
        SetShield(MaxShields());
        AddMines(5 * mineLayingBuidlings);
    }

    public void AddHealingBuilding()
    {
        healingBuildings++; 
    }

    public void AddShieldBuilding()
    {
        shieldBuildings++;
    }

    public void AddMineLayingBuilding()
    {
        mineLayingBuidlings++;
    }

    private int MaxShields()
    {
        return 60 * shieldBuildings;
    }

    private int MaxMines()
    {
        return mineLayingBuidlings * 10;
    }

    public void AddMines(int toAdd)
    {
        mines += toAdd;
        mines = Mathf.Clamp(mines, 0, MaxMines());
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetHealth()
    {
        return health;
    }

    public void SetShield(float setTo)
    {
        shield = setTo; 
    }

    public float ChangeHealth(float change)
    {
        //Reduce negative change by shield amount
        if (change < 0)
        {
            if (planetRenderer != null)
                StartCoroutine(ColorFlash(GlobalSettings.i.HitFlashColor));
            float reducedDamage = Mathf.Min(-change, shield);
            shield -= reducedDamage;
            change += reducedDamage;
        }

        health += change;
        health = Mathf.Clamp(health, 0, maxHealth);
        if (bar != null)
            bar.SetValue(health);

        if (health <= 0 && GetComponent<HomePlanetController>() == null)
        {
            Destroy(gameObject);
        }

        float returnDamage = 0f;
        if (mines > 0)
        {
            mines--;
            returnDamage += 30f; //Damage of mines
        }

        return returnDamage;
    }

        if(change < 0 && planetRenderer != null) //null check prevents exception if not assigned
        {
            //perform a damage hitflash
            StartCoroutine(ColorFlash(GlobalSettings.i.HitFlashColor));
        }

        if (useWorldspaceBar)
        {
            worldBar.UpdateFill(health / maxHealth);
        }
    }
    public void setWorldspaceBar(WorldSpaceHealthbar bar)
    {
        worldBar = bar;
        useWorldspaceBar = true;
    }
    public void SetMaxHealth(int newMaxHealth) //sets the max health of a PlanetaryHealth. Accessed by a homePlanetController when instantiating a new planet.
    {
        maxHealth = newMaxHealth;
    }

    public void TopOffHealth() //tops off the health of a PlanetaryHealth to its max. Useful when instantiating, as well as any possible fullheals throughout the game's flow.
    {
        health = maxHealth;
    }

    public static PlanetaryHealth GetClosestPlanetaryHealth(Vector3 position)
    {
        PlanetaryHealth closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (var health in planetaryHealths)
        {
            float distance = Vector3.Distance(position, health.transform.position);
            if (distance < closestDistance)
            {
                closest = health;
                closestDistance = distance;
            }
        }

        return closest;
    }

    void OnDestroy()
    {
        planetaryHealths.Remove(this);
        OrbitHandler.Instance.RemoveOrbitalData(this.gameObject);
        WaveManager.Singleton.onWaveFinished -= OnWaveFinished;
        WaveManager.Singleton.onWaveStart -= OnWaveStart;

        if(GetComponent<HomePlanetController>() != null)
        {
            killedPlanets++;
        }
    }

    private IEnumerator ColorFlash(Color flashColor)
    {
        planetRenderer.color = flashColor;
        yield return new WaitForSeconds(0.25f);
        planetRenderer.color = Color.white;
        yield return new WaitForSeconds(0.25f);
        planetRenderer.color = flashColor;
        yield return new WaitForSeconds(0.25f);
        planetRenderer.color = Color.white;
    }

    public void SetRenderer(SpriteRenderer renderer)
    {
        planetRenderer = renderer;
    }
}