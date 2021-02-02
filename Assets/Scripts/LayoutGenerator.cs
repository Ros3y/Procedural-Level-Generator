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
        Vector3 previousPosition = Vector3.zero;
        
        for(int i = 0; i < quantity; i++)
        {
            
            int width = Random.Range(roomParameters.minWidth, roomParameters.maxWidth + 1);
            int height = Random.Range(roomParameters.minHeight, roomParameters.maxHeight + 1);
            Vector3 size = new Vector3(width, 0, height);
            
            if(i == 0)
            {
                Vector3 position = Vector3.zero;
                layout.AddRoom(position, size);
                previousPosition = position;

            }
            else
            { 
                Vector3 position = GenerateRoomPosition(previousPosition);
                layout.AddRoom(position, size);
                previousPosition = position;
            }
        }

    }

    private Vector3 GenerateRoomPosition(Vector3 from)
    {
        int distance = Random.Range(layoutParameters.minRoomToRoomDistance, layoutParameters.maxRoomToRoomDistance + 1);
        int angle = Random.Range(layoutParameters.minRoomToRoomDirection, layoutParameters.maxRoomToRoomDirection);
        Vector3 direction = new Vector3(Random.Range(-1.0f,1.0f), 0, Random.Range(-1.0f,1.0f)).normalized * distance;
        Vector3 position = (from + direction);

        return position;
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
