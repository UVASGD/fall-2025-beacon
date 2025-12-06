using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    public static GlobalSettings i;
    void Awake()
    {
        if (i == null)
        {
            i = this;
        }

        AdjustTimeScale(1f); //timescale should be at the start
    }

    [SerializeField] private Color hitFlashColor;
    public Color HitFlashColor => hitFlashColor;

    [SerializeField] private Color shieldHitColor;
    public Color ShieldHitColor => shieldHitColor;

    [SerializeField] float timeScale;
    public float TimeScale => timeScale;

    public void AdjustTimeScale(float newTimeScale)
    {
        timeScale = newTimeScale;

        Debug.Log("Timescale adjusted to " + newTimeScale);
    }
}