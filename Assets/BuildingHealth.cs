using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHealth : MonoBehaviour, IHealth
{
    [SerializeField]
    private float health = 100;
    [SerializeField] 
    private float maxHealth = 100;
    private float baseMaxHealth;

    void Awake()
    {
        baseMaxHealth = maxHealth;
        ApplyRelicHp();

        if (RelicManager.Singleton != null)
            RelicManager.Singleton.OnRelicsChanged += ApplyRelicHp;
    }

    void OnDestroy()
    {
        if (RelicManager.Singleton != null)
            RelicManager.Singleton.OnRelicsChanged -= ApplyRelicHp;
    }

    private void ApplyRelicHp()
    {
        float oldMax = maxHealth;
        float ratio = (oldMax > 0f) ? (health / oldMax) : 1f;

        if (RelicManager.Singleton != null)
            maxHealth = RelicManager.Singleton.Apply(baseMaxHealth, StatType.MaxHealth);
        else
            maxHealth = baseMaxHealth;

        health = Mathf.Clamp(maxHealth * ratio, 0f, maxHealth);
    }

    public float GetHealth() => health;

    public void ChangeHealth(float delta)
    {
        health = Mathf.Clamp(health + delta, 0, maxHealth);
    }
}
