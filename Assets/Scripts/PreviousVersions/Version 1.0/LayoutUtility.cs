using UnityEngine;
using Zigurous.DataStructures;

public static class LayoutUtility
{
    public static Vector3 BranchRoom(LayoutStructure.Room from, int direction, float distance)
    {
        Vector3 offset = Vector3.zero;
        Vector3 position = Vector3.zero;
        
        switch(direction)
        {
            case 1:
                offset.z = from.bounds.extents.z + distance;
                position = (from.anchorPoint + offset);
                break;
            
            case 2:
                offset.x = from.bounds.extents.x + distance;
                position = (from.anchorPoint + offset);
                break;
            
            case 3:
                offset.z = -from.bounds.extents.z - distance;
                position = (from.anchorPoint + offset);
                break;

            case 4:
                offset.x = -from.bounds.extents.x - distance;
                position = (from.anchorPoint + offset);
                break;
        }
  

        return position;
    }

    public static Vector3 RandomRoomSize(IntRange widthRange, IntRange lengthRange)
    {
        int width = widthRange.RandomInclusive();
        int length = lengthRange.RandomInclusive();
        return new Vector3(width, 10, length);
    }    
}
