using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStarGrid : MonoBehaviour
{
    [Tooltip("Drag your Green A* Tilemap here")]
    public Tilemap aStarTilemap;

    [Tooltip("Assign your Obstacles layer here so enemies path around them!")]
    public LayerMask unwalkableMask;

    private Dictionary<Vector3Int, Node> grid = new Dictionary<Vector3Int, Node>();
    private Vector2 cellSize;
    private float nodeRadius;

    public void InitializeGrid()
    {
        grid.Clear();
        cellSize = new Vector2(aStarTilemap.layoutGrid.cellSize.x, aStarTilemap.layoutGrid.cellSize.y);

        nodeRadius = 0.05f;

        BoundsInt bounds = aStarTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);

                bool isWalkable = aStarTilemap.HasTile(cellPosition);
                Vector3 worldPos = aStarTilemap.GetCellCenterWorld(cellPosition);

                if (isWalkable)
                {
                    isWalkable = !Physics2D.OverlapCircle(worldPos, nodeRadius, unwalkableMask);
                }

                grid.Add(cellPosition, new Node(cellPosition, worldPos, isWalkable));
            }
        }
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        Vector3Int cellPos = aStarTilemap.WorldToCell(worldPosition);
        if (grid.ContainsKey(cellPos)) return grid[cellPos];
        return null;
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

                if (grid.TryGetValue(checkPos, out Node neighborNode))
                {
                    if (Mathf.Abs(x) == 1 && Mathf.Abs(y) == 1)
                    {
                        Vector3Int ortho1 = new Vector3Int(node.gridPosition.x + x, node.gridPosition.y, 0);
                        Vector3Int ortho2 = new Vector3Int(node.gridPosition.x, node.gridPosition.y + y, 0);

                        bool isOrtho1Walkable = grid.ContainsKey(ortho1) && grid[ortho1].isWalkable;
                        bool isOrtho2Walkable = grid.ContainsKey(ortho2) && grid[ortho2].isWalkable;

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