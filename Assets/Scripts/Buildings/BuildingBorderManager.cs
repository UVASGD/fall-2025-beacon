using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBorderManager : MonoBehaviour
{
    [SerializeField] GameObject borderPiece;
    public static event Action<Transform> onMouseHover;
    private void Start()
    {
        WaveManager.Singleton.onWaveFinished += OnWaveEnd;
        WaveManager.Singleton.onWaveStart += OnWaveStart;
    }

    void OnWaveEnd()
    {
        borderPiece.SetActive(true);
    }

    void OnWaveStart()
    {
        borderPiece.SetActive(false);
    }
    void OnMouseEnter()
    {
        onMouseHover.Invoke(gameObject.transform);
    }

    private void OnDestroy()
    {
        WaveManager.Singleton.onWaveFinished -= OnWaveEnd;
        WaveManager.Singleton.onWaveStart -= OnWaveStart;
    }
}