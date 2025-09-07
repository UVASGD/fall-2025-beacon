using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IHealth
{
    [SerializeField]
    private float health = 100;
    [SerializeField]
    private float maxHealth = 100;
    public float GetHealth()
    {
        return health;
    }

    public void ChangeHealth(float change)
    {
        health += change;
        health = Mathf.Clamp(health, 0, maxHealth);

        if (health == 0)
        {
            Destroy(gameObject);
        }
    }
}
