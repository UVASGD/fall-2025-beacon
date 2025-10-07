using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BodyInfo : MonoBehaviour
{
    //body info stores the data pertaining to a celestial body. It displays its name, ore content, and other flavor info.
    [SerializeField] List<Text> dataTexts; //0 is name, 1 is population, 2 is ore content
    [SerializeField] Image planetIcon;
    [SerializeField] GameObject mainParent; //toggles on and off if opened
}
