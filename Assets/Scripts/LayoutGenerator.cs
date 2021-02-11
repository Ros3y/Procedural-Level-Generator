using UnityEngine;
using Zigurous.DataStructures;

public class LayoutGenerator : MonoBehaviour
{

    [System.Serializable]
    public struct LayoutParameters
    {
        public IntRange roomToRoomDistance;

    }
    
    [System.Serializable]
    public struct RoomParameters
    {
        public IntRange width;
        public IntRange length;
        public IntRange quantity;
    }

    public LayoutOutputPreview.Parameters previewParameters;
    public LayoutParameters layoutParameters;
    public RoomParameters roomParameters;

    private void Start()
    {
        Generate();    
    }

    public void Generate()
    {
        LayoutStructure layout = new LayoutStructure();
        LayoutOutputPreview preview;
        GenerateRooms(layout);
        if(previewParameters.outputType == LayoutOutputPreview.OutputType.TwoD)
        {
            preview = new LayoutOutput2DBitmapPreview();
            preview.Preview(layout, previewParameters);
        }
        else if(previewParameters.outputType == LayoutOutputPreview.OutputType.ThreeD)
        {
            preview = new LayoutOutput3DPreview();
            preview.Preview(layout, previewParameters);
        }
    }


    private void GenerateRooms(LayoutStructure layout)
    {
        int quantity = Random.Range(roomParameters.quantity.min, roomParameters.quantity.max + 1);
        int generationAttempts = quantity;
        LayoutStructure.Room previousRoom = layout.AddRoom(Vector3.zero, CalculateRoomSize());
        int direction = 0;
        
        for(int i = 1; i < generationAttempts; i++)
        {
            bool generated = false;
            direction = Random.Range(1,5);

            
            for(int attempts = 0; attempts < 4; attempts++)
            {
                Bounds bounds = CalculateRoomBounds(previousRoom, direction);
                
                if(!layout.DoesOverlapAnyRoom(bounds))
                {
                    previousRoom = layout.AddRoom(bounds);
                    generated = true;
                    break;
                }
                else if(++direction > 4)
                {
                    direction = 1;
                }
            }
            
            if(!generated)
            {
                generationAttempts++;
                if(i >= 2)
                {
                    previousRoom = layout.rooms[Random.Range(0,layout.rooms.Count - 2)];
                }
                else
                {
                    break;
                }
            }
            
        }
        Debug.Log(layout.rooms.Count);

    }

    private Bounds CalculateRoomBounds(LayoutStructure.Room room, int direction)
    {
        int distance = Random.Range(layoutParameters.roomToRoomDistance.min, layoutParameters.roomToRoomDistance.max + 1);
        Vector3 offset = Vector3.zero;
        Vector3 position = Vector3.zero;
        
        switch(direction)
        {
            case 1:
                offset.z = room.bounds.extents.z + distance;
                position = (room.anchorPoint + offset);
                break;
            
            case 2:
                offset.x = room.bounds.extents.x + distance;
                position = (room.anchorPoint + offset);
                break;
            
            case 3:
                offset.z = -room.bounds.extents.z - distance;
                position = (room.anchorPoint + offset);
                break;

            case 4:
                offset.x = -room.bounds.extents.x - distance;
                position = (room.anchorPoint + offset);
                break;
        }
  

        return new Bounds(position, CalculateRoomSize());
    }

    public Vector3 CalculateRoomSize()
    {
        int width = Random.Range(roomParameters.width.min, roomParameters.width.max + 1);
        int height = Random.Range(roomParameters.length.min, roomParameters.length.max + 1);
        return new Vector3(width, 0, height);
    }
}
