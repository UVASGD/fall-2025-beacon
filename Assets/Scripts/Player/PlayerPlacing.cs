using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlacing : MonoBehaviour
{
    public int selectedBuildingIndex = -1; //-1 means no building is selected
    public float cellSize = 10f;

    public GameObject placingIcon;
    private PlayerBuildings playerBuildings;
    private bool placingEnabled = true;

    private Transform planetReference; //the reference planet for the grid snap
    private Vector3 referenceGridDimensions; //the dimensions of the reference planet's grids
    private void Awake()
    {
        playerBuildings = GetComponent<PlayerBuildings>();
        BuildingBorderManager.onMouseHover += UpdateReferencePlanet;
    }

    private void Start()
    {
        WaveManager.Singleton.onWaveFinished += OnWaveStop;
        WaveManager.Singleton.onWaveStart += OnWaveStart;
        BuildableArea.areaHovering += ToggleHoveredBuildableArea;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            SetSelectedIndex(-1);
        }

        if (IsPlacing() && placingEnabled)
        {
            placingIcon.SetActive(true);
            HandlePlacing();
        }
        else
        {
            placingIcon.SetActive(false);
        }
    }
    private void UpdateReferencePlanet(Transform reference)
    {
        planetReference = reference;
        //Debug.Log($"origin reference set to {reference.gameObject.name}");
    }
    public void OnWaveStart()
    {
        placingEnabled = false;
    }

    public void OnWaveStop()
    {
        placingEnabled = true;
    }

    public bool IsPlacing()
    {
        return selectedBuildingIndex >= 0;
    }

    public LevelController ValidLevelController(Vector3 checkPos)
    {
        Collider[] hits = Physics.OverlapBox(checkPos, Vector3.one * cellSize / 2f, Quaternion.identity);
        foreach(Collider hit in hits)
        {
            LevelController levelController = hit.GetComponent<LevelController>();  
            if(levelController != null )
            {
                if(levelController.SameBuilding(playerBuildings.GetBuilding(selectedBuildingIndex)))
                    return levelController;
            }
        }
        return null;
    }

    private void ToggleHoveredBuildableArea(bool hovered, Transform passedTransform) //called by an event from the buildableArea script
    {
        hoveringBuildableArea = hovered;
        buildAreaTransform = passedTransform;
    }
    private bool hoveringBuildableArea = false;
    private Transform buildAreaTransform;
    public bool ValidPlacingSpot()
    {
        //return Physics.CheckBox(checkPos, Vector3.one * cellSize / 2f, Quaternion.identity, LayerMask.GetMask("BuildableArea")) && !Physics.CheckBox(checkPos, Vector3.one * cellSize / 2f, Quaternion.identity, LayerMask.GetMask("BlockBuilding"));
        return hoveringBuildableArea; //much cheaper computationally than using the Physics library
    }

    private void HandlePlacing()
    {
        Vector3 placePosition = GetClosestGridPositionToCursor();
        placingIcon.transform.position = placePosition;
        if (Input.GetMouseButtonDown(0))
        {
            LevelController possibleLevelController = null; //ValidLevelController(placePosition);
            if (possibleLevelController != null)
            {
                possibleLevelController.LevelUp();
                playerBuildings.RemoveAtIndex(selectedBuildingIndex);
                SetSelectedIndex(-1);
            }
            else if (ValidPlacingSpot())
            {
                Building toBuild = playerBuildings.GetBuilding(selectedBuildingIndex);

                //need to attach to a parent for construction on an orbital body. To do this, BuildableArea was refactored into a script that implements IPointerEnter/Exit that passes buildAreaTransform.
                Instantiate(toBuild.buildingPrefab, placePosition, Quaternion.identity, buildAreaTransform);
                playerBuildings.RemoveAtIndex(selectedBuildingIndex);
                SetSelectedIndex(-1);
            }
        }
    }

    const float relativeGridSize = 3f;
    private Vector3 GetClosestGridPositionToCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        groundPlane.Raycast(ray, out float distance);
        Vector3 hitPoint = ray.GetPoint(distance);
        hitPoint.y = 0;
        hitPoint.x *= 1 / cellSize; hitPoint.z *= 1 / cellSize;
        hitPoint.x = Mathf.RoundToInt(hitPoint.x); hitPoint.z = Mathf.RoundToInt(hitPoint.z);

        return (hitPoint * cellSize);
    }

    public void SetSelectedIndex(int setTo)
    {
        selectedBuildingIndex = setTo;
    }


}
