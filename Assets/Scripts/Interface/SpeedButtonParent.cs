using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedButtonParent : MonoBehaviour
{
    [SerializeField] List<Image> buttonBGImages;

    [SerializeField] Color highlightedColor;
    [SerializeField] Color notHighlighted;
    private void Awake()
    {
        TimeScaleEditor.onTimescaleChange += UpdateBGColors;
        PlayerPause.onPlayerPause += CatchPlayerPause;
        UpdateBGColors(1);
    }
    private void CatchPlayerPause(bool pausing)
    {
        if (pausing)
        {
            GlobalSettings.i.AdjustTimeScale(0);
            UpdateBGColors(0);
        }
        else
        {
            GlobalSettings.i.AdjustTimeScale(1);
            UpdateBGColors(1);
        }
    }
    private void UpdateBGColors(int index)
    {
        for(int i = 0; i < buttonBGImages.Count; i++)
        {
            if(i == index)
            {
                buttonBGImages[i].color = highlightedColor;
            }
            else
            {
                buttonBGImages[i].color = notHighlighted;
            }
        }
    }
}
