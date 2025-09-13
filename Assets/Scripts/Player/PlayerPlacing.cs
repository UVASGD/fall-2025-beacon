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

    private void Awake()
    {
        playerBuildings = GetComponent<PlayerBuildings>();
    }

    private void Start()
    {
        WaveManager.Singleton.onWaveFinished += OnWaveStop;
        WaveManager.Singleton.onWaveStart += OnWaveStart;
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

    public bool ValidPlacingSpot(Vector3 checkPos)
    {
        return Physics.CheckBox(checkPos, Vector3.one * cellSize / 2f, Quaternion.identity, LayerMask.GetMask("BuildableArea")) && !Physics.CheckBox(checkPos, Vector3.one * cellSize / 2f, Quaternion.identity, LayerMask.GetMask("BlockBuilding"));
    }

    private void HandlePlacing()
    {
        Vector3 placePosition = GetClosestGridPositionToCursor();
        placingIcon.transform.position = placePosition;
        if (Input.GetMouseButtonDown(0))
        {
            LevelController possibleLevelController = ValidLevelController(placePosition);
            if (possibleLevelController != null)
            {
                possibleLevelController.LevelUp();
                playerBuildings.RemoveAtIndex(selectedBuildingIndex);
                SetSelectedIndex(-1);
            }
            else if (ValidPlacingSpot(placePosition))
            {
                Building toBuild = playerBuildings.GetBuilding(selectedBuildingIndex);
                Instantiate(toBuild.buildingPrefab, placePosition, Quaternion.identity);
                playerBuildings.RemoveAtIndex(selectedBuildingIndex);
                SetSelectedIndex(-1);
            }
        }
    }

    private Vector3 GetClosestGridPositionToCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        groundPlane.Raycast(ray, out float distance);
        Vector3 hitPoint = ray.GetPoint(distance);
        hitPoint.y = 0;
        hitPoint.x *= 1 / cellSize; hitPoint.z *= 1 / cellSize;
        hitPoint.x = Mathf.RoundToInt(hitPoint.x); hitPoint.z = Mathf.RoundToInt(hitPoint.z);
        return hitPoint * cellSize;
    }

    public void SetSelectedIndex(int setTo)
    {
        selectedBuildingIndex = setTo;
    }


}
