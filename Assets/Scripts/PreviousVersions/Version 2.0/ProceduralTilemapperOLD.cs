using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using Zigurous.DataStructures;

namespace Zigurous.Tilemapping
{
    [RequireComponent(typeof(Tilemap))]
    public sealed class ProceduralTilemapperOLD : MonoBehaviour
    {
        public TileMappingParametersOLD.Parameters parameters;
        public TileBase baseTile;
        private Tilemap _tilemap;
        private TileMappingGrid _gridTilemap;
        private List<BoundsInt> _rooms;

        private void Start()
        {
            Initialize();
            //GenerateSymetricalRooms();
            GenerateWeb();
            //GenerateTestEnvironment();
            RenderMap();
            alignCamera();
        }

        private void Initialize()
        {
            _tilemap = GetComponent<Tilemap>();
            _gridTilemap= new TileMappingGrid(_tilemap);
            _rooms = new List<BoundsInt>();
        }

        private Vector3Int MirroredPosition(Vector3Int position)
        {
            int x = (int)position.x;
            int y = (int)position.y;
            Vector3Int mirroredPosition = new Vector3Int(-x, y, 0);

            switch(parameters.axisOfSymetry)
                {
                    case TileMappingParametersOLD.axisOfSymetry.X:
                    mirroredPosition = new Vector3Int(-x, y, 0);
                    break;

                    case TileMappingParametersOLD.axisOfSymetry.Y:
                    mirroredPosition = new Vector3Int(x, -y, 0);
                    break;

                    case TileMappingParametersOLD.axisOfSymetry.Diagonal1:
                    mirroredPosition = new Vector3Int(-x, -y, 0);
                    break;

                    case TileMappingParametersOLD.axisOfSymetry.Diagonal2:
                    mirroredPosition = new Vector3Int(y, x, 0);
                    break;
                }

            return mirroredPosition;
        }
        private void GenerateSymetricalRooms()
        {
            IntRange roomWidths = parameters.roomWidth;
            IntRange roomLengths = parameters.roomLength;
            int attempts = Random.Range(parameters.roomQuantity.min, parameters.roomQuantity.max);
            BoundsInt totalBounds = _tilemap.cellBounds;
                    
            for(int i = 0; i < attempts; i++)
            {
                int x = Random.Range(-64,64);
                int y = Random.Range(-64,64);
                Vector3Int position = new Vector3Int(x, y, 0);
                Vector3Int mirroredPosition = MirroredPosition(position);
               
               int width = Random.Range(roomWidths.min, roomWidths.max);
               int length = Random.Range(roomLengths.min, roomLengths.max);
               Vector3Int size = new Vector3Int(width, length, 0);
                _rooms.Add(new BoundsInt(position, size));
                _rooms.Add(new BoundsInt(mirroredPosition, size));
            }

            if(parameters.hasHubRoom)
            {
                int scale = parameters.hubRoomScale;
                
                if(parameters.numberOfHubRooms%2 != 0)
                {
                    int hubRoomWidth = Random.Range(roomWidths.min, roomWidths.max);
                    int hubRoomLength = Random.Range(roomLengths.min, roomLengths.max);
                    Vector3Int centralSize = new Vector3Int(scale*hubRoomWidth, scale*hubRoomLength, 0);
                    Vector3Int totalBoundsCenter = new Vector3Int((int)totalBounds.center.x, (int)totalBounds.center.y, 0);
                    _rooms.Add(new BoundsInt(totalBoundsCenter, centralSize));
                    
                    for(int i = 0; i < (parameters.numberOfHubRooms - 1)/2; i++)
                    {
                        
                        int x = Random.Range(-128,128);
                        int y = Random.Range(-128,128);
                        Vector3Int position = new Vector3Int(x, y, 0);
                        Vector3Int mirroredPosition = MirroredPosition(position);
                        _rooms.Add(new BoundsInt(position, centralSize));
                        _rooms.Add(new BoundsInt(mirroredPosition, centralSize));
                    }
                }

                else
                {
                    int hubRoomWidth = Random.Range(roomWidths.min, roomWidths.max);
                    int hubRoomLength = Random.Range(roomLengths.min, roomLengths.max);
                    Vector3Int centralSize = new Vector3Int(scale*hubRoomWidth, scale*hubRoomLength, 0);
                    
                    for(int i = 0; i < (parameters.numberOfHubRooms)/2; i++)
                    {
                        int x = Random.Range(-128,128);
                        int y = Random.Range(-128,128);
                        Vector3Int position = new Vector3Int(x, y, 0);
                        Vector3Int mirroredPosition = MirroredPosition(position);
                        _rooms.Add(new BoundsInt(position, centralSize));
                        _rooms.Add(new BoundsInt(mirroredPosition, centralSize));
                    }
                }
                
            }
        }

