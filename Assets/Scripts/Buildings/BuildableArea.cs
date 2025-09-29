using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildableArea : MonoBehaviour
{
    public static Action<bool, Transform> areaHovering; //I feel smart for this one


    void OnMouseOver()
    {
        Debug.Log("Hovered buildable area");
        areaHovering.Invoke(true, gameObject.transform);
    }

    void OnMouseExit()
    {
        Debug.Log("Left buildable area");
        areaHovering.Invoke(false, gameObject.transform);
    }
}