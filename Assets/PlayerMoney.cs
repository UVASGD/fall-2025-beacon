using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMoney : MonoBehaviour
{
    [SerializeField]    
    private int money;

    public int moneyAtWaveEnd = 12;
    public TMP_Text moneyText;

    private void Start()
    {
        WaveManager.Singleton.onWaveFinished += OnWaveEnd;
        UpdateText();
    }

    void UpdateText()
    {
        moneyText.text = $"Money: ${money}";
    }

    void OnWaveEnd()
    {
        ChangeMoney(moneyAtWaveEnd);
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
}