        private void RenderMap()
        {
            // _tilemap.ClearAllTiles();

            // Draw room tiles
            foreach (BoundsInt room in _rooms)
            {
                for (int x = room.min.x; x < room.max.x; x++)
                {
                    for (int y = room.min.y; y < room.max.y; y++)
                    {
                        _tilemap.SetTile(new Vector3Int(x, y, 0), this.baseTile);
                    }
                }
            }

            //ConnectAllRooms();
            //TODO: make the render happen as the rooms are created


        }
        private void fillBounds(BoundsInt bounds)
        {
            for (int x = bounds.min.x; x < bounds.max.x; x++)
                {
                    for (int y = bounds.min.y; y < bounds.max.y; y++)
                    {
                        _tilemap.SetTile(new Vector3Int(x, y, 0), this.baseTile);
                    }
                }
        }
        private void Connect2Rooms(BoundsInt roomA, BoundsInt roomB)
        {
            int distanceX = (int)Mathf.Abs(roomA.center.x - roomB.center.x);
            int distanceY = (int)Mathf.Abs(roomA.center.y - roomB.center.y);
            Vector3Int position = new Vector3Int();
            position.x = (int)roomA.center.x;
            position.y = (int)roomA.center.y;
            int yDirection = (int)Mathf.Clamp(roomB.center.y - roomA.center.y, -1, 1);
            int xDirection = (int)Mathf.Clamp(roomB.center.x - roomA.center.x, -1, 1);
            int corridorWidth = parameters.corridorWidth;
            Vector2Int centerA = new Vector2Int((int)roomA.center.x, (int)roomA.center.y);
            Vector2Int centerB = new Vector2Int((int)roomB.center.x, (int)roomB.center.y);

            int offset = corridorWidth / 2;
            int xMin = Mathf.Min(centerA.x, centerB.x) - offset;
            int xMax = Mathf.Max(centerA.x, centerB.x) + offset;
            int xDistance = xMax - xMin;
            int yStart = centerA.y - offset;

            BoundsInt corridorHorizontal = new BoundsInt(xMin, yStart, 0, xDistance, corridorWidth, 0);

            int yMin = Mathf.Min(centerA.y, centerB.y) - offset;
            int yMax = Mathf.Max(centerA.y, centerB.y) + offset;
            int yDistance = yMax - yMin;
            int xStart = centerB.x - offset;

            BoundsInt corridorVertical = new BoundsInt(xStart, yMin, 0, corridorWidth, yDistance, 0);
            
            
            
            
            
            
            
            
            

            fillBounds(corridorHorizontal);
            fillBounds(corridorVertical);
            

        
           

            // for(int i = 0; i < distanceX; i++)
            // {
            //     position.x += xDirection;
            //     _tilemap.SetTile(position, this.baseTile);
            
            // }

            // for(int i = 0; i < distanceY; i++)
            // {
            //     position.y += yDirection;
            //     _tilemap.SetTile(position, this.baseTile);
            // }

        }

