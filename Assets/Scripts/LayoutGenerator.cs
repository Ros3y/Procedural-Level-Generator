using UnityEngine;

public class LayoutGenerator : MonoBehaviour
{
    [System.Serializable]
    public struct LayoutParameters
    {

        public int minRoomToRoomDistance;
        public int maxRoomToRoomDistance;

        [Range(0,360)]
        public int minRoomToRoomDirection;

        [Range(0,360)]
        public int maxRoomToRoomDirection;

        [Range(0.0f,1.0f)]
        public float branchChance; 
    }
    
    [System.Serializable]
    public struct RoomParameters
    {
        public float wallThickness;
        public int minWidth;
        public int maxWidth;
        public int minHeight;
        public int maxHeight;
        public int minQuantity;
        public int maxQuantity;
    }

    public LayoutParameters layoutParameters;
    public RoomParameters roomParameters;
    public MeshRenderer previewRenderer;

    private void Start()
    {
        Generate();    
    }

    public void Generate()
    {
        LayoutStructure layout = new LayoutStructure();
        GenerateRooms(layout);
        ConvertToBitmap(layout);
    }


    private void GenerateRooms(LayoutStructure layout)
    {
        int quantity = Random.Range(roomParameters.minQuantity, roomParameters.maxQuantity + 1);
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
        int distance = Random.Range(layoutParameters.minRoomToRoomDistance, layoutParameters.maxRoomToRoomDistance + 1);
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
        int width = Random.Range(roomParameters.minWidth, roomParameters.maxWidth + 1);
        int height = Random.Range(roomParameters.minHeight, roomParameters.maxHeight + 1);
        return new Vector3(width, 0, height);
    }

    private void ConvertToBitmap(LayoutStructure layout)
    {
        Bounds totalBounds = layout.CalculateTotalBounds();
        int textureSize = Mathf.CeilToInt(Mathf.Max(totalBounds.size.x, totalBounds.size.z));
        float wallThickness = roomParameters.wallThickness;
        Texture2D texture = new Texture2D(textureSize, textureSize);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Point;
        
        for(int i = 0; i < layout.rooms.Count; i++)
        {
            LayoutStructure.Room room = layout.rooms[i];
            Bounds innerBounds = new Bounds(room.bounds.center ,room.bounds.size);
            innerBounds.Expand(new Vector3(wallThickness * -2.0f, 0, wallThickness * -2.0f));
            
            for(float pointX = room.bounds.min.x; pointX <= room.bounds.max.x; pointX += 1.0f)
            {
                for(float pointZ = room.bounds.min.z; pointZ <= room.bounds.max.z; pointZ += 1.0f)
                {
                    if(!innerBounds.Contains(new Vector3(pointX, 0, pointZ)))
                    {
                        int x = Convert3DPointTo2DPixel(pointX, totalBounds.center.x, textureSize);
                        int y = Convert3DPointTo2DPixel(pointZ, totalBounds.center.z, textureSize);
                        texture.SetPixel(x, y, Color.red);
                    }
                }
            }

            
            int centerX = Convert3DPointTo2DPixel(room.anchorPoint.x, totalBounds.center.x, textureSize);
            int centerY = Convert3DPointTo2DPixel(room.anchorPoint.z, totalBounds.center.z, textureSize);
            texture.SetPixel(centerX, centerY, Color.blue);
        }
        texture.Apply();
        previewRenderer.material.mainTexture = texture;

    }

    public int Convert3DPointTo2DPixel(float point, float centerOffset, int textureSize)
    {
        return Mathf.CeilToInt(point - centerOffset + (textureSize/2));
    }
}
