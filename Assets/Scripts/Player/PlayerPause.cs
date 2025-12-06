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
            Debug.Log("Unpausing from space bar");
            pausedIndicator.SetActive(false);
            onPlayerPause.Invoke(false);
        }
        else
        {
            Debug.Log("Pausing the game");
            pausedIndicator.SetActive(true);
            onPlayerPause.Invoke(true);
        }
    }
}