        private void ConnectAllRooms()
        {
            int roomCount = _rooms.Count;
            List<BoundsInt> connectedRooms = new List<BoundsInt>();
            List<BoundsInt> unconnectedRooms = new List<BoundsInt>();
            int connectedRoomCount = connectedRooms.Count;
            
            for(int i = 0; i < roomCount - 1; i++)
            {
                int chance = Random.Range(0,10);
                if(chance >= 5)
                {
                    Connect2Rooms(_rooms[roomCount - 1], _rooms[i]);
                    connectedRooms.Add(_rooms[i]);
                }
                else
                {
                    unconnectedRooms.Add(_rooms[i]);
                }
            }
            
            for(int i = 0; i < unconnectedRooms.Count; i++)
            {
                int randomRoom = Random.Range(0,connectedRoomCount - 1);
                Connect2Rooms(unconnectedRooms[i], connectedRooms[randomRoom]);
            }
        }

        private void alignCamera()
        {
            BoundsInt totalBounds = _tilemap.cellBounds;
            Vector3 cameraPosition = new Vector3(totalBounds.center.x, totalBounds.center.z, -0.3f);
            Camera.main.transform.position = cameraPosition;
            Camera.main.orthographicSize = Mathf.Max(totalBounds.size.x, totalBounds.size.z);
        }

        private BoundsInt CreateBoundsAtCenterPoint(Vector3Int center, Vector3Int size)
        {
            Vector3Int position = new Vector3Int(center.x - (size.x / 2), center.y - (size.y / 2), 0);
            return new BoundsInt(position, size);
        }

        private BoundsInt BranchRoom(BoundsInt room, int direction)
        {
            
            
            int width = Random.Range(parameters.roomWidth.min, parameters.roomWidth.max);
            int length = Random.Range(parameters.roomLength.min, parameters.roomLength.max);
            Vector3Int size = new Vector3Int(width, length, 0);
            Vector3Int position = new Vector3Int();
            BoundsInt newRoom = new BoundsInt();
            int roomDistance = Random.Range(parameters.roomToRoomDistance.min, parameters.roomToRoomDistance.max);
            int roomOffset = Random.Range(parameters.roomOffset.min, parameters.roomOffset.max);
            Vector3Int centerOffset = new Vector3Int(size.x / 2, size.y /2, 0);
            
            switch(direction)
            {
                case 0: // up
                position = new Vector3Int((int)room.center.x, room.yMax, 0);
                position.y += roomDistance + centerOffset.y;
                position.x += roomOffset;
                newRoom = CreateBoundsAtCenterPoint(position, size);
                _rooms.Add(newRoom);
                Connect2Rooms(room, newRoom);
                return newRoom;

                case 1: // right
                position = new Vector3Int(room.xMax, (int)room.center.y, 0);
                position.y += roomOffset;
                position.x += roomDistance + centerOffset.x;
                newRoom = CreateBoundsAtCenterPoint(position, size);
                _rooms.Add(newRoom);
                Connect2Rooms(room, newRoom);
                return newRoom;

                case 2: // down
                position = new Vector3Int((int)room.center.x, room.yMin, 0);
                position.y -= roomDistance + centerOffset.y;
                position.x += roomOffset;
                newRoom = CreateBoundsAtCenterPoint(position, size);
                _rooms.Add(newRoom);
                Connect2Rooms(room, newRoom);
                return newRoom;
                

                case 3: // left
                position = new Vector3Int(room.xMin, (int)room.center.y, 0);
                position.y += roomOffset;
                position.x -= roomDistance + centerOffset.x;
                newRoom = CreateBoundsAtCenterPoint(position, size);
                _rooms.Add(newRoom);
                Connect2Rooms(room, newRoom);
                return newRoom; 
            }
            return newRoom;
        }

        private void BranchRooms(BoundsInt room, int direction, int numberOfRooms)
        {
            BoundsInt branchRoom = room;
            for(int i = 0; i < numberOfRooms; i ++)
            {
                branchRoom = BranchRoom(branchRoom, direction);
            }
        }

