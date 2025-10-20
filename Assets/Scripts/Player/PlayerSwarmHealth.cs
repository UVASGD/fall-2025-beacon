using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwarmHealth : MonoBehaviour, IHealth
{
    [SerializeField]
    private float health = 100;
    [SerializeField]
    private float maxHealth = 100;

    private CarrierBuffController carrierBuffController;

    void Awake()
    {
        if(GetComponent<CarrierBuffController>() != null)
            carrierBuffController = GetComponent<CarrierBuffController>();
    }

    public float GetHealth()
    {
        return health;
    }

    public float GetMaxHealth() { 
        return maxHealth;
    }

    public void ChangeHealth(float change)
    {
        if(carrierBuffController != null && carrierBuffController.CarrierInRange())
        {
            if(change < 0)
            {
                change *= (1f - carrierBuffController.damageReduction);
            }
        }

        health += change;
        health = Mathf.Clamp(health, 0, maxHealth);

        if(health == 0)
        {
            Destroy(gameObject);
        }
    }

    public void InitializeHealth(float _maxHealth)
    {
        maxHealth = _maxHealth;
        health = _maxHealth;
    }
}