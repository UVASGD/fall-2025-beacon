using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPause : MonoBehaviour
{
    public KeyCode pauseKeybind;
    public GameObject pausedIndicator;

    private void Update()
    {
        if (Input.GetKeyDown(pauseKeybind))
        {
            TogglePause();
        }
    }

    public static event Action<bool> onPlayerPause;
    private bool currentlyPaused => GlobalSettings.i.TimeScale == 0;
    public void TogglePause()
    {
        if (currentlyPaused)
        {
            pausedIndicator.SetActive(false);
            onPlayerPause.Invoke(false);
        }
        else
        {
            pausedIndicator.SetActive(true);
            onPlayerPause.Invoke(true);
        }
    }
}