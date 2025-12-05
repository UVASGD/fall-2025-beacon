using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EarnedMoneyList : MonoBehaviour
{
    [Header("Lerp stuff")]
    [SerializeField] Vector2 originPosition;
    [SerializeField] Vector2 onScreenPosition;
    [SerializeField] float lerpTime;
    [SerializeField] AnimationCurve lerpCurve; //curve dictates the acceleration of the lerp

    [Header("Incomes")]
    [SerializeField] List<TMP_Text> incomeTypes;
    [SerializeField] float incomeDisplayDelay = 0.5f;
    [SerializeField] float endDisplayDelay = 2f;
    private List<int> earnedIncome = new List<int>();
    private int earnedIncomeTotal;
    [SerializeField] TMP_Text totalIncomeAmount;

    public IEnumerator EndTurnIncome(List<int> incomeTypes, int incomeTotal)
    {
        if (earnedIncome == null)
            earnedIncome = new List<int>();

        earnedIncome.Clear();
        earnedIncome = incomeTypes;
        earnedIncomeTotal = incomeTotal;

        yield return LerpOnscreen();

        yield return DisplayEarnedIncome();
        yield return new WaitForSeconds(incomeDisplayDelay);

        yield return LerpOffScreen();

        ClearUIValues(); //reset text values to 0 appropriately
    }

    private void ClearUIValues()
    {
        foreach (var text in incomeTypes)
        {
            text.text = "###";
        }
        totalIncomeAmount.text = "###";
    }

    private IEnumerator DisplayEarnedIncome()
    {
        if (earnedIncome.Count != incomeTypes.Count)
            yield break;

        //now iterate through each income category and add appropriately
        for (int i = 0; i < incomeTypes.Count; i++)
        {
            incomeTypes[i].text = earnedIncome[i].ToString();

            yield return new WaitForSeconds(incomeDisplayDelay); //wait 
        }

        totalIncomeAmount.text = earnedIncomeTotal.ToString();

        yield return new WaitForSeconds(endDisplayDelay / 2); //halved due to odd delay
    }

    private IEnumerator LerpOnscreen() //lerps the earnedMoney item onscreen
    {
        float elapsedTime = 0f;
        float elapsedPercentage;
        while (elapsedTime <= lerpTime)
        {
            elapsedTime += Time.deltaTime;
            elapsedPercentage = elapsedTime / lerpTime;

            float curveVal = lerpCurve.Evaluate(elapsedPercentage);

            transform.position = Vector2.Lerp(originPosition, onScreenPosition, curveVal);

            yield return null; //pass to the next frame of the lerp
        }

        Debug.Log("Lerp onscreen completed");
    }

    private IEnumerator LerpOffScreen() //the same as LerpOnscreen, but with the lerp function's arguments switched order
    {
        float elapsedTime = 0f;
        float elapsedPercentage;
        while (elapsedTime <= lerpTime)
        {
            elapsedTime += Time.deltaTime;
            elapsedPercentage = elapsedTime / lerpTime;

            float curveVal = lerpCurve.Evaluate(elapsedPercentage);

            transform.position = Vector2.Lerp(onScreenPosition, originPosition, curveVal);

            yield return null; //pass to the next frame of the lerp
        }

        Debug.Log("Lerp offscreen completed");
    }
}