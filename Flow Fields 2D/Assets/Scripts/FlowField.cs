using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowField
{
    // Variables
    public GridCell[,] grid { get; private set; }
    public Vector2Int gridSize { get; private set; }
    public float cellRadius { get; private set; }

    private float cellDiameter;
    private Vector2 startPosition;

    // Constructor
    public FlowField(float cellRadiusP, Vector2Int gridSizeP, Vector2 startPositionP)
    {
        cellRadius = cellRadiusP;
        cellDiameter = cellRadius * 2f;
        gridSize = gridSizeP;
        startPosition = startPositionP;
    }

    // Create grid
    public void CreateGrid()
    {
        // Initialize grid array
        grid = new GridCell[gridSize.x, gridSize.y];

        // Store cell locations
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 worldPosition = new Vector3(cellDiameter * x + cellRadius, cellDiameter * y + cellRadius, 0); // cellRadius (offset) will center the cell at index (0, 0) on the origin (0, 0, 0)
                grid[x, y] = new GridCell(worldPosition, new Vector2Int(x, y), startPosition);
            }
        }
    }

    public void CreateCostField()
    {
        Debug.Log("Creating cost field");

        Vector3 cellHalfSize = Vector3.one * cellRadius;
        int terrainMask = LayerMask.GetMask("Impassible", "Rough Terrain");

        foreach(GridCell currentCell in grid)
        {
            Debug.Log("First foreach");

            Collider[] obstacles = Physics.OverlapBox(currentCell.worldPosition, cellHalfSize, Quaternion.identity, terrainMask);
            bool hasIncreasedCost = false;

            foreach(Collider col in obstacles)
            {
                Debug.Log("Layer for " + col + " = " + col.gameObject.layer);

                if (col.gameObject.layer == 8)
                {
                    currentCell.IncreaseCost(255);
                    continue;
                }
                else if (!hasIncreasedCost && col.gameObject.layer == 9)
                {
                    currentCell.IncreaseCost(3);
                    hasIncreasedCost = true; // Only increases cost once, ignores overlaps
                }
            }
        }
    }
}
