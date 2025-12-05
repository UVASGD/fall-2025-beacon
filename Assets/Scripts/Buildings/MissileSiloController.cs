using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSiloController : MonoBehaviour
{
    public GameObject missilePrefab;
    public Vector2 minMaxTimeBetweenSpawns; //Pick a random time between these two floats. This is how long the silo waits before it automatically launches its next missile.
    public Transform launchPoint;

    private float timeToNextLaunch = 0f;
    private bool waveStarted = false;

    private void Awake()
    {
        WaveManager.Singleton.onWaveStart += OnWaveStart;
        WaveManager.Singleton.onWaveFinished += OnWaveEnd;
    }

    private void FixedUpdate()
    {
        if (waveStarted)
        {
            LaunchFixedUpdateLogic();
        }
    }

    void LaunchFixedUpdateLogic()
    {
        if (timeToNextLaunch > 0f)
        {
            timeToNextLaunch -= Time.fixedDeltaTime * GlobalSettings.i.TimeScale;
        }
        else
        {
            Instantiate(missilePrefab, launchPoint.position, Quaternion.identity);
            timeToNextLaunch = GetRandomLaunchTime();
        }
    }

    void OnWaveStart()
    {
        waveStarted = true;
        timeToNextLaunch = GetRandomLaunchTime();
    }

    void OnWaveEnd()
    {
        waveStarted = false;
    }

    float GetRandomLaunchTime()
    {
        return Random.Range(minMaxTimeBetweenSpawns.x, minMaxTimeBetweenSpawns.y);
    }

    private void OnDestroy()
    {
        if (WaveManager.Singleton != null)
        {
            WaveManager.Singleton.onWaveStart -= OnWaveStart;
            WaveManager.Singleton.onWaveFinished -= OnWaveEnd;
        }
    }
}
