using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    public bool[,] grid = new bool[9, 9];
    public bool showGrid = true;
    public float cellSize = 5f;
    public Color gridColor = Color.green;
    public Material lineMaterial;
    public float lineWidth = 0.05f;

    public void DisplayGrid()
    {

    }

    private Vector3 GetGridPositionFromBoolIndex(int x, int y)
    {
        Vector3 startingPosition = Vector3.right * -cellSize * 4 + Vector3.forward * cellSize * 4;
        return startingPosition + Vector3.right * x + Vector3.back * y;
    }

    private void DisplayCell(Vector3 worldPosition)
    {
        
    }
}
