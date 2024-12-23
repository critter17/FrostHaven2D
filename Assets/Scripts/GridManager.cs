using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class GridManager : MonoBehaviour
{
    public Grid grid;
    [SerializeField] Tilemap hoverMap;
    [SerializeField] Tile highlightTile;
    [SerializeField] Tile selectedTile;
    [SerializeField] Tile openedDoorTile;
    public GameObject character;
    [SerializeField] Vector3Int characterStartingPosition = new Vector3Int(2, 9);
    [SerializeField] List<TileData> tileData;
    [SerializeField] List<DoorData> doorsToOpen = new List<DoorData>();

    private List<Tilemap> groundMaps = new List<Tilemap>();
    private List<Tilemap> overlayMaps = new List<Tilemap>();
    private Dictionary<TileBase, TileData> dataFromTiles;
    private HexGrid hexGrid;
    private BFSResult bfsResult;
    private List<Node> path = new List<Node>();
    private int pathCost = 0;
    private int pathCostUsed = 0;
    private List<Vector3Int> hexNeighbors = new List<Vector3Int>();

    public int movementPoints = 2;

    protected void Awake()
    {
        dataFromTiles = new Dictionary<TileBase, TileData>();

        foreach (var tileData in tileData)
        {
            foreach (var tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData);
            }
        }

        Tilemap[] tilemaps = GetComponentsInChildren<Tilemap>(true);
        foreach (var tilemap in tilemaps)
        {
            if (tilemap.CompareTag("Walkable"))
            {
                groundMaps.Add(tilemap);
            }
            else if (tilemap.CompareTag("Overlays"))
            {
                overlayMaps.Add(tilemap);
            }
        }

        hexGrid = new HexGrid(groundMaps, overlayMaps, dataFromTiles);
    }



    protected void Start()
    {
        character.transform.position = grid.CellToWorld(characterStartingPosition);
    }

    protected void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = grid.WorldToCell(mousePosition);

        if (Input.GetMouseButtonDown(0) && hexGrid.HasValidTileAt(cellPosition) && hoverMap.GetTile(cellPosition))
        {
            if (path.Count > 0 && path[^1] == hexGrid.GetNodeAt(cellPosition))
            {
                hoverMap.SetTile(path[^1].coordinates, highlightTile);
                pathCost -= path[^1].GetMovementCost();
                path.RemoveAt(path.Count - 1);

                Debug.Log("Tile at " + cellPosition + " cleared");
                EventManager.OnHexesSelected.Invoke(path.Count);
            }
            else
            {
                foreach (var hex in bfsResult.GetPathTo(hexGrid.GetNodeAt(cellPosition)))
                {
                    path.Add(hex);
                }

                Debug.Log("Destination: " + cellPosition);
                EventManager.OnHexesSelected.Invoke(path.Count);
            }

            pathCost = 0;

            foreach (var pathNode in path)
            {
                pathCost += pathNode.GetMovementCost();
            }

            if (path.Count == 0)
            {
                HighlightPossibleMovement(grid.WorldToCell(character.transform.position), movementPoints - pathCostUsed);
            }
            else
            {
                Debug.Log("Movement Left: " + movementPoints + " - " + pathCostUsed + " - " + pathCost);
                HighlightPossibleMovement(path[^1].coordinates, movementPoints - pathCostUsed - pathCost);
            }

            foreach (var pathNode in path)
            {
                hoverMap.SetTile(pathNode.coordinates, selectedTile);
            }
        }
    }

    public void StartMoveAbility(int movementPoints)
    {
        this.movementPoints = movementPoints;
        pathCostUsed = 0;
        HighlightPossibleMovement(grid.WorldToCell(character.transform.position), this.movementPoints);
    }

    public void StopMoveAbility()
    {
        Debug.Log("Stop Move Ability");
        foreach (var pathNode in path)
        {
            hoverMap.SetTile(pathNode.coordinates, null);
        }
        foreach (Vector3Int neighbor in hexNeighbors)
        {
            if (hoverMap.GetTile(neighbor) == highlightTile)
            {
                hoverMap.SetTile(neighbor, null);
            }
        }
        path.Clear();
        pathCost = 0;
        pathCostUsed = 0;
    }

    public void HighlightPossibleMovement(Vector3Int startPosition, int movementPoints)
    {
        foreach (Vector3Int neighbor in hexNeighbors)
        {
            if (hoverMap.GetTile(neighbor) == highlightTile)
            {
                hoverMap.SetTile(neighbor, null);
            }
        }

        bfsResult = GraphSearch.BFSGetRange(hexGrid, startPosition, movementPoints);
        hexNeighbors = new List<Vector3Int>(bfsResult.GetRangePositions());

        foreach (Vector3Int neighbor in hexNeighbors)
        {
            hoverMap.SetTile(neighbor, highlightTile);
        }
    }

    public void ConfirmSelection()
    {
        Vector3Int pathDestination = path[^1].coordinates;
        character.transform.position = grid.CellToWorld(pathDestination);
        Node characterNode = hexGrid.GetNodeAt(pathDestination);
        if (characterNode.nodeType == NodeType.ClosedDoor)
        {
            // Change door sprite and state, reveal next room's overlays and enemies
            Tilemap tilemapWithDoor = groundMaps.Find(tilemap =>
                hexGrid.GetNodeAt(pathDestination).nodeType == NodeType.ClosedDoor &&
                tilemap.GetTile(pathDestination));

            List<Tilemap> tilemapsToReveal = new List<Tilemap>();
            DoorData doorToOpen = doorsToOpen.Find(doorData => doorData.door == pathDestination);
            foreach (var tilemapString in doorToOpen.tilemapsToReveal)
            {
                tilemapsToReveal.Add(overlayMaps.Find(tilemap => tilemap.name == tilemapString));
            }
            foreach (var tilemap in tilemapsToReveal)
            {
                tilemap.gameObject.SetActive(true);
            }
            tilemapWithDoor.SetTile(pathDestination, openedDoorTile);
            hexGrid.SetNodeType(pathDestination, NodeType.OpenDoor);
            hexGrid.UpdateNeighborsFor(pathDestination);
        }
        HighlightPossibleMovement(pathDestination, movementPoints - pathCostUsed - pathCost);
        pathCostUsed += pathCost;
        foreach (var pathNode in path)
        {
            hoverMap.SetTile(pathNode.coordinates, null);
        }
        path.Clear();
        pathCost = 0;
        EventManager.OnHexesSelected.Invoke(path.Count);
    }
}
