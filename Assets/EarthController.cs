using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthController : MonoBehaviour
{
    public static EarthController Singleton;

    private void Awake()
    {
        Singleton = this;
    }


}
