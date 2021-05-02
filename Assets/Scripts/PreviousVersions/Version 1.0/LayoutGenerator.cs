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
        Vector3 initialSize = LayoutUtility.RandomRoomSize(roomParameters.width, roomParameters.length);
        LayoutStructure.Room previousRoom = layout.AddRoom(Vector3.zero, initialSize);
        int direction = 0;
        
        for(int i = 1; i < generationAttempts; i++)
        {
            bool generated = false;
            direction = Random.Range(1,5);

            for(int attempts = 0; attempts < 4; attempts++)
            {
                int distance = layoutParameters.roomToRoomDistance.RandomInclusive();
                Vector3 position = LayoutUtility.BranchRoom(previousRoom, direction, distance);
                Vector3 size = LayoutUtility.RandomRoomSize(roomParameters.width, roomParameters.length);
                Bounds bounds = new Bounds(position, size);

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
}
