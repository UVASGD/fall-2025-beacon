using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IHealth
{
    public static event Action<int> OnEnemyKilled;

    [SerializeField] private float health = 100;
    [SerializeField] private float maxHealth = 100;
    [SerializeField] int defeatBonus; //income gained from defeating this enemy
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
            //invoke event to track kill for defeated enemy income bonus
            OnEnemyKilled?.Invoke(defeatBonus);
            Destroy(gameObject);
        }
    }
}
