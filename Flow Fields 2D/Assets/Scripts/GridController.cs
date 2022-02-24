using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GridController : MonoBehaviour
{
    // Variables
    public Vector2Int gridSize; // Modifies grid size in-editor
    public float cellRadius = 0.5f; // Default to 1 unity wide
    public FlowField currentFlowField;
    public Vector2 startPosition;
    private bool showCost = false;

    private void InitializeFlowField()
    {
        currentFlowField = new FlowField(cellRadius, gridSize, startPosition);
        currentFlowField.CreateGrid();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            InitializeFlowField();
            currentFlowField.CreateCostField();
        }
    }

    // Debug
    private void OnDrawGizmos()
    {
        if (currentFlowField == null)
        {
            DrawGrid(Color.red);
        }
        else
        {
            DrawGrid(Color.green);
            ShowCost();
        }
    }

    private void DrawGrid(Color drawColorP)
    {
        // Variables
        Vector3 drawSize, drawCenter;

        Gizmos.color = drawColorP;

        drawSize = new Vector3(cellRadius * 2, cellRadius * 2, cellRadius * 2);

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                drawCenter = new Vector3((cellRadius * 2) * x + cellRadius, (cellRadius * 2) * y + cellRadius, 0);
                Gizmos.DrawWireCube(drawCenter, drawSize);
            }
        }
    }

    private void ShowCost()
    {
        // Variables
        GUIStyle style;

        style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleCenter;

        foreach(GridCell cell in currentFlowField.grid)
        {
            Handles.Label(cell.worldPosition, cell.cost.ToString(), style);
        }
    }
}
