using UnityEngine;

public class Node
{
    public Vector3Int coordinates;
    public NodeType nodeType;
    public int movementCost;
    public string tilemapName;

    public Node(Vector3Int coordinates, NodeType nodeType, int movementCost, string tilemapName)
    {
        this.coordinates = coordinates;
        this.nodeType = nodeType;
        this.movementCost = movementCost;
        this.tilemapName = tilemapName;
    }

    public Vector3Int GetCoords()
    {
        return coordinates;
    }

    public int GetMovementCost()
    {
        return movementCost;
    }

    public string GetTilemapName()
    {
        return tilemapName;
    }

    public bool IsImpassable()
    {
        return nodeType == NodeType.Obstacle || nodeType == NodeType.Wall;
    }
}

public enum NodeType
{
    None,
    Default,
    DifficultTerrain,
    HazardousTerrain,
    Wall,
    Corridor,
    Obstacle,
    ClosedDoor,
    OpenDoor
}
