using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell
{
    // Variables
    public Vector3 worldPosition;
    public Vector2Int gridIndex;
    public Vector2 startPosition;
    public byte cost; // 0 - 255 (cost of grid cell)

    // Constructor
    public GridCell(Vector3 worldPositionP, Vector2Int gridIndexP, Vector2 startPositionP)
    {
        worldPosition = worldPositionP;
        gridIndex = gridIndexP;
        startPosition = startPositionP;
        cost = 1; // Default cost for flat ground
    }

    public void IncreaseCost(int increaseAmount)
    {
        if (cost == byte.MaxValue)
        {
            return;
        }
        else if (increaseAmount + cost > byte.MaxValue)
        {
            cost = byte.MaxValue;
        }
        else
        {
            cost += (byte)increaseAmount;
        }
    }
}
