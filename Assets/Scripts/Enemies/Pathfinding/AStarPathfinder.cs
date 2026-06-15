using System.Collections.Generic;
using UnityEngine;

public static class AStarPathfinder
{
    public static List<Vector2> FindPath(AStarGrid grid, Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode == null) return null;

        if (targetNode == null || !targetNode.isWalkable)
        {
            targetNode = FindClosestWalkableNode(grid, targetPos);
            if (targetNode == null) return null;
        }

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost ||
                   (openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbor in grid.GetNeighbors(currentNode))
            {
                if (!neighbor.isWalkable || closedSet.Contains(neighbor)) continue;

                int moveCost = currentNode.gCost + GetDistance(currentNode, neighbor);

                if (moveCost < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = moveCost;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parentNode = currentNode;

                    if (!openSet.Contains(neighbor)) openSet.Add(neighbor);
                }
            }
        }
        return null;
    }

    private static List<Vector2> RetracePath(Node startNode, Node endNode)
    {
        List<Vector2> path = new List<Vector2>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.worldPosition);
            currentNode = currentNode.parentNode;
        }

        path.Reverse();
        return path;
    }

    private static int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
        int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

        if (dstX > dstY) return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    private static Node FindClosestWalkableNode(AStarGrid grid, Vector3 targetPos)
    {
        Vector3Int cellPos = grid.aStarTilemap.WorldToCell(targetPos);

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector3Int checkPos = new Vector3Int(cellPos.x + x, cellPos.y + y, 0);

                Vector3 worldPos = grid.aStarTilemap.GetCellCenterWorld(checkPos);
                Node potentialNode = grid.NodeFromWorldPoint(worldPos);

                if (potentialNode != null && potentialNode.isWalkable)
                {
                    return potentialNode;
                }
            }
        }
        return null;
    }
}