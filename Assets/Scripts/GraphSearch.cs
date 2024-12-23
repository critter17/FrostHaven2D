using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphSearch
{
    public static BFSResult BFSGetRange(HexGrid hexGrid, Vector3Int startPoint, int movementPoints)
    {
        Dictionary<Vector3Int, Node> visitedNodes = new Dictionary<Vector3Int, Node>();
        Dictionary<Vector3Int, int> costSoFar = new Dictionary<Vector3Int, int>();
        Queue<Node> nodesToVisitQueue = new Queue<Node>();

        Node startingNode = hexGrid.GetNodeAt(startPoint);

        nodesToVisitQueue.Enqueue(startingNode);
        costSoFar.Add(startPoint, 0);
        visitedNodes.Add(startPoint, null);

        while (nodesToVisitQueue.Count > 0)
        {
            Node currentNode = nodesToVisitQueue.Dequeue();
            foreach (Vector3Int neighborPosition in hexGrid.GetNeighbors(currentNode.coordinates))
            {
                if (hexGrid.hexGrid[neighborPosition].IsImpassable())
                {
                    continue;
                }

                int nodeCost = hexGrid.hexGrid[neighborPosition].GetMovementCost();
                int currentCost = costSoFar[currentNode.coordinates];
                int newCost = currentCost + nodeCost;

                if (newCost <= movementPoints)
                {
                    if (!visitedNodes.ContainsKey(neighborPosition))
                    {
                        visitedNodes[neighborPosition] = currentNode;
                        costSoFar[neighborPosition] = newCost;
                        nodesToVisitQueue.Enqueue(hexGrid.GetNodeAt(neighborPosition));
                    }
                    else if (costSoFar[neighborPosition] > newCost)
                    {
                        costSoFar[neighborPosition] = newCost;
                        visitedNodes[neighborPosition] = currentNode;
                    }
                }
            }
        }

        return new BFSResult { visitedNodesDict = visitedNodes };
    }

    public static List<Node> GeneratePathBFS(Node current, Dictionary<Vector3Int, Node> visitedNodesDict)
    {
        List<Node> path = new List<Node>
        {
            current
        };

        while (visitedNodesDict[current.coordinates] != null)
        {
            path.Add(visitedNodesDict[current.coordinates]);
            current = visitedNodesDict[current.coordinates];
        }
        path.Reverse();
        return path.Skip(1).ToList();
    }
}

public struct BFSResult
{
    public Dictionary<Vector3Int, Node> visitedNodesDict;

    public List<Node> GetPathTo(Node destination)
    {
        if (!visitedNodesDict.ContainsKey(destination.coordinates))
        {
            return new List<Node>();
        }

        return GraphSearch.GeneratePathBFS(destination, visitedNodesDict);
    }

    public bool IsHexPositionInRange(Vector3Int position)
    {
        return visitedNodesDict.ContainsKey(position);
    }

    public IEnumerable<Vector3Int> GetRangePositions() => visitedNodesDict.Keys;
}