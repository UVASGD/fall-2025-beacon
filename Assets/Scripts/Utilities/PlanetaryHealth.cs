using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetaryHealth : MonoBehaviour, IHealth
{
    public static List<PlanetaryHealth> planetaryHealths = new List<PlanetaryHealth>();

    [SerializeField] private float health = 100;
    [SerializeField] private float maxHealth = 100;
    public Bar bar;

    [Header("Planetary Hit Flash")]
    [SerializeField] private SpriteRenderer planetRenderer; //so the planet can hitFlash red on impact

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
    }

    public float GetHealth()
    {
        return health;
    }

    public void ChangeHealth(float change)
    {
        health += change;
        health = Mathf.Clamp(health, 0, maxHealth);
        if (bar != null)
            bar.SetValue(health);

        if (health <= 0 && GetComponent<HomePlanetController>() == null)
        {
            Destroy(gameObject);
        }

        if(change < 0 && planetRenderer != null) //null check prevents exception if not assigned
        {
            //perform a damage hitflash
            StartCoroutine(ColorFlash(GlobalSettings.i.HitFlashColor));
        }
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