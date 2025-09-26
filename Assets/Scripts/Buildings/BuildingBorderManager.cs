using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBorderManager : MonoBehaviour
{
    public List<GameObject> borderPieces = new List<GameObject>();

    private void Start()
    {
        WaveManager.Singleton.onWaveFinished += OnWaveEnd;
        WaveManager.Singleton.onWaveStart += OnWaveStart;   
    }

    void OnWaveEnd()
    {
        foreach (var piece in borderPieces)
        {
            piece.gameObject.SetActive(true);
        }
    }

    void OnWaveStart()
    {
        foreach (var piece in borderPieces)
        {
            piece.gameObject.SetActive(false);
        }
    }
}
