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
    }

    [SerializeField] private Color hitFlashColor;
    public Color HitFlashColor => hitFlashColor;
}
