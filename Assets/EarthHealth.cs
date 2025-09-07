using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthHealth : MonoBehaviour, IHealth
{
    [SerializeField]
    private float health = 100;
    [SerializeField]
    private float maxHealth = 100;
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
}
