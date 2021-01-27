using System.Collections.Generic;
using UnityEngine;

public class LayoutStructure
{
    public struct Room
    {
        public Bounds bounds;
    }
    private Bounds area;
    private List<Room> rooms = new List<Room>();
    
}
