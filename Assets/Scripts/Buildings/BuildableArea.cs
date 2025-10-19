using System;
using UnityEngine;

public class BuildableArea : MonoBehaviour
{
    public static Action<bool, Transform> areaHovering;


    void OnMouseOver()
    {
        areaHovering.Invoke(true, gameObject.transform);
    }

    void OnMouseExit()
    {
        areaHovering.Invoke(false, gameObject.transform);
    }
}