        private void connect2RandomRooms()
        {
            int a = Random.Range(0, _rooms.Count);
            int b = Random.Range(0, _rooms.Count);
            Vector3Int proximity = parameters.proximityScale * _rooms[a].size;
            BoundsInt roomA = _rooms[a];
            Vector3Int roomACenter = new Vector3Int((int)roomA.center.x, (int)roomA.center.y, 0);
            BoundsInt potentialConnections = new BoundsInt(roomACenter, proximity);
            BoundsInt roomB = new BoundsInt();
            Vector3Int roomBcenter = new Vector3Int((int)_rooms[b].center.x, (int)_rooms[b].center.y, 0);

            while(a == b && !(potentialConnections.Contains(roomBcenter)))
            {
                b = Random.Range(0, _rooms.Count);
            }

            if(a != b && potentialConnections.Contains(roomBcenter))
            {
                roomB = _rooms[b];
                Connect2Rooms(roomA, roomB);
            }
        }

        private void GenerateWeb()
        {
            int totalRooms = Random.Range(parameters.roomQuantity.min, parameters.roomQuantity.max);
            int roomCount = 0;
            int index = 0;
            int direction = 0;
            int hubRoomWidth = Random.Range(parameters.roomWidth.min, parameters.roomWidth.max);
            int hubRoomLength = Random.Range(parameters.roomLength.min, parameters.roomLength.max);
            Vector3Int centralSize = new Vector3Int(parameters.hubRoomScale*hubRoomWidth, parameters.hubRoomScale*hubRoomLength, 0);
            Vector3Int centerPosition = new Vector3Int();
            BoundsInt centerRoom = new BoundsInt(centerPosition, centralSize);
            _rooms.Add(centerRoom);
            while(direction < 4)
            {
                BranchRooms(centerRoom, direction, parameters.branchLength);
                roomCount += parameters.branchLength;
                direction++;
            }
            while(roomCount < totalRooms && parameters.branchLength > 0 )
            {
                direction = Random.Range(1,4);
                index = Random.Range(0, _rooms.Count);
                BranchRooms(_rooms[index], direction, parameters.branchLength);
                if(totalRooms - roomCount < parameters.branchLength)
                {
                   BranchRooms(_rooms[index], direction, totalRooms - roomCount);
                   roomCount = totalRooms;
                   break;    
                }
                roomCount += parameters.branchLength;
            }
            
            for(int i = 0; i < parameters.additionalConnections; i++)
            {
                connect2RandomRooms();
            }
        }

        private void GenerateTestEnvironment()
        {
            int totalRooms = Random.Range(parameters.roomQuantity.min, parameters.roomQuantity.max);
            Vector3Int center = new Vector3Int(0,0,0);
            Vector3Int defaultSize = new Vector3Int(parameters.roomWidth.max,parameters.roomLength.max,0);
            BoundsInt startRoom = new BoundsInt(center, defaultSize);
            BoundsInt previousRoom = startRoom;
            for(int i = 1; i < totalRooms; i++)
            {
                _rooms.Add(previousRoom);
                int width = Random.Range(parameters.roomWidth.min, parameters.roomWidth.max);
                int length = Random.Range(parameters.roomLength.min, parameters.roomLength.max);
                Vector3Int size = new Vector3Int(width, length, 0);
                int randomX = Random.Range(0,64);
                int randomY = Random.Range(0,64);
                Vector3Int position = new Vector3Int(randomX, randomY, 0);
                BoundsInt newRoom = new BoundsInt(position, size);
                _rooms.Add(newRoom);
                Connect2Rooms(previousRoom,newRoom);
                previousRoom = newRoom;

            }
        }

        private void GenerateRoom()
        {
            int roomWidth = Random.Range(parameters.roomWidth.min, parameters.roomWidth.max);
            int roomLength = Random.Range(parameters.roomLength.min, parameters.roomLength.max);
        }

        

    }

}
