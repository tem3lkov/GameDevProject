using UnityEngine;

public class Node
{
    public Vector3Int gridPosition;
    public Vector2 worldPosition;
    public bool isWalkable;

    public int gCost; 
    public int hCost; 
    public Node parentNode;

    public int FCost => gCost + hCost;

    public Node(Vector3Int gridPos, Vector2 worldPos, bool walkable)
    {
        gridPosition = gridPos;
        worldPosition = worldPos;
        isWalkable = walkable;
    }
}