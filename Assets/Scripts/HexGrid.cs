using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HexGrid
{
    public Dictionary<Vector3Int, Node> hexGrid = new Dictionary<Vector3Int, Node>();
    public Dictionary<Vector3Int, List<Vector3Int>> hexNeighbors = new Dictionary<Vector3Int, List<Vector3Int>>();
    public HexGrid(List<Tilemap> groundMap, List<Tilemap> overlayMaps, Dictionary<TileBase, TileData> dataFromTiles)
    {
        CreateGraph(groundMap, overlayMaps, dataFromTiles);
        DetermineAllNeighbors();
    }

    public void CreateGraph(List<Tilemap> groundMap, List<Tilemap> overlayMaps, Dictionary<TileBase, TileData> dataFromTiles)
    {
        foreach (var overlayMap in overlayMaps)
        {
            for (int tileX = 0; tileX < overlayMap.size.x; tileX++)
            {
                for (int tileY = 0; tileY < overlayMap.size.y; tileY++)
                {
                    Vector3Int tileCoords = new Vector3Int(tileX, tileY);
                    TileBase overlayTile = overlayMap.GetTile(tileCoords);
                    string tilemapOverlayIsOver = "";

                    if (overlayTile && !hexGrid.ContainsKey(tileCoords))
                    {
                        foreach (var tilemap in groundMap)
                        {
                            TileBase tile = tilemap.GetTile(tileCoords);

                            if (tile)
                            {
                                tilemapOverlayIsOver = tilemap.name;
                            }
                        }

                        TileData overlayTileData = dataFromTiles[overlayTile];
                        Debug.Log("Overlay tile node type: " + tileCoords + " " + overlayTileData.nodeType);
                        hexGrid.Add(tileCoords, new Node(tileCoords, overlayTileData.nodeType, overlayTileData.movementCost, tilemapOverlayIsOver));
                    }
                }
            }
        }

        foreach (var map in groundMap)
        {
            for (int i = 0; i < map.size.x; i++)
            {
                for (int j = 0; j < map.size.y; j++)
                {
                    Vector3Int tileCoords = new Vector3Int(i, j);
                    TileBase groundTile = map.GetTile(tileCoords);

                    if (groundTile && !hexGrid.ContainsKey(tileCoords))
                    {
                        TileData overlayTileData = dataFromTiles[groundTile];
                        hexGrid.Add(tileCoords, new Node(tileCoords, overlayTileData.nodeType, overlayTileData.movementCost, map.name));
                    }
                }
            }
        }
    }

    public Node GetNodeAt(Vector3Int position)
    {
        return hexGrid[position];
    }

    public bool HasValidTileAt(Vector3Int position)
    {
        return hexGrid.ContainsKey(position);
    }

    public void SetNodeType(Vector3Int position, NodeType newNodeType)
    {
        hexGrid[position].nodeType = newNodeType;
    }

    public void DetermineAllNeighbors()
    {
        foreach (var hexCoordinate in hexGrid.Keys)
        {
            UpdateNeighborsFor(hexCoordinate);
        }
    }

    public void UpdateNeighborsFor(Vector3Int hexCoordinate)
    {
        hexNeighbors[hexCoordinate] = new List<Vector3Int>();

        foreach (var direction in Direction.GetDirectionList(hexCoordinate.y))
        {
            if (hexGrid.ContainsKey(hexCoordinate + direction) && NeighborsAreConnected(hexCoordinate, direction))
            {
                hexNeighbors[hexCoordinate].Add(hexCoordinate + direction);
            }
        }
    }

    public List<Vector3Int> GetNeighbors(Vector3Int hexCoordinates)
    {
        if (!hexGrid.ContainsKey(hexCoordinates))
        {
            return new List<Vector3Int>();
        }

        return hexNeighbors[hexCoordinates];
    }

    private bool NeighborsAreConnected(Vector3Int hexCoordinates, Vector3Int direction)
    {
        return
            hexGrid[hexCoordinates].nodeType == NodeType.OpenDoor ||
            hexGrid[hexCoordinates + direction].nodeType == NodeType.OpenDoor ||
            hexGrid[hexCoordinates].tilemapName == hexGrid[hexCoordinates + direction].tilemapName;
    }
}
