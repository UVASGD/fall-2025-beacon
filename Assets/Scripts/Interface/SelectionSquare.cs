using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionSquare : MonoBehaviour
{
    [SerializeField] private RectTransform selectionBox;
    private Vector2 startPos;
    private Vector2 endPos;

    void Update()
    {
        if(WaveManager.Singleton.waveState == WaveState.Inactive) return; //don't continue if the wave is inactive
        // Mouse pressed → anchor the start
        if (Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
            selectionBox.gameObject.SetActive(true);
        }

        // Mouse held → update the box
        if (Input.GetMouseButton(0))
        {
            endPos = Input.mousePosition;
            UpdateBox();
        }

        // Mouse released → finalize selection
        if (Input.GetMouseButtonUp(0))
        {
            SelectObjects();
            selectionBox.gameObject.SetActive(false);
        }
    }

    private void UpdateBox()
    {
        Vector2 boxStart = startPos;
        Vector2 boxEnd   = endPos;

        // Create the rectangle size
        Vector2 size = new Vector2(
            Mathf.Abs(boxEnd.x - boxStart.x),
            Mathf.Abs(boxEnd.y - boxStart.y));

        // Put anchor at the min corner
        Vector2 pivot = new Vector2(
            boxEnd.x < boxStart.x ? 1 : 0,
            boxEnd.y < boxStart.y ? 1 : 0);

        selectionBox.pivot = pivot;
        selectionBox.position = boxStart;
        selectionBox.sizeDelta = size;
    }

    private void SelectObjects()
    {
        Rect rect = GetScreenRect(startPos, endPos);

        // insert selection code here
    }

    private Rect GetScreenRect(Vector2 p1, Vector2 p2)
    {
        // Creates a proper rect regardless of drag direction
        return new Rect(
            Mathf.Min(p1.x, p2.x),
            Mathf.Min(p1.y, p2.y),
            Mathf.Abs(p1.x - p2.x),
            Mathf.Abs(p1.y - p2.y));
    }
}