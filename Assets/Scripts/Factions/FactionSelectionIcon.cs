using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FactionSelectionIcon : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    //static event declares to FactionManager the currently selected faction in the menu
    public static event Action<int> onFactionSelectionClick;

    public TMP_Text factionTitle;
    public Image factionImage;
    public TMP_Text overviewText;
    public int factionId = -1; //-1 will fail data validation in FactionManager, so make sure every FactionBase has an appropriate factionID

    [SerializeField] Image backgroundImage;
    [SerializeField] Color highlightedColor;
    [SerializeField] Color unhighlightedColor;
    public void OnPointerClick(PointerEventData eventData)
    {
        onFactionSelectionClick?.Invoke(factionId); //invoke the onFactionSelectoinClick with the passed parameter of the factionID
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //enable a highlight
        backgroundImage.color = highlightedColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //disable the highlight on exit
        backgroundImage.color = unhighlightedColor;
    }
}
