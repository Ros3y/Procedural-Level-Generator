using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class ProceduralTilemapper : MonoBehaviour
{
    private Tilemap _tilemap;
    public TileBase baseTile;
    public TileMappingParameters.Parameters parameters;
    private int _roomCount;
    private int _roomQuantity;
    private Dictionary<Vector2Int, bool> _units = new Dictionary<Vector2Int, bool>();
    private List<UnitBounds> _rooms;
    
    private void Awake()
    {
        _tilemap = GetComponent<Tilemap>();
        _rooms = new List<UnitBounds>();

    }

    private void Start()
    {
        Generate();
    }

    private void Update()
    {
        if (parameters.regenerate)
        {
            parameters.regenerate = false;
            Generate();
        }
    }

    private void Generate()
    {
        _tilemap.ClearAllTiles();
        _units.Clear();

        UnitBounds centralRoom = CreateCentralRoom();

        _roomCount = 1;
        _roomQuantity = Random.Range(parameters.roomQuantityMin, parameters.roomQuantityMax);

        BranchRecursively(centralRoom);
        if(parameters.allowAdditionalConnections)
        {
            ConnectRandomRooms();
        }
        AlignCamera();
    }

    private void AlignCamera()
    {
        Bounds totalBounds = _tilemap.localBounds;
        Vector3 cameraPosition = new Vector3(totalBounds.center.x, totalBounds.center.y, Camera.main.transform.position.z);
        Camera.main.transform.position = cameraPosition;
        Camera.main.orthographicSize = Mathf.Max(totalBounds.size.x, totalBounds.size.y) / 1.618f;
    }


    private UnitBounds CreateCentralRoom()
    {
        Vector2Int position = Vector2Int.zero;
        Vector2Int size = new Vector2Int(Random.Range(parameters.roomSizeMin.x, parameters.roomSizeMax.x), Random.Range(parameters.roomSizeMin.y, parameters.roomSizeMax.y));
        size = size * parameters.centralRoomScale;
        return CreateRoom(position, size);
    }

    private void BranchRecursively(UnitBounds room)
    {
        UnitBounds branch;
        UnitBounds randomRoom = _rooms[Random.Range(0,_rooms.Count)];
        
        if (_roomCount++ >= _roomQuantity) {
            return;
        }

        int direction = Random.Range(0, 4);
        float chance = Random.Range(0.0f,100.0f);
        if(chance > parameters.centralBranchChance)
        {
            branch = BranchRoom(room, direction);  
        }

        else
        {
            branch = BranchRoom(_rooms[0], direction);   
        }
        BranchRecursively(branch);
    }

    private UnitBounds BranchRoom(UnitBounds room, int direction)
    {
        Vector2Int size = new Vector2Int(Random.Range(parameters.roomSizeMin.x, parameters.roomSizeMax.x), Random.Range(parameters.roomSizeMin.y, parameters.roomSizeMax.y));
        Vector2Int position = room.position;
        Vector2Int centerOffset = size / 2;

        int distance = Random.Range(parameters.roomDistanceMin, parameters.roomDistanceMax);
        int offset = Random.Range(parameters.roomOffsetMin, parameters.roomOffsetMax);

        switch (direction)
        {
            case 0: // up
                position.y += room.size.y + distance;
                position.x = room.center.x + offset - centerOffset.x;
                break;

            case 1: // down
                position.y -= size.y + distance;
                position.x = room.center.x + offset - centerOffset.x;
                break;

            case 2: // right
                position.x += room.size.x + distance;
                position.y = room.center.y + offset - centerOffset.y;
                break;

            case 3: // left
                position.x -= size.x + distance;
                position.y = room.center.y + offset - centerOffset.y;
                break;
        }

        UnitBounds branch = CreateRoom(position, size);
        ConnectRooms(room, branch);
        if(parameters.symetricalBranching)
        {
            UnitBounds SymetricalBranch = GenerateSymetricalRoom(branch);
            ConnectRooms(room, SymetricalBranch);
        }
        return branch;
    }

    private UnitBounds GenerateSymetricalRoom(UnitBounds room)
    {
        int x = room.center.x;
        int y = room.center.y;
        Vector2Int mirroredPosition = new Vector2Int(-x, y);

        switch(parameters.axisOfSymetry)
             {
                case TileMappingParameters.axisOfSymetry.X:
                mirroredPosition = new Vector2Int(-x, y);
                break;

                case TileMappingParameters.axisOfSymetry.Y:
                mirroredPosition = new Vector2Int(x, -y);
                break;

                case TileMappingParameters.axisOfSymetry.Diagonal1:
                mirroredPosition = new Vector2Int(-x, -y);
                break;

                case TileMappingParameters.axisOfSymetry.Diagonal2:
                mirroredPosition = new Vector2Int(y, x);
                break;
            }

        return CreateRoom(mirroredPosition, room.size);
    }

    private void ConnectRooms(UnitBounds roomA, UnitBounds roomB)
    {
        Vector2Int centerA = roomA.center;
        Vector2Int centerB = roomB.center;

        int centering = parameters.corridorWidth / 2;

        int xMin = Mathf.Min(centerA.x, centerB.x) - centering;
        int xMax = Mathf.Max(centerA.x, centerB.x) + centering;
        int xDistance = xMax - xMin;
        int xStart = centerB.x - centering;

        int yMin = Mathf.Min(centerA.y, centerB.y) - centering;
        int yMax = Mathf.Max(centerA.y, centerB.y) + centering;
        int yDistance = yMax - yMin;
        int yStart = centerA.y - centering;

        UnitBounds corridorHorizontal = new UnitBounds(xMin, yStart, xDistance, parameters.corridorWidth);
        UnitBounds corridorVertical = new UnitBounds(xStart, yMin, parameters.corridorWidth, yDistance);

        FillUnitBounds(corridorHorizontal);
        FillUnitBounds(corridorVertical);
    }

    private void ConnectRandomRooms()
    {
        List<UnitBounds> _tempRooms = _rooms;
        int addedConnections = (int)Mathf.Floor(parameters.additionalConnections * _rooms.Count);
        for(int i = 0; i < addedConnections - 1; i++)
        {
            int randomIndexA = Random.Range(0,_tempRooms.Count);
            UnitBounds roomA = _rooms[randomIndexA];
            _tempRooms.Remove(_rooms[randomIndexA]);
            int randomIndexB = Random.Range(0,_tempRooms.Count);
            UnitBounds roomB = _rooms[randomIndexB];
            ConnectRooms(roomA, roomB);
        }
    }

    private UnitBounds CreateRoom(Vector2Int position, Vector2Int size)
    {
        float chance = Random.Range(0.0f, 100.0f);
        UnitBounds room = new UnitBounds(position, size);
        if(chance < parameters.focalRoomChance)
        {
            room.size = room.size * parameters.focalRoomScale;
        }
        SetOccupied(room, true);
        _rooms.Add(room);
        FillUnitBounds(room);
        return room;
    }

    private void SetOccupied(UnitBounds bounds, bool occupied)
    {
        for (int x = bounds.position.x; x < bounds.position.x + bounds.size.x; x++)
        {
            for (int y = bounds.position.y; y < bounds.position.y + bounds.size.y; y++)
            {
                Vector2Int unit = new Vector2Int(x, y);
                _units[unit] = occupied;
            }
        }
    }

    private void FillUnitBounds(UnitBounds bounds)
    {
        BoundsInt tileBounds = UnitBoundsToTileBounds(bounds);
        FillTileBounds(tileBounds);
    }

    private void FillTileBounds(BoundsInt bounds)
    {
        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                _tilemap.SetTile(new Vector3Int(x, y, 0), this.baseTile);
            }
        }
    }

    private BoundsInt UnitBoundsToTileBounds(UnitBounds unitBounds)
    {
        Vector3Int position = UnitToTile(unitBounds.position);
        Vector3Int size = UnitToTile(unitBounds.size);
        return new BoundsInt(position, size);
    }

    private Vector3Int UnitToTile(Vector2Int unit)
    {
        return new Vector3Int(
            unit.x * parameters.unitSize.x,
            unit.y * parameters.unitSize.y,
            0);
    }

}
