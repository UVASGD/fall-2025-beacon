using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBorderManager : MonoBehaviour
{
    [SerializeField] GameObject borderPiece;

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
}