using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bar : MonoBehaviour
{
    public RectTransform backgroundBar;
    public RectTransform actualBar;

    [SerializeField]
    private float value = 100f;
    [SerializeField]
    private float maxValue = 100f;

    public void SetMaxValue(float _maxValue)
    {
        maxValue = _maxValue;
    }

    public void SetValue(float _value)
    {
        value = _value;
        float percentage = (1-value / maxValue);
        Vector2 newOffsetMax = actualBar.offsetMax;
        newOffsetMax.x = -percentage * backgroundBar.rect.width;
        actualBar.offsetMax = newOffsetMax;
    }
}
