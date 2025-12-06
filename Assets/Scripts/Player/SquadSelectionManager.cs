using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SquadSelectionManager : MonoBehaviour
{
    [SerializeField] TMP_Text selectionText;
    private int squadCount => squadrons.Count;
    private List<HumanController> squadrons;
    public static SquadSelectionManager Singleton;
    void Awake()
    {
        if (Singleton != null)
        {
            Singleton = this;
        }
        squadrons = new List<HumanController>();

        SelectionSquare.onObjectSelect += SelectShips;
    }
    private void SelectShips(List<HumanController> ships)
    {
        squadrons = ships;
        UpdateSelectionText(true);
    }
    private void UpdateSelectionText(bool activate)
    {
        selectionText.text = $"{squadCount} squadrons selected";
        selectionText.gameObject.SetActive(activate);
    }
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            //if squadrons is not empty, set their targets to the worldspace position
            if (squadrons.Count > 0)
            {
                Vector3 mousePos = GetMouseWorldPositionXZ();

                foreach (var member in squadrons)
                {
                    member.connectedBeacon.transform.position = mousePos;
                }

                squadrons.Clear();
                UpdateSelectionText(false);
            }
        }
    }
    Vector3 GetMouseWorldPositionXZ()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        groundPlane.Raycast(ray, out float distance);
        return ray.GetPoint(distance);
    }

}
