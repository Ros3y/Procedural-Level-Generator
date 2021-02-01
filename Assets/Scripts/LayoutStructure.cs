using System.Collections.Generic;
using UnityEngine;

public class LayoutStructure
{
    public struct Room
    {
        public Bounds bounds;
        public Vector3 anchorPoint;

        public Room(Vector3 position, Vector3 size)
        {
            this.bounds = new Bounds(position, size);
            this.anchorPoint = this.bounds.center;
        }
    }
    
    public Bounds area {get; private set;}
    public List<Room> rooms {get; private set;} = new List<Room>();
    
    public LayoutStructure(int width, int height)
    {
        this.area = new Bounds(Vector3.zero, new Vector3(width, 0, height));
    }

    public void AddRoom(Vector3 position, Vector3 size)
    {
        Room room = new Room(position, size);
        rooms.Add(room);
    }
}
