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

        public Room(Bounds bounds)
        {
            this.bounds = bounds;
            this.anchorPoint = this.bounds.center;
        }
    }
    
    public List<Room> rooms {get; private set;} = new List<Room>();
    
    

    public Room AddRoom(Vector3 position, Vector3 size)
    {
        Room room = new Room(position, size);
        rooms.Add(room);
        return room;
    }

    public Room AddRoom(Bounds bounds)
    {
        Room room = new Room(bounds);
        rooms.Add(room);
        return room;
    }

    public Bounds CalculateTotalBounds()
    {
        Bounds totalBounds = new Bounds();
        for(int i = 0; i < rooms.Count; i++)
        {
            totalBounds.Encapsulate(rooms[i].bounds);
        }
        return totalBounds;
    }

    public bool DoesOverlap(Room room, Bounds bounds)
    {   
        return room.bounds.Intersects(bounds);
    }

    public bool DoesOverlapAnyRoom(Bounds bounds)
    {
        for(int i = 0; i < rooms.Count; i++)
        {
            if(DoesOverlap(rooms[i], bounds))
            {
                return true;
            }
        }
        return false;
    }
}
