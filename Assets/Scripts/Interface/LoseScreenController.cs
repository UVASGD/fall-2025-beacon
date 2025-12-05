using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoseScreenController : MonoBehaviour
{
    public TMP_Text wavesSurvivedText;
    public TMP_Text moneyGenText;
    public TMP_Text asteriodsLost;

    private void OnEnable()
    {
        wavesSurvivedText.text = $"Waves Survived:\n{GetRoundsSurvived()}";
        moneyGenText.text = $"Money Generated:\n{GetMoneyGenerated()}";
        asteriodsLost.text = $"Asteriods Lost:\n{GetAsteriodsLost()}";
    }

    public int GetRoundsSurvived()
    {
        return WaveManager.Singleton.GetFinishedWaveCount();
    }

    public int GetMoneyGenerated()
    {
        return PlayerMoney.Instance.GetTotalIncome();
    }

    public int GetAsteriodsLost()
    {
        return PlanetaryHealth.GetAndResetKilledPlanets();
    }
}
