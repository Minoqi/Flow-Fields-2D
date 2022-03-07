using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class FlowField
{
    // Variables
    public GridCell[,] grid { get; private set; }
    public Vector2Int gridSize { get; private set; }
    public float cellRadius { get; private set; }

    public GridCell destinationCell; // Final destination

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
        Vector3 cellHalfSize = Vector3.one * cellRadius;
        int terrainMask = LayerMask.GetMask("Impassible", "Rough Terrain");

        foreach(GridCell currentCell in grid)
        {
            Collider[] obstacles = Physics.OverlapBox(currentCell.worldPosition, cellHalfSize, Quaternion.identity, terrainMask);
            bool hasIncreasedCost = false;

            foreach(Collider col in obstacles)
            {
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

    public void CreateIntegrationField(GridCell destinationCellP)
    {
        // Initialize destination cell data
        destinationCell = destinationCellP;
        destinationCell.cost = 0;
        destinationCell.bestCost = 0;

        Queue<GridCell> cellsToCheck = new Queue<GridCell>();
        cellsToCheck.Enqueue(destinationCell); // Adds destination cell to end of queue

        while(cellsToCheck.Count > 0)
        {
            GridCell currentCell = cellsToCheck.Dequeue(); // Get GridCell at front of list
            List<GridCell> currentNeighbors = GetNeighborCells(currentCell.gridIndex, cardinalDirections); // Get neighboring cells in top, left, right and left directions (square)

            // Converts neighbor cells integration grid cell cost as an option
            foreach(GridCell currentNeighbor in currentNeighbors)
            {
                if (currentNeighbor.cost == byte.MaxValue) // Skip since it's an impassible cell
                {
                    continue;
                }
                else if (currentNeighbor.cost + currentCell.bestCost < currentNeighbor.bestCost) // Neighbor cell is not impassible
                {
                    currentNeighbor.bestCost = (ushort)(currentNeighbor.cost + currentCell.bestCost); // Update best cost with proper value
                    cellsToCheck.Enqueue(currentNeighbor);

                }
            }
        }
    }

    public void CreateFlowField()
    {
        foreach(GridCell currentCell in grid)
        {
            // Variables
            List<GridCell> currentNeighbors = GetNeighborCells(currentCell.gridIndex, allDirections);
            int bestCost = currentCell.bestCost;

            foreach(GridCell currentNeighbor in currentNeighbors)
            {
                if (currentNeighbor.bestCost < bestCost) // If neighbor cell is cheaper than previous best cell, set new cost and direction
                {
                    bestCost = currentNeighbor.bestCost;
                    currentCell.bestDirection = cardinalAndIntercardinalDirections.DefaultIfEmpty(noDirection).FirstOrDefault(direction => direction == (currentNeighbor.gridIndex - currentCell.gridIndex));
                }
            }
        }
    }

    // Get neighboring cells
    private List<GridCell> GetNeighborCells(Vector2Int nodeIndex, List<Vector2Int> directions)
    {
        // Variables
        List<GridCell> neighborCells = new List<GridCell>();

        // Get all neighbor cells
        foreach(Vector2Int currentDirection in directions)
        {
            GridCell newNeighbor = GetCellAtRelativePos(nodeIndex, currentDirection);

            if (newNeighbor != null) // Ignores out-of-bounds (edge of grid)
            {
                neighborCells.Add(newNeighbor);
            }
        }

        return neighborCells;
    }

    // Finds neighboring cell in position agents looking
    private GridCell GetCellAtRelativePos(Vector2Int originalPosition, Vector2Int relativePosition)
    {
        // Variables
        Vector2Int finalPosition = originalPosition + relativePosition;

        // Check if out-of-bounds (edge-of-grid)
        if (finalPosition.x < 0 || finalPosition.x >= gridSize.x || finalPosition.y < 0 || finalPosition.y >= gridSize.y)
        {
            return null;
        }
        else
        {
            return grid[finalPosition.x, finalPosition.y];
        }
    }

    // Get cell user clicks on and set as destination cell
    public GridCell GetCellFromWorldPosition(Vector3 worldPosition)
    {
        // Variables
        int x, y;
        float percentX = worldPosition.x / (gridSize.x * cellDiameter);
        float percentY = worldPosition.y / (gridSize.y * cellDiameter);

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        x = Mathf.Clamp(Mathf.FloorToInt((gridSize.x) * percentX), 0, gridSize.x - 1);
        y = Mathf.Clamp(Mathf.FloorToInt((gridSize.y) * percentY), 0, gridSize.y - 1);

        return grid[x, y];
    }




    // ---------- GRID DIRECTION SECTION -------------

    // Variables
    public Vector2Int Vector;

    // Directions
    public static Vector2Int noDirection = new Vector2Int(0, 0);
    public static Vector2Int northDirection = new Vector2Int(0, 1);
    public static Vector2Int southDirection = new Vector2Int(0, -1);
    public static Vector2Int eastDirection = new Vector2Int(1, 0);
    public static Vector2Int westDirection = new Vector2Int(-1, 0);
    public static Vector2Int northEastDirection = new Vector2Int(1, 1);
    public static Vector2Int northWestDirection = new Vector2Int(-1, 1);
    public static Vector2Int southEastDirection = new Vector2Int(1, -1);
    public static Vector2Int southWestDirection = new Vector2Int(-1, -1);

    // Direction Lists
    public static List<Vector2Int> cardinalDirections = new List<Vector2Int>
    {
        northDirection,
        eastDirection,
        southDirection,
        westDirection
    };

    public static List<Vector2Int> cardinalAndIntercardinalDirections = new List<Vector2Int>
    {
        northDirection,
        northEastDirection,
        eastDirection,
        southEastDirection,
        southDirection,
        southWestDirection,
        westDirection,
        northWestDirection
    };

    public static List<Vector2Int> allDirections = new List<Vector2Int>
    {
        noDirection,
        northDirection,
        northEastDirection,
        eastDirection,
        southEastDirection,
        southDirection,
        southWestDirection,
        westDirection,
        northWestDirection
    };

    public Vector2Int GetNoDirectionCell()
    {
        return noDirection;
    }

    public Vector2Int GetNorthDirectionCell()
    {
        return northDirection;
    }

    public Vector2Int GetNorthEastDirectionCell()
    {
        return northEastDirection;
    }

    public Vector2Int GetEastDirectionCell()
    {
        return eastDirection;
    }

    public Vector2Int GetSouthEastDirectionCell()
    {
        return southEastDirection;
    }

    public Vector2Int GetSouthDirectionCell()
    {
        return southDirection;
    }

    public Vector2Int GetSouthWestDirectionCell()
    {
        return southWestDirection;
    }

    public Vector2Int GetWestDirectionCell()
    {
        return westDirection;
    }


    public Vector2Int GetNorthWestDirectionCell()
    {
        return northWestDirection;
    }
}
