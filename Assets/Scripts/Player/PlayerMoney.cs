using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMoney : MonoBehaviour
{
    public static PlayerMoney Instance;

    [SerializeField] private int money;
    public TMP_Text moneyText;

    [SerializeField] int baseIncome;
    [SerializeField] List<PlanetIncome> taxedPlanets; //this is intentionally left as a list in case we have multiple planets for some reason
    private int defeatedEnemyIncome = 0;
    [SerializeField] EarnedMoneyList earnedMoneyUI;
    private int totalIncome = 0;

    private void Start()
    {
        Instance = this;
        WaveManager.Singleton.onWaveFinished += OnWaveEnd;
        //EnemyHealth.OnEnemyKilled += EnemyDefeatedBonus;
        UpdateText();
    }
    
    private void EnemyDefeatedBonus(int earnedAmount)
    {
        defeatedEnemyIncome += earnedAmount;
    }
    void UpdateText()
    {
        moneyText.text = $"Money: ${money}";
    }

    void OnWaveEnd()
    {
        StartCoroutine(CalculateRevenue());   
    }

    private IEnumerator CalculateRevenue() //calculations for end-of-wave revenue. This includes the base income, ships destroyed, a multiplier bonus depending on damage taken, etc.
    {
        int earnedIncome = 0;
        int fromMining = GetMiningMoney();

        earnedIncome += baseIncome;
        earnedIncome += fromMining;
        totalIncome += earnedIncome;

        List<int> incomeSources = new List<int>();
        incomeSources.Add(baseIncome);
        incomeSources.Add(fromMining); //mining money

        ChangeMoney(earnedIncome);

        yield return earnedMoneyUI.EndTurnIncome(incomeSources, earnedIncome);

        defeatedEnemyIncome = 0;
    }

    private int GetMiningMoney()
    {
        int total = 0;
        foreach(var mine in MineController.mineControllers)
        {
            total += mine.GetMiningMoney();
        }
        foreach(var cracker in PlanetCracker.crackerControllers)
        {
            total += cracker.GetCrackingMoney();
        }
        return total;
    }

    public int GetMoney()
    {
        return money;
    }

    public bool ChangeMoney(int change)
    {
        if(change > 0)
        {
            money += change;
            UpdateText();
            return true;
        }

        if(money >= change)
        {
            money += change;
            UpdateText();
            return true;
        }
        else
        {
            return false;
        }
    }

    public int GetTotalIncome()
    {
        return totalIncome;
    }
}