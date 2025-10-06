using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Planet/AsteroidCaptureChance")]
public class CaptureChance : ScriptableObject
{
    //Capture Chance is the configuration for a planet, which determines the statistics of capturing a planet randomly at the end of each turn/
    //This scriptableObject is a SerializedField in the OrbitHandler, which onTurnEnd calculates the chance of capturing an asteroid for use.

    [SerializeField] float meanCaptureChance;
    [SerializeField] float captureDeviation;
    [SerializeField] float meanBodySize;
    [SerializeField] float sizeDeviation;

    //deviation fields are the size of one standard deviation in asteroid size. I have never implemented actual bell curves in a game before, so this is a first.
    //after some research, a Gaussian Distribution seems the most appropriate way to implement a bell curve. This will be implemented in the OrbitHandler script.

    //getters
    public float MeanCaptureChance => meanCaptureChance;
    public float CaptureDeviation => captureDeviation;
    public float MeanBodySize => meanBodySize;
    public float SizeDeviation => sizeDeviation;
}