using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetaryHealth : MonoBehaviour, IHealth
{
    [SerializeField] private float health = 100;
    [SerializeField] private float maxHealth = 100;
    public Bar bar;

    void Start()
    {
        bar.SetMaxValue(maxHealth);
        bar.SetValue(health);
    }

    public float GetHealth()
    {
        return health;
    }

    public void ChangeHealth(float change)
    {
        health += change;
        health = Mathf.Clamp(health, 0, maxHealth);
        bar.SetValue(health);
    }

    public void SetMaxHealth(int newMaxHealth) //sets the max health of a PlanetaryHealth. Accessed by a homePlanetController when instantiating a new planet.
    {
        maxHealth = newMaxHealth;
    }

    public void TopOffHealth() //tops off the health of a PlanetaryHealth to its max. Useful when instantiating, as well as any possible fullheals throughout the game's flow.
    {
        health = maxHealth;
    }
}
