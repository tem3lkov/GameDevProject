using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStarGrid : MonoBehaviour
{
    [Tooltip("Drag your Green A* Tilemap here")]
    public Tilemap aStarTilemap;

    [Tooltip("Assign your Obstacles layer here so enemies path around them!")]
    public LayerMask unwalkableMask;

    private Node[,] grid;

    private int gridOriginX;
    private int gridOriginY;
    private int gridWidth;
    private int gridHeight;

    private Vector2 cellSize;
    private float nodeRadius;

    public void InitializeGrid()
    {
        cellSize = new Vector2(aStarTilemap.layoutGrid.cellSize.x, aStarTilemap.layoutGrid.cellSize.y);
        nodeRadius = 0.05f;

        BoundsInt bounds = aStarTilemap.cellBounds;

        gridOriginX = bounds.xMin;
        gridOriginY = bounds.yMin;
        gridWidth = bounds.size.x;
        gridHeight = bounds.size.y;

        grid = new Node[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x + gridOriginX, y + gridOriginY, 0);

                bool isWalkable = aStarTilemap.HasTile(cellPosition);
                Vector3 worldPos = aStarTilemap.GetCellCenterWorld(cellPosition);

                if (isWalkable)
                {
                    isWalkable = !Physics2D.OverlapCircle(worldPos, nodeRadius, unwalkableMask);
                }

                grid[x, y] = new Node(cellPosition, worldPos, isWalkable);
            }
        }
    }

    public Node GetNode(Vector3Int cellPos)
    {
        int arrayX = cellPos.x - gridOriginX;
        int arrayY = cellPos.y - gridOriginY;

        if (arrayX >= 0 && arrayX < gridWidth && arrayY >= 0 && arrayY < gridHeight)
        {
            return grid[arrayX, arrayY];
        }
        return null;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        Vector3Int cellPos = aStarTilemap.WorldToCell(worldPosition);
        return GetNode(cellPos);
    }

    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                Vector3Int checkPos = new Vector3Int(node.gridPosition.x + x, node.gridPosition.y + y, 0);

                Node neighborNode = GetNode(checkPos);

                if (neighborNode != null)
                {
                    if (Mathf.Abs(x) == 1 && Mathf.Abs(y) == 1)
                    {
                        Vector3Int ortho1 = new Vector3Int(node.gridPosition.x + x, node.gridPosition.y, 0);
                        Vector3Int ortho2 = new Vector3Int(node.gridPosition.x, node.gridPosition.y + y, 0);

                        Node nodeOrtho1 = GetNode(ortho1);
                        Node nodeOrtho2 = GetNode(ortho2);

                        bool isOrtho1Walkable = nodeOrtho1 != null && nodeOrtho1.isWalkable;
                        bool isOrtho2Walkable = nodeOrtho2 != null && nodeOrtho2.isWalkable;

                        if (!isOrtho1Walkable || !isOrtho2Walkable)
                        {
                            continue;
                        }
                    }

                    neighbors.Add(neighborNode);
                }
            }
        }
        return neighbors;
    }
}