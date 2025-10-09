using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildableArea : MonoBehaviour
{
    public static Action<bool, Transform> areaHovering; //I feel smart for this one


    void OnMouseOver()
    {
        areaHovering.Invoke(true, gameObject.transform);
    }

    void OnMouseExit()
    {
        areaHovering.Invoke(false, gameObject.transform);
    }
}