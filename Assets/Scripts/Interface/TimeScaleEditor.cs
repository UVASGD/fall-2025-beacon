using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TimeScaleEditor : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] float targetTimescale;
    [SerializeField] int buttonIndex;
    public static event Action<int> onTimescaleChange;
    public void OnPointerClick(PointerEventData eventData)
    {
        onTimescaleChange.Invoke(buttonIndex);
        GlobalSettings.i.AdjustTimeScale(targetTimescale);
    }
}