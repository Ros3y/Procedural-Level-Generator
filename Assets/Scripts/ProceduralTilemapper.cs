using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

namespace Zigurous.Tilemapping
{
    [RequireComponent(typeof(Tilemap))]
    public sealed class ProceduralTilemapper : MonoBehaviour
    {
        public TileBase baseTile;
        private Tilemap _tilemap;
        private List<Bounds> _rooms;

        private void Start()
        {
            Initialize();
            GenerateMap();
            RenderMap();
            alignCamera();
        }

        private void Initialize()
        {
            _tilemap = GetComponent<Tilemap>();
            _rooms = new List<Bounds>();
        }

        private void GenerateMap()
        {
            for (int i = 0; i < 10; i++)
            {
                int x = Random.Range(-64, 64);
                int y = Random.Range(-64, 64);
                Vector3 position = new Vector3(x, y);
                Vector3 mirroredPosition = new Vector3();
                
                int randomInt = Random.Range(0, 2);

                if(randomInt == 0)
                {
                    Vector3 position2 = new Vector3(-x, y);
                    mirroredPosition = position2;
                }
                
                if(randomInt == 1)
                {
                    Vector3 position2 = new Vector3(x, -y);
                    mirroredPosition = position2;
                }
                
                if(randomInt == 2)
                {
                    Vector3 position2 = new Vector3(-x, -y);
                    mirroredPosition = position2;
                }

                


                int width = Random.Range(8, 16);
                int length = Random.Range(8, 16);
                Vector3 size = new Vector3(width, length);

                _rooms.Add(new Bounds(position, size));
                _rooms.Add(new Bounds(mirroredPosition, size));
            }
            int centralWidth = Random.Range(8, 16);
            int centralLength = Random.Range(8, 16);
            Vector3 centralSize = new Vector3(2.5f*centralWidth, 2.5f*centralLength);
            Bounds totalBounds = CalculateTotalBounds();
            _rooms.Add(new Bounds(totalBounds.center, centralSize));
        }

        private void RenderMap()
        {
            _tilemap.ClearAllTiles();

            // Draw room tiles
            foreach (Bounds room in _rooms)
            {
                for (int x = (int)room.min.x; x < room.max.x; x++)
                {
                    for (int y = (int)room.min.y; y < room.max.y; y++)
                    {
                        _tilemap.SetTile(new Vector3Int(x, y, 0), this.baseTile);
                    }
                }
            }

            ConnectAllRooms();


        }
        private void Connect2Rooms(Bounds roomA, Bounds roomB)
        {
            float distanceX = Mathf.Abs(roomA.center.x - roomB.center.x);
            float distanceY = Mathf.Abs(roomA.center.y - roomB.center.y);
            Vector3Int position = new Vector3Int();
            position.x = (int)roomA.center.x;
            position.y = (int)roomA.center.y;
            int xDirection = (int)Mathf.Clamp(roomB.center.x - roomA.center.x, -1, 1);
            int yDirection = (int)Mathf.Clamp(roomB.center.y - roomA.center.y, -1, 1);
        
            // if(Mathf.Max(roomB.center.x, roomA.center.x) == roomB.center.x)
            // {
            //     xDirection = 1;
            // }

            // if(Mathf.Max(roomB.center.x, roomA.center.x) == roomA.center.x)
            // {
            //     xDirection = -1;
            // }

            // if(Mathf.Max(roomB.center.y, roomA.center.y) == roomB.center.y)
            // {
            //     yDirection = 1;
            // }

            // if(Mathf.Max(roomB.center.y, roomA.center.y) == roomA.center.y)
            // {
            //     yDirection = -1;
            // }

            for(int i = 0; i < distanceX; i++)
            {
                _tilemap.SetTile(new Vector3Int(position.x, position.y, 0), this.baseTile);
                position.x += xDirection;
            }

            for(int i = 0; i < distanceY; i++)
            {
                _tilemap.SetTile(new Vector3Int(position.x, position.y, 0), this.baseTile);
                position.y += yDirection;
            }
        
        }

        private void ConnectAllRooms()
        {
            int roomCount = _rooms.Count;
            List<Bounds> connectedRooms = new List<Bounds>();
            List<Bounds> unconnectedRooms = new List<Bounds>();
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
                Debug.Log(unconnectedRooms[i]);
                Debug.Log("made it here");
                int randomRoom = Random.Range(0,connectedRoomCount - 1);
                Connect2Rooms(unconnectedRooms[i], connectedRooms[randomRoom]);
            }
        }

        
        private Bounds CalculateTotalBounds()
        {
            Bounds totalBounds = new Bounds();
            for(int i = 0; i < _rooms.Count; i++)
            {
                totalBounds.Encapsulate(_rooms[i]);
            }
            return totalBounds;
        }
        private void alignCamera()
        {
            Bounds totalBounds = CalculateTotalBounds();
            Vector3 cameraPosition = new Vector3(totalBounds.center.x, totalBounds.center.z, -0.3f);
            Camera.main.transform.position = cameraPosition;
            Camera.main.orthographicSize = Mathf.Max(totalBounds.size.x, totalBounds.size.z);
        }

    }

}
