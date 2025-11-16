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

    public void TogglePause()
    {
        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
            pausedIndicator.SetActive(false);
        }
        else
        {
            Time.timeScale = 0f;
            pausedIndicator.SetActive(true);
        }
    }
}
