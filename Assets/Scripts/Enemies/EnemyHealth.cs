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

    private bool died =false;

    public float GetHealth()
    {
        return health;
    }

    public float ChangeHealth(float change)
    {
        health += change;
        health = Mathf.Clamp(health, 0, maxHealth);

        if (health == 0 && !died)
        {
            //invoke event to track kill for defeated enemy income bonus
            died = true;
            OnEnemyKilled?.Invoke(defeatBonus);
            if(gameObject != null)
                Destroy(gameObject);
        }

        return 0f;
    }
}
